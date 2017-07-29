using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;
using System.Text;
using System.IO;

public class MainGame : MonoBehaviour{
	public Text levelTitle;
	public GameObject[] prefabs;
	public GameObject complimentScreen;
	public Text complimentText;
	public Text complimentTextShadow;
	PieceClasses.GameBoard mainBoard;
	PieceClasses.GameBoard oldBoard;
	int heldPieceIndex=-1;
	Vector2 startPositionGrid;
	Vector2 lastPosition;
	bool animating = false;
	Animation mainAnimation;
	Scheduler mainScheduler;
	float cameraHeight, cameraWidth;
	float topHeight;
	
	void Start (){
		mainScheduler = new Scheduler ();
		cameraHeight = 2f * Camera.main.orthographicSize;
		cameraWidth = cameraHeight * Camera.main.aspect;
		levelTitle.text = PackPresets.currentPack.currentLevel+1+"";
		mainBoard=importPiecesFromFile ("Pack"+(PackPresets.currentPack.packI+1)+"Level"+(PackPresets.currentPack.currentLevel+1));
		mainLevelLoop();
	}
	
	public void animate(PieceClasses.Piece piece, Vector2 endPosition, float speed){
		mainAnimation= new Animation (piece, endPosition, new Vector2(1f,1f), true, speed);
		animating = true;
	}
	
	void mainLevelLoop(){

		mainScheduler.addUpdateFunction (updateGame);
		mainScheduler.addTime (.5f);
		mainScheduler.addInitFunctionWithTime (showRandomCompliment, .5f);

		mainScheduler.addInitFunction (delegate() {
			PackPresets.currentPack.solveLevel();
			animating = false;
		});

		mainScheduler.addInitFunction (setupTransition); //also imports new board
		mainScheduler.addUpdateFunction (animatePieces);

		mainScheduler.addInitFunction (delegate() {
			complimentScreen.SetActive(false);
			levelTitle.text = PackPresets.currentPack.currentLevel+1+"";
			Destroy(oldBoard.gameObject);
			mainBoard.backgroundPiece.gameObject.SetActive(true);
		});

		mainScheduler.addInitFunction (mainLevelLoop);

	}

	bool animatePieces(){
		bool done = true;
		foreach (PieceClasses.Piece piece in mainBoard.pieces)
			if (piece.isAnimating){
				done=false;
				piece.isAnimating=piece.animation.animate();
			}
		return !done;
	}

