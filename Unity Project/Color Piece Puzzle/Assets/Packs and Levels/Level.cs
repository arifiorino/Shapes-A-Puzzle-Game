using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class Level{
	public int packIndex;
	public bool solved;
	public int index;
	public GameObject button;
	public PieceClasses.Piece previewPiece;
	public JSONNode gameDict;
	public int[] solution; //array of colors of solution

	public Level(int packIndex, int index, bool solved){
		this.packIndex = packIndex;
		this.index = index;
		this.solved = solved;

		TextAsset reader = (TextAsset)Resources.Load ("Pack"+(this.packIndex+1)+"Level"+(this.index+1));
		this.gameDict = JSON.Parse (reader.text);

		if (this.solved) {
			string solutionString = PlayerPrefs.GetString ("pack"+(this.packIndex+1)+"Level"+(this.index+1)+"Solution");
			Debug.Log ("Solution: "+solutionString);
			JSONArray solutionArray = JSON.Parse (solutionString).AsArray;
			this.solution = new int[solutionArray.Count];
			for (int i=0;i<solutionArray.Count;i++) {
				this.solution [i] = solutionArray [i].AsInt;
			}
		}
	}
}
