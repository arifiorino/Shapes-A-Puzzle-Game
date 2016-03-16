using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pack{
	public int packI;
	public Level[] levels;
	public int lastSolvedLevel;
	public int currentLevel;
	public string name;
	public GameObject button;
	public Pack(int numLevels, string name){
		levels = new Level[numLevels];
		this.name = name;
	}
	public void makeLevels(int lastSolvedLevel){
		this.lastSolvedLevel = lastSolvedLevel;
		for (int i=0; i<levels.Length; i++)
			levels [i] = new Level (i <= lastSolvedLevel+1, i <= lastSolvedLevel, i+1);
	}
	public void solveLevel(){

		if (currentLevel < levels.Length)
			currentLevel++;
		else
			currentLevel = 0;

		lastSolvedLevel++;
		makeLevels (lastSolvedLevel);

		PlayerPrefs.SetInt ("Pack" + packI + " Current Level", currentLevel);
		PlayerPrefs.SetInt ("Pack" + packI + " Last Solved Level", lastSolvedLevel);
	}
	public string getScore(){
		return lastSolvedLevel+1 + "/" + levels.Length;
	}
}