	void setupTransition(){
		float ANIMATION_TIME = .8f;
		oldBoard = mainBoard;
		mainBoard=importPiecesFromFile ("Pack"+(PackPresets.currentPack.packI+1)+"Level"+(PackPresets.currentPack.currentLevel+1));
		Vector2[] transitionEndPositions = new Vector2[mainBoard.pieces.Length];
		for (int i=0; i<mainBoard.pieces.Length; i++) {
			transitionEndPositions [i] = mainBoard.pieces [i].gameObject.transform.localPosition;
			for (int y=0; y<mainBoard.pieces[i].height; y++)//set all pieces to transition to white
				for (int x=0; x<mainBoard.pieces[i].width; x++)
					if (mainBoard.pieces[i].squaresOn[x,y]){
						mainBoard.pieces[i].squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=new Color(1,1,1,0);
						mainBoard.pieces[i].isAnimating=true;
						mainBoard.pieces[i].animation=new Animation(mainBoard.pieces[i], transitionEndPositions [i], new Vector2(1f,1f), false, ANIMATION_TIME);
					}
		}

		int numPiecesMatched = 0;
		for (int pieceI=0; pieceI<mainBoard.pieces.Length; pieceI++)
			for (int oldBoardY=(int)oldBoard.backgroundPiece.position.y; oldBoardY<oldBoard.size.y-mainBoard.pieces[pieceI].height+1; oldBoardY++)
				for (int oldBoardX=(int)oldBoard.backgroundPiece.position.x; oldBoardX<oldBoard.size.x-mainBoard.pieces[pieceI].width+1; oldBoardX++)
					if (oldBoard.squaresOn [oldBoardX, oldBoardY]) { //iterate thru oldboard
						bool pieceMatchesBoard = true;
						for (int pieceX=0; pieceX<mainBoard.pieces[pieceI].width; pieceX++)
							for (int pieceY=0; pieceY<mainBoard.pieces[pieceI].height; pieceY++)
								if (mainBoard.pieces [pieceI].squaresOn [pieceX, pieceY] && !oldBoard.squaresOn [oldBoardX + pieceX, oldBoardY + pieceY]) //piece doesnt match board
									pieceMatchesBoard = false;
						if (pieceMatchesBoard) {
							numPiecesMatched++;
							Vector2 pos = mainBoard.gameObject.transform.InverseTransformPoint (oldBoard.gameObject.transform.TransformPoint (new Vector2 (oldBoardX, -oldBoardY)));
							mainBoard.pieces [pieceI].gameObject.transform.localPosition = pos;
							float boardRatio = oldBoard.gameObject.transform.localScale.x / mainBoard.gameObject.transform.localScale.x;
							mainBoard.pieces [pieceI].gameObject.transform.localScale = new Vector2 (boardRatio, boardRatio);
							for (int pieceX=0; pieceX<mainBoard.pieces[pieceI].width; pieceX++)
								for (int pieceY=0; pieceY<mainBoard.pieces[pieceI].height; pieceY++)
									if (mainBoard.pieces [pieceI].squaresOn [pieceX, pieceY]) { //iterate thru new piece squares
										oldBoard.squaresOn [oldBoardX + pieceX, oldBoardY + pieceY] = false;
										oldBoard.squares [oldBoardX + pieceX, oldBoardY + pieceY].gameObject.SetActive (false);
										mainBoard.pieces [pieceI].squares [pieceX, pieceY].gameObject.GetComponent<SpriteRenderer> ().color = oldBoard.squares [oldBoardX + pieceX, oldBoardY + pieceY].color;
										mainBoard.pieces [pieceI].squares [pieceX, pieceY].color = mainBoard.pieces [pieceI].color;
									}
							mainBoard.pieces [pieceI].animation = new Animation (mainBoard.pieces [pieceI], transitionEndPositions [pieceI], new Vector2 (1f, 1f), false, ANIMATION_TIME);
							oldBoardY=(int)(oldBoard.size.y-mainBoard.pieces[pieceI].height+1);
							oldBoardX=(int)(oldBoard.size.x-mainBoard.pieces[pieceI].width+1);
						}
					}
		if (numPiecesMatched == 0)
			oldBoard.gameObject.SetActive (false);
		mainBoard.backgroundPiece.gameObject.SetActive (false);
		mainBoard.setSpritesOrderInLayer ();
		oldBoard.setSpritesOrderInLayer ("Old Pieces");
		updateAllGameObjects (mainBoard);

		//Debug.Break ();
	}
	
	PieceClasses.GameBoard importPiecesFromFile(string filename){
		Debug.Log ("Loading:"+filename);
		TextAsset reader = (TextAsset)Resources.Load (filename);
		string jsonString = reader.text;
		
		JSONNode gameDict = JSON.Parse (jsonString);
		PieceClasses.GameBoard board = new PieceClasses.GameBoard(new Vector2(gameDict["boardSize"][0].AsFloat, gameDict["boardSize"][1].AsFloat));//new board

		JSONArray piecesArray = gameDict["pieces"].AsArray; //Create pieces
		board.pieces=new PieceClasses.Piece[piecesArray.Count];
		for (int i=0;i<piecesArray.Count;i++){
			board.pieces[i]=importPiece(piecesArray[i]);
		}

		board.backgroundPiece=importPiece(gameDict["backgroundPiece"]);//Create Background piece
		board.backgroundPiece.position = new Vector2 ((int)(board.size.x/2-board.backgroundPiece.width / 2), (int)(board.size.y/2-board.backgroundPiece.height/2));

		createBoardGameObjects (board);

		return board;
	}

	void createBoardGameObjects(PieceClasses.GameBoard board){
		board.gameObject = new GameObject ("Board");//create board
		foreach (PieceClasses.Piece piece in board.pieces) {//create pieces
			createPieceGameObjects (piece, board);
		}
		createPieceGameObjects (board.backgroundPiece, board, 1.2f, 1.2f, false, "Background Piece");//create background piece
		board.backgroundPiece.setSortingLayer("Background Piece");
		topHeight=(50f/(float)Camera.main.pixelHeight)*cameraHeight;
		Rect viewRect = new Rect (0, -topHeight/2f, cameraWidth, cameraHeight-topHeight);
		float boardRatio = board.size.x / board.size.y;
		if (boardRatio < viewRect.width / viewRect.height) {
			board.gameObject.transform.localScale= new Vector3 (viewRect.height*boardRatio/board.size.x, viewRect.height/board.size.y, 1f); //scale to height
			board.gameObject.transform.position = new Vector2 (-viewRect.height*boardRatio/2+viewRect.x, viewRect.height/2+viewRect.y);
		} else {
			board.gameObject.transform.localScale= new Vector3 (viewRect.width/board.size.x, viewRect.width/boardRatio/board.size.y, 1f); //scale to width
			board.gameObject.transform.position = new Vector2 (-viewRect.width/2+viewRect.x, viewRect.width/boardRatio/2+viewRect.y);
		}
	}
	
