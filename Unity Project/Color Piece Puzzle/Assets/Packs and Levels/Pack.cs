using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pack{
	public int index;
	public Color color;
	public Level[] levels;
	public Level currentLevel;
	public string name;
	public GameObject button;
	public int numSolvedLevels;

	public Pack(int index, int numLevels, string name, Color color){
		levels = new Level[numLevels];
		this.name = name;
		this.color = color;
		this.index = index;
		this.createLevels ();
	}

	public void createLevels(){
		numSolvedLevels = 0;
		for (int levelI=0;levelI<levels.Length;levelI++) {
			bool solved = PlayerPrefs.GetInt ("pack" + (this.index+1) + "Level" + (levelI+1) + "Solved", 0) == 1;

			this.levels [levelI] = new Level (this.index, levelI, solved);
			if (solved)
				numSolvedLevels++;
		}
	}

	public void solveLevel(){

		this.numSolvedLevels++;
		this.currentLevel.solved = true;

		PlayerPrefs.SetInt ("pack" + (this.index+1) + "Level" + (this.currentLevel.index+1) + "Solved", 1);

		this.currentLevel = this.levels [this.currentLevel.index+1];
		PlayerPrefs.SetInt ("pack" + this.index + "CurrentLevel", this.currentLevel.index);
		PlayerPrefs.Save ();
	}

	public string getScore(){
		return this.numSolvedLevels + "/" + this.levels.Length;
	}
}