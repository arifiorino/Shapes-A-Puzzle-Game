using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {
	public void changeScene(string scene){
		Debug.Log ("Changed to scene:" + scene);
		SceneManager.LoadScene (scene);
	}
}
