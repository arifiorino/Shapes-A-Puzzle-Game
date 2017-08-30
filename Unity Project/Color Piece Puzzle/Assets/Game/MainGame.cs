using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;
using System.IO;

public class MainGame : MonoBehaviour
{
	public Text levelTitle;
	public GameObject[] prefabs;
	public GameObject boardContainer;
	public Canvas canvas;
	public GameObject backButton;
	public GameObject settingsButton;

	public GameObject nextLevelButton;
	public GameObject lastLevelButton;
	public GameObject undoButton;
	public GameObject gameMusicPlayer;

	public GameObject tutorialText;

	PieceClasses.GameBoard mainBoard;
	PieceClasses.GameBoard oldBoard;
	int heldPieceIndex = -1;
	Vector2 startPositionGrid;
	Vector2 lastPosition;
	bool animating = false, animatingLastPiece=false;
	Animation mainAnimation;
	Scheduler mainScheduler;
	List<MoveClass.Move> moves;

	void Start (){
		gameMusicPlayer.GetComponent<AudioSource> ().mute = MusicPlayer.getMute ();

		MusicPlayer.pause ();

		backButton.GetComponent<Button>().onClick.AddListener(delegate() {
			MusicPlayer.play();
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
		});

		settingsButton.GetComponent<Button>().onClick.AddListener(delegate() {
			MusicPlayer.play();
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
		});

		nextLevelButton.GetComponent<Button>().onClick.AddListener(delegate() {
			PackPresets.currentPack.currentLevel=PackPresets.currentPack.levels[PackPresets.currentPack.currentLevel.index+1];
			PlayerPrefs.SetInt ("pack" + PackPresets.currentPack.index + "CurrentLevel", PackPresets.currentPack.currentLevel.index);
			PlayerPrefs.Save ();
			levelTitle.text = "Level "+(PackPresets.currentPack.currentLevel.index + 1);
			setMenuButtonsDisabled ();
			Destroy(mainBoard.gameObject);
			mainBoard = importPiecesFromGameDict (PackPresets.currentPack.currentLevel.gameDict);
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			moves.Clear();
		});

		lastLevelButton.GetComponent<Button>().onClick.AddListener(delegate() {
			PackPresets.currentPack.currentLevel=PackPresets.currentPack.levels[PackPresets.currentPack.currentLevel.index-1];
			PlayerPrefs.SetInt ("pack" + PackPresets.currentPack.index + "CurrentLevel", PackPresets.currentPack.currentLevel.index);
			PlayerPrefs.Save ();
			levelTitle.text = "Level "+(PackPresets.currentPack.currentLevel.index + 1);
			setMenuButtonsDisabled ();
			Destroy(mainBoard.gameObject);
			mainBoard = importPiecesFromGameDict (PackPresets.currentPack.currentLevel.gameDict);
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			moves.Clear();
		});

		undoButton.GetComponent<Button> ().onClick.AddListener (delegate() {
			if (!animating){
				MoveClass.Move move = moves[moves.Count-1];
				Debug.Log("Start: "+move.startPosition);
				Vector2 endPos=mainBoard.movePiece(move.piece, move.startPosition, false);
				animate (move.piece, endPos, 60f);
				bool pieceSnapped = mainBoard.pickUpOrDropPiece (move.piece, false);
				if (pieceSnapped) {
					SoundEffectPlayer.playSound (SoundEffectPlayer.snapSound);
				}
				updateAllGameObjects (mainBoard);
				moves.RemoveAt(moves.Count-1);
				setMenuButtonsDisabled();
			}
		});

		moves = new List<MoveClass.Move> ();
		mainScheduler = new Scheduler ();
		levelTitle.text = "Level "+(PackPresets.currentPack.currentLevel.index + 1);
		setMenuButtonsDisabled ();
		mainBoard = importPiecesFromGameDict (PackPresets.currentPack.currentLevel.gameDict);

		if (!LoadStart.haveSolvedALevel) {
			tutorialText.SetActive (true);
		}

		mainLevelLoop ();
	}

