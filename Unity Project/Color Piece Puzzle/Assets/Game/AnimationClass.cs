using UnityEngine;
using System.Collections;

public class Animation{
	public PieceClasses.Piece piece;
	public Vector3 endPosition, startPosition;
	Color endColor;
	Color[] startColors;
	float totalTime;
	float startTime=-1f;
	Vector2 endScale, startScale;
	bool isBySpeed;

	public Animation(PieceClasses.Piece initPiece, Vector2 initEndPosition, Vector2 initEndScale, bool initBySpeed, float initSpeedOrTime){
		isBySpeed=initBySpeed;
		piece=initPiece;
		endPosition.z=0;
		endPosition=initEndPosition;
		startPosition=piece.gameObject.transform.localPosition;
		endScale=initEndScale;
		startScale=piece.gameObject.transform.localScale;
		float changeX=Mathf.Max(endPosition.x+endScale.x-startPosition.x, startPosition.x+startScale.x-endPosition.x);
		float changeY=Mathf.Max(endPosition.y+endScale.y-startPosition.y, startPosition.y+startScale.y-endPosition.y);
		Vector2 displacement=new Vector2(changeX, changeY);

		if (isBySpeed){
			totalTime=displacement.magnitude/initSpeedOrTime;
		}else
			totalTime=initSpeedOrTime;
		endColor = piece.color;
		startColors = new Color[piece.numSquaresOn];
		int i = 0;
		for (int y=0; y<piece.height; y++)
			for (int x=0; x<piece.width; x++)
			if (piece.squaresOn[x,y]){
				startColors[i]=piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color;
				i++;
			}

	}

	public bool animate(){
		if (startTime==-1f)
			startTime=Time.time;
		float percentage = (Time.time - startTime)/totalTime;
		percentage = fadeIn (percentage, 0f, 1f, 0f, 1f);
		if (percentage>=1) {
			piece.gameObject.transform.localPosition=endPosition;
			piece.gameObject.transform.localScale = endScale;
			piece.setSortingLayer("Pieces");
			for (int y=0; y<piece.height; y++)
				for (int x=0; x<piece.width; x++)
				if (piece.squaresOn[x,y]){
					piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=endColor;
				}

			return false;
		}

		Vector3 pos = piece.gameObject.transform.localPosition;
		piece.gameObject.transform.localPosition = new Vector3(startPosition.x+(endPosition.x-startPosition.x)*percentage, startPosition.y+(endPosition.y-startPosition.y)*percentage, pos.z);
		piece.gameObject.transform.localScale = new Vector2(startScale.x+(endScale.x-startScale.x)*percentage, startScale.y+(endScale.y-startScale.y)*percentage);

		int i = 0;
		for (int y=0; y<piece.height; y++)
			for (int x=0; x<piece.width; x++)
			if (piece.squaresOn[x,y]){
				float r=startColors[i].r+(endColor.r-startColors[i].r)*percentage;
				float g=startColors[i].g+(endColor.g-startColors[i].g)*percentage;
				float b=startColors[i].b+(endColor.b-startColors[i].b)*percentage;
				float a=startColors[i].a+(endColor.a-startColors[i].a)*percentage;
				piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().color=new Color(r,g,b,a);
				i++;
			}

		return true;
	}
	public static float fadeIn(float x, float minX, float maxX, float minY, float maxY){
		float percentage = (x - minX) / (maxX - minX);
		float Vf = 2f * (maxY - minY) / (maxX - minX);
		return .5f*(x-minX)*Vf*percentage+minY;
	}

}