using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSettings : MonoBehaviour {

	public GameObject backButton;
	public GameObject musicToggle;
	public GameObject soundEffectsToggle;

	// Use this for initialization
	void Start () {
		musicToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = PlayerPrefs.GetInt ("musicOn", 1) == 1;
		soundEffectsToggle.GetComponent<UnityEngine.UI.Toggle> ().isOn = PlayerPrefs.GetInt ("soundsOn", 1) == 1;

		musicToggle.GetComponent<UnityEngine.UI.Toggle> ().onValueChanged.AddListener(delegate(bool isOn) {
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			MusicPlayer.setMute(!isOn);

			PlayerPrefs.SetInt ("musicOn", isOn?1:0);
			PlayerPrefs.Save();
			Debug.Log("musicOn: "+PlayerPrefs.GetInt ("musicOn", 1));
		});

		soundEffectsToggle.GetComponent<UnityEngine.UI.Toggle> ().onValueChanged.AddListener(delegate(bool isOn) {
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			SoundEffectPlayer.setMute(!isOn);

			PlayerPrefs.SetInt ("soundsOn", isOn?1:0);
			PlayerPrefs.Save();
			Debug.Log("soundsOn: "+PlayerPrefs.GetInt ("soundsOn", 1));
		});

		backButton.GetComponent<Button>().onClick.AddListener(delegate() {
			Debug.Log("Back");
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			SceneManager.LoadScene(PackPresets.lastScene);
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