	public void setMenuButtonsDisabled(){
		lastLevelButton.GetComponent<Button> ().interactable = PackPresets.currentPack.currentLevel.index != 0;
		nextLevelButton.GetComponent<Button> ().interactable = PackPresets.currentPack.currentLevel.index != PackPresets.currentPack.levels.Length-1;
		undoButton.GetComponent<Button> ().interactable = moves.Count > 0;
	}

	public void animate (PieceClasses.Piece piece, Vector2 endPosition, float speed)
	{
		mainAnimation = new Animation (piece, endPosition, new Vector2 (1f, 1f), true, speed);
		animating = true;
	}

	void mainLevelLoop ()
	{

		mainScheduler.addUpdateFunction (updateGame);


		mainScheduler.addInitFunction (delegate() {
			//ScreenCapture.CaptureScreenshot("pack" + PackPresets.currentPack.index + "Level"+ PackPresets.currentPack.currentLevel.index+".png", 4);
			if (!LoadStart.haveSolvedALevel){
				LoadStart.haveSolvedALevel=true;
				PlayerPrefs.SetInt ("haveSolvedALevel", 1);
				PlayerPrefs.Save ();
			}
			this.heldPieceIndex = -1;
			this.saveLevelSolution ();
			PackPresets.currentPack.solveLevel ();
			animating = false;
			moves.Clear();
		});

		mainScheduler.addTime (1f);

		mainScheduler.addInitFunction (delegate() {
			tutorialText.SetActive (false);
		});

		mainScheduler.addInitFunction (setupTransition); //also imports new board
		mainScheduler.addUpdateFunction (animatePieces);

		mainScheduler.addInitFunction (delegate() {
			levelTitle.text = "Level "+(PackPresets.currentPack.currentLevel.index + 1);
			setMenuButtonsDisabled();
			Destroy (oldBoard.gameObject);
			mainBoard.backgroundPiece.gameObject.SetActive (true);
		});

		mainScheduler.addInitFunction (mainLevelLoop);

	}

	bool animatePieces ()
	{
		bool done = true;
		foreach (PieceClasses.Piece piece in mainBoard.pieces)
			if (piece.isAnimating) {
				done = false;
				piece.isAnimating = piece.animation.animate ();
			}

		return !done;
	}

