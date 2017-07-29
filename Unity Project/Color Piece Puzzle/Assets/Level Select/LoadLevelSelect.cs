using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class LoadLevelSelect : MonoBehaviour {
	public Text packTitle;
	public GameObject content;
	public GameObject levelButtonPrefab;
	public GameObject[] prefabs;

	int numRows = 5;
	int numColumns = 5; //multiplies to numLevelsInPack
	int borderToScreen=10;
	int borderBetweenPages = 20;
	int borderBetweenLevels = 5;

	void Start () {
		packTitle.text = PackPresets.currentPack.name;

		float width = (content.GetComponent<RectTransform> ().rect.width-borderToScreen*2+borderBetweenLevels)/numColumns-borderBetweenLevels;
		float height = (content.GetComponent<RectTransform> ().rect.height-borderToScreen*2+borderBetweenLevels)/numRows-borderBetweenLevels;

		float x = borderToScreen;
		float y = -borderToScreen;
		float pageX = x;
		int levelColumn = 0;
		int levelRow = 0;
		int levelI = 0;
		foreach (Level level in PackPresets.currentPack.levels) {
			level.button=(GameObject)Instantiate(levelButtonPrefab, new Vector3(x,y,0), Quaternion.identity);
			level.button.transform.SetParent(content.transform, false);
			level.button.GetComponent<RectTransform>().anchoredPosition=new Vector2(x,y);
			level.button.GetComponent<RectTransform>().sizeDelta=new Vector2(width, height);
			level.button.GetComponentInChildren<Text>().text=""+level.difficulty;

//			level.gameBoard = importGameBoardFromFile ("Pack"+(PackPresets.currentPack.packI+1)+"Level"+(levelI+1), level.button.GetComponent<RectTransform>().rect);
//			level.gameBoard.gameObject.transform.parent = level.button.transform;
//			level.gameBoard.gameObject.SetActive (false);

			TextAsset reader = (TextAsset)Resources.Load ("Pack"+(PackPresets.currentPack.packI+1)+"Level"+(levelI+1));
			string jsonString = reader.text;
			JSONNode gameDict = JSON.Parse (jsonString);
			level.backgroundPiece=importPiece(gameDict["backgroundPiece"]);
			createPieceGameObjects (level.backgroundPiece, new Vector2 (0, 0));
			level.backgroundPiece.gameObject.transform.parent = level.button.transform;

			if (!level.solved)
				level.button.GetComponentInChildren<Text>().fontStyle=FontStyle.Italic;
			//level.button.GetComponent<Button>().interactable=level.unlocked;
			int tempLevelI=levelI;
			level.button.GetComponent<Button>().onClick.AddListener(delegate() {
				PackPresets.currentPack.currentLevel=tempLevelI;
				SceneManager.LoadScene("Game/Game");
			});

			if (levelI<PackPresets.currentPack.levels.Length-1){ //not last level because that would be useless
				x+=width+borderBetweenLevels;
				levelColumn++;
				if (levelColumn>=numColumns){//next row
					x=pageX;
					y-=height+borderBetweenLevels;
					levelColumn=0;
					levelRow++;
				}
				if (levelRow>=numRows){//next page
					pageX+=(width*numRows)+(borderBetweenLevels*(numRows-1))+borderBetweenPages;
					x=pageX;
					y = -borderToScreen;
					levelColumn=0;
					levelRow=0;
				}
				levelI++;
			}

		}
		content.GetComponent<RectTransform> ().sizeDelta=new Vector2(pageX+(width*numRows)+(borderBetweenLevels*(numRows-1))+borderToScreen,0);
	}

	PieceClasses.GameBoard importGameBoardFromFile(string filename, Rect buttonRect){
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

		createBoardGameObjects (board, buttonRect);

		return board;
	}

	void createBoardGameObjects(PieceClasses.GameBoard board, Rect viewRect){
		viewRect.position = new Vector2 (0f, 0f);
		board.gameObject = new GameObject ("Board");//create board
		foreach (PieceClasses.Piece piece in board.pieces) {//create pieces
			createPieceGameObjects (piece, board);
		}
		createPieceGameObjects (board.backgroundPiece, board, 1.2f, 1.2f, false, "Background Piece");//create background piece
//		board.backgroundPiece.setSortingLayer("Background Piece");
//		topHeight=(50f/(float)Camera.main.pixelHeight)*cameraHeight;
//		Rect viewRect = new Rect (0, -topHeight/2f, cameraWidth, cameraHeight-topHeight);
		float boardRatio = board.size.x / board.size.y;
		Debug.Log(viewRect.height+", "+boardRatio+", "+board.size.x);
		if (boardRatio < viewRect.width / viewRect.height) {
			board.gameObject.transform.localScale= new Vector3 (viewRect.height*boardRatio/board.size.x, viewRect.height/board.size.y, 1f); //scale to height
			board.gameObject.transform.localPosition = new Vector2 (-viewRect.height*boardRatio/2+viewRect.x, viewRect.height/2+viewRect.y);
		} else {
			board.gameObject.transform.localScale= new Vector3 (viewRect.width/board.size.x, viewRect.width/boardRatio/board.size.y, 1f); //scale to width
			board.gameObject.transform.localPosition = new Vector2 (-viewRect.width/2+viewRect.x, viewRect.width/boardRatio/2+viewRect.y);
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

	void createPieceGameObjects(PieceClasses.Piece piece, Vector2 position, float scaleX=1, float scaleY=1, string gameObjectName="Piece"){
		piece.gameObject = new GameObject (gameObjectName);
		int width = piece.squares.GetLength (0), height = piece.squares.GetLength (1);
		for (int y=0; y<height; y++)
			for (int x=0; x<width; x++)
				if (piece.squaresOn [x,y]){
					GameObject prefab=prefabs[piece.squares[x,y].prefabNum];
					Vector3 squarePosition=new Vector3(x+.5f,-y-.5f,0);//-(width/2f)+x+.5f, (height/2f)-y-.5f for centered
					Quaternion rotation=Quaternion.Euler(0,0,piece.squares[x,y].rotation);
					piece.squares[x,y].gameObject=(GameObject)Instantiate(prefab, squarePosition,rotation);
					piece.squares[x,y].gameObject.transform.parent=piece.gameObject.transform;
					piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=piece.color;
					piece.squares[x,y].gameObject.GetComponent<Transform>().localScale=new Vector3(scaleX, scaleY, 1f);
				}
		piece.gameObject.transform.position = position;
		piece.gameObject.transform.localScale=new Vector3(1f,1f,1f);
	}

}