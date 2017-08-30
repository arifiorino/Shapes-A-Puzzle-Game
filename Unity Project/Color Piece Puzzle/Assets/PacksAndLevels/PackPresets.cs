using UnityEngine;
using System.Collections;

public class PackPresets:MonoBehaviour{
	public static Pack[] packs;
	public static Pack currentPack;
	public static Color[] colorScheme;
	public static string lastScene;

	void Start(){

		colorScheme = new Color[11]; //blue, red, brown, green, yellow, orange, dark blue, greyish, brownish, light blue, background grey

		colorScheme[0]=new Color(0f/255f,160f/255f,176f/255f);
		colorScheme[1]=new Color(204f/255f,51f/255f,63f/255f);
		colorScheme[2]=new Color(106f/255f,74f/255f,60f/255f);
		colorScheme[3]=new Color(57f/255f,121f/255f,37f/255f);
		colorScheme[4]=new Color(237f/255f,201f/255f,81f/255f);
		colorScheme[5]=new Color(235f/255f,104f/255f,65f/255f);
		colorScheme[6]=new Color(11f/255f,39f/255f,75f/255f);
		colorScheme[7]=new Color(125f/255f,118f/255f,98f/255f);
		colorScheme[8]=new Color(157f/255f,107f/255f,53f/255f);
		colorScheme[9]=new Color(107f/255f,174f/255f,200f/255f);
		colorScheme[10]=new Color(220f/255f,220f/255f,220f/255f);

	}

	public static void loadPacks(){
		Debug.Log ("Loading");

		packs = new Pack[5];
		packs [0] = new Pack (0, 50, "Basic 1", colorScheme [0]);
		packs [1] = new Pack (1, 50, "Basic 2", colorScheme [1]);
		packs [2] = new Pack (2, 50, "Basic 3", colorScheme [3]);
		packs [3] = new Pack (3, 50, "Basic 4", colorScheme [5]);
		packs [4] = new Pack (4, 50, "Basic 5", colorScheme [4]);

		for (int i = 0; i < packs.Length; i++) {
			packs [i].currentLevel = packs [i].levels [PlayerPrefs.GetInt ("pack" + i + "CurrentLevel", 0)];
		}

		currentPack = packs [0];
	}
}
