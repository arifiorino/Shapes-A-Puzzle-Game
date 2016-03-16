using UnityEngine;
using System.Collections;

public class PackPresets:MonoBehaviour{
	public static Pack[] packs;
	public static Pack currentPack;
	void Start(){
		PlayerPrefs.DeleteAll ();
		packs = new Pack[1];
		packs [0] = new Pack (50, "Basic 1");
//		packs [1] = new Pack (100, "Basic 2");
//		packs [2] = new Pack (100, "Basic 3");
//		packs [3] = new Pack (100, "Basic 4");

		for (int i = 0;i<packs.Length;i++) {
			packs[i].packI=i;
			packs[i].currentLevel=PlayerPrefs.GetInt("Pack"+i+" Current Level",-1);
			packs[i].makeLevels(PlayerPrefs.GetInt("Pack"+i+" Last Solved Level",-1));
		}

		currentPack = packs [0];
	}
}
