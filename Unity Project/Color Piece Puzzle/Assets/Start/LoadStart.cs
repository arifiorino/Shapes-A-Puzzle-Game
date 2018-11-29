using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadStart : MonoBehaviour {
	public Button startButton, resumeButton, packsButton;
	public GameObject startButtonBackground, resumeButtonBackground, packsButtonBackground;
	public static bool firstRun, haveSolvedALevel;

	// Use this for initialization
	void Start () {

//		PlayerPrefs.DeleteAll ();

		Debug.Log ("Starting");

		firstRun = PlayerPrefs.GetInt ("firstRun", 0) == 0;
		PlayerPrefs.SetInt ("firstRun", 1);
		PlayerPrefs.Save ();

		haveSolvedALevel = PlayerPrefs.GetInt ("haveSolvedALevel", 0) == 1;

		MusicPlayer.setMute(PlayerPrefs.GetInt ("musicOn", 1) == 0);
		SoundEffectPlayer.setMute(PlayerPrefs.GetInt ("soundsOn", 1) == 0);

		resumeButtonBackground.SetActive (!firstRun);
		packsButtonBackground.SetActive (!firstRun);
		startButtonBackground.SetActive (firstRun);

		startButton.GetComponent<Button>().onClick.AddListener(delegate() {
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			SceneManager.LoadScene("PacksAndLevels/PackSelect");
		});

		resumeButton.GetComponent<Button>().onClick.AddListener(delegate() {
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			PackPresets.currentPack=PackPresets.packs[PlayerPrefs.GetInt ("currentPack")];
			PackPresets.currentPack.currentLevel=PackPresets.currentPack.levels[PlayerPrefs.GetInt ("pack"+PackPresets.currentPack.index+"CurrentLevel")];
			SceneManager.LoadScene("Game/Game");

		});

		packsButton.GetComponent<Button>().onClick.AddListener(delegate() {
			SoundEffectPlayer.playSound(SoundEffectPlayer.clickSound);
			SceneManager.LoadScene("PacksAndLevels/PackSelect");
		});

		PackPresets.loadPacks ();



	}

}
