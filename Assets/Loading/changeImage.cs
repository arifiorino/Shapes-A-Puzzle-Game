using UnityEngine;
using System.Collections;

public class changeImage : MonoBehaviour {
	float timeTillChange=.5f;
	public Sprite[] sprites;
	int spriteIndex=0;
	public SpriteRenderer spriteRenderer;
	
	void Update () {
		timeTillChange -= Time.deltaTime;
		if (timeTillChange < 0) {
			spriteRenderer.sprite=sprites[spriteIndex];
			spriteIndex++;
			spriteIndex%=sprites.Length;
			timeTillChange=.5f;
		}
	}
}
