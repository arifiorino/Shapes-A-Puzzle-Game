using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {
	public void changeScene(int sceneI){
		Debug.Log ("Changed to scene:" + sceneI);
		Application.LoadLevel (sceneI);
	}
}
