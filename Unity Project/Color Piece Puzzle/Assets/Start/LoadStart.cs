using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadStart : MonoBehaviour {
	public Button startButton, resumeButton, packsButton;
	public GameObject startButtonBackground, resumeButtonBackground, packsButtonBackground;
	public static bool firstRun;

	// Use this for initialization
	void Start () {
		firstRun = PlayerPrefs.GetInt ("firstRun", 0) == 0;
		PlayerPrefs.SetInt ("firstRun", 1);

		resumeButtonBackground.SetActive (!firstRun);
		packsButtonBackground.SetActive (!firstRun);
		startButtonBackground.SetActive (firstRun);

//		startButton.GetComponent<Button>().onClick.AddListener(delegate() {
//			
//		});
//
//		resumeButton.GetComponent<Button>().onClick.AddListener(delegate() {
//
//		});
//
//		packsButton.GetComponent<Button>().onClick.AddListener(delegate() {
//
//		});

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