	void setupTransition ()
	{
		float ANIMATION_TIME = 1f;
		oldBoard = mainBoard;
		mainBoard=importPiecesFromGameDict (PackPresets.currentPack.currentLevel.gameDict);
		Vector2[] transitionEndPositions = new Vector2[mainBoard.pieces.Length];
		for (int i=0; i<mainBoard.pieces.Length; i++) {
			transitionEndPositions [i] = mainBoard.pieces [i].gameObject.transform.localPosition;
			for (int y = 0; y < mainBoard.pieces [i].height; y++) {//set all pieces to transition to white
				for (int x = 0; x < mainBoard.pieces [i].width; x++) {
					if (mainBoard.pieces [i].squaresOn [x, y]) {
						mainBoard.pieces [i].squares [x, y].gameObject.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 0);
					}
				}
			}
			mainBoard.pieces[i].isAnimating=true;
			mainBoard.pieces[i].animation=new Animation(mainBoard.pieces[i], transitionEndPositions [i], new Vector2(1f,1f), false, ANIMATION_TIME);
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

	void saveLevelSolution ()
	{
		JSONArray solution = new JSONArray ();
		PackPresets.currentPack.currentLevel.solution = new int[mainBoard.numSquaresOn];

		int solutionI = 0;
		for (int y = 0; y < mainBoard.backgroundPiece.height; y++) {
			for (int x = 0; x < mainBoard.backgroundPiece.width; x++) {
				if (mainBoard.backgroundPiece.squaresOn [x, y]) {
					
					Color squareColor = mainBoard.squares [(int)mainBoard.backgroundPiece.position.x + x, (int)mainBoard.backgroundPiece.position.y + y].color;
					bool changed = false;
					for (int colorI = 0; colorI < PackPresets.colorScheme.Length; colorI++) {
						if (PackPresets.colorScheme [colorI].Equals (squareColor)) {
							changed = true;
							solution.Add (new JSONData (colorI));
							PackPresets.currentPack.currentLevel.solution [solutionI] = colorI;
						}
					}
					if (!changed) {
						Debug.Log ("Color not found!!!");
					}
					solutionI++;
				}
			}
		}
		Debug.Log ("Level solution: " + solution.ToString ());
		PlayerPrefs.SetString ("pack" + (PackPresets.currentPack.index + 1) + "Level" + (PackPresets.currentPack.currentLevel.index + 1) + "Solution", solution.ToString ());
		PlayerPrefs.Save ();
	}

	PieceClasses.GameBoard importPiecesFromGameDict (JSONNode gameDict)
	{
		PieceClasses.GameBoard board = new PieceClasses.GameBoard (new Vector2 (gameDict ["boardSize"] [0].AsFloat, gameDict ["boardSize"] [1].AsFloat));//new board

		JSONArray piecesArray = gameDict ["pieces"].AsArray; //Create pieces
		board.pieces = new PieceClasses.Piece[piecesArray.Count];

		for (int i = 0; i < piecesArray.Count; i++) {
			board.pieces [i] = importPiece (piecesArray [i]);
		}

		board.backgroundPiece = importPiece (gameDict ["backgroundPiece"]);//Create Background piece
		board.backgroundPiece.position = new Vector2 ((int)(board.size.x / 2 - board.backgroundPiece.width / 2), (int)(board.size.y / 2 - board.backgroundPiece.height / 2));

		createBoardGameObjects (board);

		return board;
	}

	public static Vector2 boundsScreenSize(Bounds bounds)
	{
		var origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, 0.0f));
		var extents = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, 0.0f));

		return new Vector2(extents.x - origin.x, extents.y - origin.y);
	}

	void createBoardGameObjects (PieceClasses.GameBoard board)
	{
		board.gameObject = new GameObject ("Board");//create board
		board.gameObject.transform.SetParent(boardContainer.transform);
		board.gameObject.transform.localScale = new Vector3 (1, 1, 1);
		board.gameObject.transform.localPosition = new Vector2 (0,0);

		foreach (PieceClasses.Piece piece in board.pieces) {//create pieces
			createPieceGameObjects (piece, board);
		}
		createPieceGameObjects (board.backgroundPiece, board, 1.2f, 1.2f, false, "Background Piece");//create background piece
		board.backgroundPiece.setSortingLayer ("Background Piece");


		Vector2 boardContainerSize = boardContainer.GetComponent<RectTransform> ().rect.size;
		Vector2 squareSize = boundsScreenSize (board.pieces[0].firstSquare().gameObject.GetComponent<Renderer> ().bounds);

		float scaleByWidth = boardContainerSize.x / (board.size.x * squareSize.x);
		float scaleByHeight = boardContainerSize.y / (board.size.y * squareSize.y);

		if (scaleByWidth<scaleByHeight) { //If scaling by width is smaller, then do that
			board.gameObject.transform.localScale = new Vector3 (scaleByWidth*canvas.scaleFactor, scaleByWidth*canvas.scaleFactor, 1f); //scale to height, idk what canvas scalefactor is but whatever it works
			board.gameObject.transform.localPosition = new Vector2 (0,-(boardContainerSize.y-(board.size.y*squareSize.y*scaleByWidth))/2);
		} else {
			board.gameObject.transform.localScale = new Vector3 (scaleByHeight*canvas.scaleFactor, scaleByHeight*canvas.scaleFactor, 1f); //scale to width
			board.gameObject.transform.localPosition = new Vector2 ((boardContainerSize.x-(board.size.x*squareSize.x*scaleByHeight))/2,0);
		}
	}

	PieceClasses.Piece importPiece (JSONNode pieceDict)
	{ //make piece object from jsondict of a piece
		bool[,] squaresOn = new bool[pieceDict ["size"] [0].AsInt, pieceDict ["size"] [1].AsInt];
		int i = 0;
		for (int y = 0; y < pieceDict ["size"] [1].AsInt; y++)
			for (int x = 0; x < pieceDict ["size"] [0].AsInt; x++) {
				squaresOn [x, y] = pieceDict ["squaresOn"] [i].AsInt == 1;
				i++;
			}
		Color color = PackPresets.colorScheme[pieceDict ["color"].AsInt];
		return new PieceClasses.Piece (squaresOn, color, new Vector2 (pieceDict ["startPos"] [0].AsFloat, pieceDict ["startPos"] [1].AsFloat));
	}

	void createPieceGameObjects (PieceClasses.Piece piece, PieceClasses.GameBoard board1, float scaleX = 1, float scaleY = 1, bool checkIntersection = true, string gameObjectName = "Piece")
	{//put board,piece,and square gameobjects on screen
		piece.gameObject = new GameObject (gameObjectName);
		piece.gameObject.transform.parent = board1.gameObject.transform;
		int width = piece.squares.GetLength (0), height = piece.squares.GetLength (1);
		for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				if (piece.squaresOn [x, y]) {
					GameObject prefab = prefabs [piece.squares [x, y].prefabNum];
					Vector3 position = new Vector3 (x + .5f, -y - .5f, 0);//-(width/2f)+x+.5f, (height/2f)-y-.5f for centered
					Quaternion rotation = Quaternion.Euler (0, 0, piece.squares [x, y].rotation);
					piece.squares [x, y].gameObject = (GameObject)Instantiate (prefab, position, rotation);
					piece.squares [x, y].gameObject.transform.parent = piece.gameObject.transform;
					piece.squares [x, y].gameObject.GetComponent<SpriteRenderer> ().color = piece.color;
					piece.squares [x, y].gameObject.GetComponent<Transform> ().localScale = new Vector3 (scaleX, scaleY, 1f);
				}
		piece.gameObject.transform.position = VectorMethods.multiplyVectors (piece.position, new Vector2 (1f, -1f));
		board1.movePiece (piece, piece.position, true, checkIntersection);//move piece to its position
		piece.gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
		piece.setSortingLayer ("Pieces");
	}

	void updateAllGameObjects (PieceClasses.GameBoard board1)
	{
		foreach (PieceClasses.Piece piece in board1.pieces) {
			for (int y = 0; y < piece.height; y++)
				for (int x = 0; x < piece.width; x++)
					if (piece.squaresOn [x, y]) {
						GameObject prefab = prefabs [piece.squares [x, y].prefabNum];
						piece.squares [x, y].gameObject.transform.localRotation = Quaternion.Euler (0, 0, piece.squares [x, y].rotation);
						piece.squares [x, y].gameObject.GetComponent<SpriteRenderer> ().sprite = prefab.GetComponent<SpriteRenderer> ().sprite;
						//piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=piece.squares[x,y].color;
					}
		}
		if (board1.backgroundPiece != null)
			for (int y = 0; y < board1.backgroundPiece.height; y++)
				for (int x = 0; x < board1.backgroundPiece.width; x++)
					if (board1.backgroundPiece.squaresOn [x, y]) {
						GameObject prefab = prefabs [board1.backgroundPiece.squares [x, y].prefabNum];
						board1.backgroundPiece.squares [x, y].gameObject.transform.localRotation = Quaternion.Euler (0, 0, board1.backgroundPiece.squares [x, y].rotation);
						board1.backgroundPiece.squares [x, y].gameObject.GetComponent<SpriteRenderer> ().sprite = prefab.GetComponent<SpriteRenderer> ().sprite;
						//board1.backgroundPiece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=board1.backgroundPiece.squares[x,y].color;
					}
	}

	bool updateGame ()
	{
		if (animating)
			animating = mainAnimation.animate ();
		else if (animatingLastPiece) {
			animatingLastPiece = false;
			SoundEffectPlayer.playSound (SoundEffectPlayer.winSound,.5f);
			return false;
		}

		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch (0);

			Vector2 position = Camera.main.ScreenToWorldPoint (touch.position);

			position = mainBoard.gameObject.transform.InverseTransformPoint (position);

			switch (Input.GetTouch (0).phase) {
			case TouchPhase.Began:
				lastPosition = position;
				for (int i = 0; i < mainBoard.pieces.Length; i++) {
					if (mainBoard.pieces [i].intersects (position)) {
						heldPieceIndex = i;
						startPositionGrid = mainBoard.pieces [i].gameObject.transform.localPosition;
						startPositionGrid = new Vector2 (startPositionGrid.x, -startPositionGrid.y);//negative y to match mainBoard
						mainBoard.pickUpOrDropPiece (mainBoard.pieces [i], true);
						updateAllGameObjects (mainBoard);
					}
				}
				break;
					
			case TouchPhase.Moved:
				if (heldPieceIndex != -1) {//holding a piece
					Vector3 p=mainBoard.pieces [heldPieceIndex].gameObject.transform.localPosition;
					mainBoard.pieces [heldPieceIndex].gameObject.transform.localPosition=(new Vector2 (p.x, p.y))+(position - lastPosition);///canvas.scaleFactor
					lastPosition = position;
					mainBoard.pieces [heldPieceIndex].setSortingLayer ("Held Piece");
				}
				break;
					
			case TouchPhase.Ended:
				if (heldPieceIndex != -1) {//holding a piece
					Vector2 deltaPositionGrid = mainBoard.gameObject.transform.InverseTransformPoint (mainBoard.pieces [heldPieceIndex].gameObject.transform.position); //position of object with respect to the mainBoard
					deltaPositionGrid = new Vector2 (deltaPositionGrid.x, -deltaPositionGrid.y);//negative y to match mainBoard
					deltaPositionGrid = VectorMethods.subtractVectors (deltaPositionGrid, startPositionGrid); //subtract initial position
					deltaPositionGrid = new Vector2 (Mathf.RoundToInt (deltaPositionGrid.x), Mathf.RoundToInt (deltaPositionGrid.y)); //nearest block

					Vector2 startPos = mainBoard.pieces [heldPieceIndex].position;
					Vector2 endPos = mainBoard.movePiece(mainBoard.pieces[heldPieceIndex], VectorMethods.addVectors(startPositionGrid,deltaPositionGrid), false);
					if (startPos!=endPos){
						moves.Add(new MoveClass.Move(mainBoard.pieces [heldPieceIndex], startPos, endPos)); //The y in endPos is gonna be negative. Deal with it.
						setMenuButtonsDisabled();
					}

					animate (mainBoard.pieces [heldPieceIndex], endPos, 20f);
					bool pieceSnapped = mainBoard.pickUpOrDropPiece (mainBoard.pieces [heldPieceIndex], false);
					if (pieceSnapped) {
						SoundEffectPlayer.playSound (SoundEffectPlayer.snapSound);
					}
					updateAllGameObjects (mainBoard);
					if (mainBoard.isSolved ()) {
//						mainBoard.pieces [heldPieceIndex].gameObject.transform.localPosition = endPos;
						animatingLastPiece = true;
//						return false;
					}
					heldPieceIndex = -1;
				}
				break;
			}
		}
		return true;
	}

	void Update ()
	{
		mainScheduler.update ();
	}
}