	PieceClasses.Piece importPiece(JSONNode pieceDict){ //make piece object from jsondict of a piece
		bool[,] squaresOn=new bool[pieceDict["size"][0].AsInt,pieceDict["size"][1].AsInt];
		int i=0;
		for (int y=0;y<pieceDict["size"][1].AsInt;y++)
		for (int x=0;x<pieceDict["size"][0].AsInt;x++){
			squaresOn[x,y]=pieceDict["squaresOn"][i].AsInt==1;
			i++;
		}
		Color color=new Color(pieceDict["color"][0].AsFloat/255f,pieceDict["color"][1].AsFloat/255f,pieceDict["color"][2].AsFloat/255f);
		return new PieceClasses.Piece (squaresOn, color, new Vector2(pieceDict["startPos"][0].AsFloat, pieceDict["startPos"][1].AsFloat));
	}
	
	void createPieceGameObjects(PieceClasses.Piece piece, PieceClasses.GameBoard board1, float scaleX=1, float scaleY=1, bool checkIntersection=true, string gameObjectName="Piece"){//put board,piece,and square gameobjects on screen
		piece.gameObject = new GameObject (gameObjectName);
		piece.gameObject.transform.parent = board1.gameObject.transform;
		int width = piece.squares.GetLength (0), height = piece.squares.GetLength (1);
		for (int y=0; y<height; y++)
			for (int x=0; x<width; x++)
			if (piece.squaresOn [x,y]){
				GameObject prefab=prefabs[piece.squares[x,y].prefabNum];
				Vector3 position=new Vector3(x+.5f,-y-.5f,0);//-(width/2f)+x+.5f, (height/2f)-y-.5f for centered
				Quaternion rotation=Quaternion.Euler(0,0,piece.squares[x,y].rotation);
				piece.squares[x,y].gameObject=(GameObject)Instantiate(prefab, position,rotation);
				piece.squares[x,y].gameObject.transform.parent=piece.gameObject.transform;
				piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=piece.color;
				piece.squares[x,y].gameObject.GetComponent<Transform>().localScale=new Vector3(scaleX, scaleY, 1f);
			}
		piece.gameObject.transform.position = VectorMethods.multiplyVectors (piece.position, new Vector2 (1f, -1f));
		board1.movePiece (piece, piece.position, true, checkIntersection);//move piece to its position
		piece.gameObject.transform.localScale=new Vector3(1f,1f,1f);
	}
	void updateAllGameObjects(PieceClasses.GameBoard board1){
		foreach (PieceClasses.Piece piece in board1.pieces) {
			for (int y=0; y<piece.height; y++)
				for (int x=0; x<piece.width; x++)
					if (piece.squaresOn [x,y]){
						GameObject prefab=prefabs[piece.squares[x,y].prefabNum];
						piece.squares[x,y].gameObject.transform.localRotation=Quaternion.Euler(0,0,piece.squares[x,y].rotation);
						piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().sprite=prefab.GetComponent<SpriteRenderer>().sprite;
						//piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=piece.squares[x,y].color;
					}
		}
		if (board1.backgroundPiece!=null)
			for (int y=0; y<board1.backgroundPiece.height; y++)
				for (int x=0; x<board1.backgroundPiece.width; x++)
					if (board1.backgroundPiece.squaresOn [x,y]){
						GameObject prefab=prefabs[board1.backgroundPiece.squares[x,y].prefabNum];
						board1.backgroundPiece.squares[x,y].gameObject.transform.localRotation=Quaternion.Euler(0,0,board1.backgroundPiece.squares[x,y].rotation);
						board1.backgroundPiece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().sprite=prefab.GetComponent<SpriteRenderer>().sprite;
						//board1.backgroundPiece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=board1.backgroundPiece.squares[x,y].color;
					}
	}
	
