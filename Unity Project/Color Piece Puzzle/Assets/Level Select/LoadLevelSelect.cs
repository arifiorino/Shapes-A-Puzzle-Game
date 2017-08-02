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

		Debug.Log ("Loading level select");

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
			level.button.GetComponentInChildren<Text>().text=""+(level.index+1);

			level.previewPiece=importPiece(level.gameDict["backgroundPiece"]);
			createPreviewPieceGameObjects (level.previewPiece, level.button.transform.GetChild(1).gameObject);

			Vector2 pieceRectSize = level.previewPiece.gameObject.GetComponent<RectTransform> ().rect.size;
			Vector2 squareSize = boundsScreenSize (level.previewPiece.firstSquare().gameObject.GetComponent<Renderer> ().bounds, Camera.main);

			float scaleByWidth = pieceRectSize.x / (level.previewPiece.width * squareSize.x)*.8f; //The should take up 70% of piece
			float scaleByHeight = pieceRectSize.y / (level.previewPiece.height * squareSize.y)*.8f;
			level.previewPiece.gameObject.transform.localScale = new Vector2 (Mathf.Min(scaleByWidth,scaleByHeight), Mathf.Min(scaleByWidth,scaleByHeight));

			if (level.solved) {
				this.colorPreviewPiece (level.previewPiece, level.solution);
			}


			int tempLevelI=levelI;
			level.button.GetComponent<Button>().onClick.AddListener(delegate() {
				PackPresets.currentPack.currentLevel=PackPresets.currentPack.levels[tempLevelI];
				SceneManager.LoadScene("Game/Game");

				PlayerPrefs.SetInt ("pack" + PackPresets.currentPack.index + "CurrentLevel", tempLevelI);
				PlayerPrefs.Save ();
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

	void createPreviewPieceGameObjects(PieceClasses.Piece piece, GameObject gameObject){
		piece.gameObject = gameObject;
		for (int y=0; y<piece.height; y++)
			for (int x=0; x<piece.width; x++)
				if (piece.squaresOn [x,y]){
					GameObject prefab=prefabs[piece.squares[x,y].prefabNum];
					Vector3 squarePosition=new Vector3(-(piece.width/2f)+x+.5f, (piece.height/2f)-y-.5f);
					Quaternion rotation=Quaternion.Euler(0,0,piece.squares[x,y].rotation);
					piece.squares[x,y].gameObject=(GameObject)Instantiate(prefab);
					piece.squares[x,y].gameObject.transform.parent=piece.gameObject.transform;
					piece.squares [x, y].gameObject.transform.localPosition = squarePosition;
					piece.squares [x, y].gameObject.transform.localRotation = rotation;
					piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=piece.color;
					piece.squares[x,y].gameObject.GetComponent<Transform>().localScale=new Vector3(1f, 1f, 1f);
				}
		piece.setSortingLayer ("Pieces");
	}

	void colorPreviewPiece(PieceClasses.Piece piece, int[]solution){
		int i = 0;
		for (int y = 0; y < piece.height; y++) {
			for (int x = 0; x < piece.width; x++) {
				if (piece.squaresOn [x, y]) {
					piece.squares [x, y].gameObject.GetComponent<SpriteRenderer> ().color = PackPresets.colorScheme [solution [i]];
					i++;
				}
			}
		}
	}

	public static Vector2 boundsScreenSize(Bounds bounds, Camera camera)
	{
		var origin = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, 0.0f));
		var extents = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, 0.0f));

		return new Vector2(extents.x - origin.x, extents.y - origin.y);
	}

}