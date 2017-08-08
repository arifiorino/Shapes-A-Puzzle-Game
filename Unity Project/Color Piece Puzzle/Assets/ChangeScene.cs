using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {
	public void changeScene(string scene){
		SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);

		if (scene.Equals ("Settings/Settings")) //dont need it in other cases, yet
			PackPresets.lastScene = SceneManager.GetActiveScene ().name;

		Debug.Log ("Changed to scene:" + scene);
		SceneManager.LoadScene (scene);
	}
}