	void showRandomCompliment(){
		string[] compliments=new string[] {"Awesome!", "Way to go!", "Well Done!", "Great Job!"};
		int i = Random.Range (0, compliments.Length-1);
		complimentText.text = compliments[i];
		complimentTextShadow.text = compliments[i];

		Vector2 BGPos=VectorMethods.addVectors(mainBoard.backgroundPiece.gameObject.transform.localPosition, new Vector2(mainBoard.backgroundPiece.width/2f, -mainBoard.backgroundPiece.height/2f));
		complimentScreen.gameObject.transform.position=Camera.main.WorldToScreenPoint(mainBoard.gameObject.transform.TransformPoint(BGPos));
		complimentScreen.SetActive(true);
	}
	
	string getTime(){
		float timePassed = Time.time;// - startTime;
		int hours=Mathf.FloorToInt(timePassed/60f/60f);
		timePassed -= hours *60f*60f;
		int mins = Mathf.FloorToInt(timePassed/60f);
		timePassed -= mins *60f;
		int secs = Mathf.FloorToInt(timePassed);
		timePassed -= secs;
		int centiSecs=Mathf.FloorToInt(timePassed*100f);
		string text;
		if (hours>0)
			text = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", hours, mins, secs, centiSecs);
		else if (mins>0)
			text = string.Format("{0:00}:{1:00}.{2:00}", mins, secs, centiSecs);
		else if (secs>0)
			text = string.Format("{0:00}.{1:00}", secs, centiSecs);
		else
			text = string.Format(".{0:00}", centiSecs);
		return text;
	}

	bool updateGame(){
		if (animating)
			animating=mainAnimation.animate();
		if (Input.touchCount > 0) {
			Touch touch=Input.GetTouch(0);
			Vector2 position= Camera.main.ScreenToWorldPoint(touch.position);
			position=mainBoard.gameObject.transform.InverseTransformPoint(position);
			
			switch (Input.GetTouch(0).phase){
				case TouchPhase.Began:
					lastPosition=position;
					for (int i=0;i<mainBoard.pieces.Length;i++){
						if (mainBoard.pieces[i].intersects(position)){
							heldPieceIndex=i;
							startPositionGrid=mainBoard.pieces[i].gameObject.transform.localPosition;
							startPositionGrid=new Vector2(startPositionGrid.x, -startPositionGrid.y);//negative y to match mainBoard
							mainBoard.pickUpOrDropPiece(mainBoard.pieces[i], true);
							updateAllGameObjects(mainBoard);
						}
					}
					break;
					
				case TouchPhase.Moved:
					if (heldPieceIndex!=-1){//holding a piece
						mainBoard.pieces[heldPieceIndex].gameObject.transform.Translate(VectorMethods.multiplyVectors((position-lastPosition),mainBoard.gameObject.transform.localScale));
						lastPosition=position;
						mainBoard.pieces[heldPieceIndex].setSortingLayer("Held Piece");
					}
					break;
					
				case TouchPhase.Ended:
					if (heldPieceIndex!=-1){//holding a piece
						Vector2 deltaPositionGrid=mainBoard.gameObject.transform.InverseTransformPoint(mainBoard.pieces[heldPieceIndex].gameObject.transform.position); //position of object with respect to the mainBoard
						deltaPositionGrid=new Vector2(deltaPositionGrid.x, -deltaPositionGrid.y);//negative y to match mainBoard
						deltaPositionGrid=VectorMethods.subtractVectors(deltaPositionGrid,startPositionGrid); //subtract initial position
						deltaPositionGrid=new Vector2(Mathf.RoundToInt(deltaPositionGrid.x), Mathf.RoundToInt(deltaPositionGrid.y)); //nearest block
						Vector2 endPos=mainBoard.movePiece(mainBoard.pieces[heldPieceIndex], VectorMethods.addVectors(startPositionGrid,deltaPositionGrid), false);
						animate(mainBoard.pieces[heldPieceIndex], endPos, 20f);
						mainBoard.pickUpOrDropPiece(mainBoard.pieces[heldPieceIndex], false);
						updateAllGameObjects(mainBoard);
						if (mainBoard.isSolved()){
							mainBoard.pieces[heldPieceIndex].gameObject.transform.localPosition=endPos;
							return false;
						}
						heldPieceIndex=-1;
					}
					break;
			}
		}
		return true;
	}

	void Update (){
		mainScheduler.update ();
	}
}