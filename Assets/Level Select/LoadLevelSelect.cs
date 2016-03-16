using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadLevelSelect : MonoBehaviour {
	public Text packTitle;
	public GameObject content;
	public GameObject levelButtonPrefab;

	int numRows = 5;
	int numColumns = 5; //multiplies to numLevelsInPack
	int borderToScreen=10;
	int borderBetweenPages = 20;
	int borderBetweenLevels = 5;

	void Start () {
		packTitle.text = PackPresets.currentPack.name;

		float width = (content.GetComponent<RectTransform> ().rect.width-borderToScreen*2+borderBetweenLevels)/numColumns-borderBetweenLevels;
		float height = (content.GetComponent<RectTransform> ().rect.height-borderToScreen*2+borderBetweenLevels)/numRows-borderBetweenLevels;

		float x = borderToScreen;
		float y = -borderToScreen;
		float pageX = x;
		int levelColumn = 0;
		int levelRow = 0;
		int levelI = 0;
		foreach (Level level in PackPresets.currentPack.levels) {
			level.button=(GameObject)Instantiate(levelButtonPrefab, new Vector3(x,y,0), Quaternion.identity);
			level.button.transform.SetParent(content.transform, false);
			level.button.GetComponent<RectTransform>().anchoredPosition=new Vector2(x,y);
			level.button.GetComponent<RectTransform>().sizeDelta=new Vector2(width, height);
			level.button.GetComponentInChildren<Text>().text=""+level.difficulty;
			if (!level.solved)
				level.button.GetComponentInChildren<Text>().fontStyle=FontStyle.BoldAndItalic;
//			level.button.GetComponent<Button>().interactable=level.unlocked;
			int tempLevelI=levelI;
			level.button.GetComponent<Button>().onClick.AddListener(delegate() {
				PackPresets.currentPack.currentLevel=tempLevelI;
				Application.LoadLevel (3); //scene of levels
			});

			if (levelI<PackPresets.currentPack.levels.Length-1){ //not last level because that would be useless
				x+=width+borderBetweenLevels;
				levelColumn++;
				if (levelColumn>=numColumns){//next row
					x=pageX;
					y-=height+borderBetweenLevels;
					levelColumn=0;
					levelRow++;
				}
				if (levelRow>=numRows){//next page
					pageX+=(width*numRows)+(borderBetweenLevels*(numRows-1))+borderBetweenPages;
					x=pageX;
					y = -borderToScreen;
					levelColumn=0;
					levelRow=0;
				}
				levelI++;
			}
		}
		content.GetComponent<RectTransform> ().sizeDelta=new Vector2(pageX+(width*numRows)+(borderBetweenLevels*(numRows-1))+borderToScreen,0);
	}
}
