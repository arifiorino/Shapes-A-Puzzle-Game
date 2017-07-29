using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour {

	void Start () {
		StartCoroutine(LoadNewScene());
	}

	IEnumerator LoadNewScene() {

		yield return new WaitForSeconds(3);

		AsyncOperation async = SceneManager.LoadSceneAsync("Loading/Animation");

		while (!async.isDone) {
			yield return null;
		}

	}
}
