using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadPackSelect : MonoBehaviour {
	public GameObject content;
	public GameObject packButtonPrefab;

	int borderBetweenPacks=5;
	int borderToTop=10;

	void Start () {
		float top = -borderToTop;
		int packI = 0;
		foreach (Pack pack in PackPresets.packs) {
			pack.button=(GameObject)Instantiate(packButtonPrefab, new Vector3(0,top,0), Quaternion.identity);
			pack.button.transform.SetParent(content.transform, false);

			pack.button.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,top);
			pack.button.GetComponentsInChildren<Text>()[0].text=""+pack.name;
			pack.button.GetComponentsInChildren<Text>()[1].text=pack.getScore();

			int tempPackI=packI;
			pack.button.GetComponent<Button>().onClick.AddListener(delegate() {
				PackPresets.currentPack=PackPresets.packs[tempPackI];
				Application.LoadLevel (2); //scene of levels
			});


			top-=packButtonPrefab.GetComponent<RectTransform>().rect.height;
			top-=borderBetweenPacks;
			packI++;
		}

		Vector2 size=content.GetComponent<RectTransform> ().sizeDelta;
		RectTransform rt = content.GetComponent<RectTransform> ();
		content.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -top+borderToTop);
	}

}
