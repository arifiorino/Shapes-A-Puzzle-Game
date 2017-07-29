using UnityEngine;
using System.Collections;

public class PackPresets:MonoBehaviour{
	public static Pack[] packs;
	public static Pack currentPack;
	public static Color[] colorScheme;
	void Start(){
		colorScheme = new Color[4];
		//[0,160,176],[57,121,37],[204,51,63],[235,104,65]
		colorScheme[0]=new Color(0f/255f,160f/255f,176f/255f);
		colorScheme[1]=new Color(57f/255f,121f/255f,37f/255f);
		colorScheme[2]=new Color(204f/255f,51f/255f,63f/255f);
		colorScheme[3]=new Color(235f/255f,104f/255f,65f/255f);

		PlayerPrefs.DeleteAll ();
		packs = new Pack[4];
		packs [0] = new Pack (50, "Basic 1");
		packs [1] = new Pack (50, "Basic 2");
		packs [2] = new Pack (50, "Basic 3");
		packs [3] = new Pack (50, "Basic 4");

		for (int i = 0;i<packs.Length;i++) {
			packs[i].packI=i;
			packs[i].currentLevel=PlayerPrefs.GetInt("Pack"+i+" Current Level",-1);
			packs[i].makeLevels(PlayerPrefs.GetInt("Pack"+i+" Last Solved Level",-1));
		}

		currentPack = packs [0];
	}
}
