using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Animate : MonoBehaviour {
	public GameObject outerCircle;
	public GameObject innerCircle;
	public GameObject loadingArc;
	public Text titleText;

	string title="Ari Productions";
	float angle=0f; //vertical
	float size; //diameter
	float maxSize;
	bool sizeIncreasing = false;
	float angleDelta=2f;
	Color[] colors;
	int colorI=-1;
	float percentageLoaded=0f;
	float loadSpeed=.005f;
	float timeTillLoadSpeedChange=.2f;
	Scheduler mainScheduler;

	void Start(){
		colors = new Color[] {
			new Color (204 / 255f, 51 / 255f, 63 / 255f), //red
			new Color (57 / 255f, 121 / 255f, 37 / 255f), //green
			new Color (235 / 255f, 104 / 255f, 65 / 255f),//orange
			new Color (107 / 255f, 174 / 255f, 200 / 255f)//light blue
//			new Color (11 / 255f, 39 / 255f, 75 / 255f),  //dark blue
		};

		size = innerCircle.transform.localScale.x;
		maxSize = size;
		mainScheduler = new Scheduler ();
		mainScheduler.addTime (5f);
		mainScheduler.addUpdateFunction (updateAnimation);
//		mainScheduler.addTime (.5f);
		mainScheduler.addInitFunction (delegate {
			SceneManager.LoadScene("Start/Start");
		});
		mainScheduler.start ();
	}

	bool updateAnimation(){
		if (sizeIncreasing) {
			size = maxSize * angle / 360;
		} else {
			size = maxSize * (1 - (angle / 360));
		}

		float circleDistance = (maxSize / 2) - (size / 2);

		Vector3 circlePosition = new Vector3 ();
		circlePosition.x = Mathf.Cos ((angle+90)*Mathf.Deg2Rad) * circleDistance;
		circlePosition.y = Mathf.Sin ((angle+90)*Mathf.Deg2Rad) * circleDistance;
		circlePosition.z = -1;

		innerCircle.transform.localPosition=circlePosition;
		innerCircle.transform.localScale=new Vector2(size,size);

		angleDelta = 100 + (angle / 360f * 300);

		angle += angleDelta*Time.deltaTime;
		if (angle > 360f) {
			angle = 0;
			sizeIncreasing = !sizeIncreasing;
			if (sizeIncreasing) {
				if (percentageLoaded >= 1f)
					return false;
				colorI++;
				colorI %= colors.Length;
				innerCircle.GetComponent<SpriteRenderer> ().color = colors [colorI];
				loadingArc.GetComponent<LineRenderer>().SetColors(colors [colorI],colors [colorI]);
			}
		}

		timeTillLoadSpeedChange -= Time.deltaTime;
		if (timeTillLoadSpeedChange < 0f) {
			timeTillLoadSpeedChange = (Random.value * .5f) + .1f;
			loadSpeed=(Random.value * .0025f) + .0001f;
		}
		percentageLoaded += loadSpeed;
		percentageLoaded = Mathf.Min (percentageLoaded, 1f);

		float loadingAngle = percentageLoaded * 360f;
		int numberOfPoints = 0;
		for (float a = 0f; a <= loadingAngle; a++)
			numberOfPoints++;
	
		loadingArc.GetComponent<LineRenderer> ().SetVertexCount (numberOfPoints);
		int i = 0;
		for (float a = 0f; a <= loadingAngle; a++) {
			float d = (maxSize / 2f) + ((outerCircle.transform.localScale.x - maxSize) / 4f);
			Vector3 position = new Vector3 ();
			position.x = Mathf.Cos ((a + 90) * Mathf.Deg2Rad) * d;
			position.y = Mathf.Sin ((a + 90) * Mathf.Deg2Rad) * d;
			position.z = -2;
			loadingArc.GetComponent<LineRenderer> ().SetPosition (i, position);
			i++;
		}

		if (percentageLoaded <= .25f)
			titleText.text = title.Substring (0, (int)(title.Length * percentageLoaded * 4f));
		else
			titleText.text = title;

		return true;
	}

	void Update () {
		mainScheduler.update ();
	}
}