using UnityEngine;
using System.Collections;

public class VectorMethods{

	public static Vector2 toVector2(Vector3 vector){
		return new Vector2 (vector.x, vector.y);
	}
	
	public static Vector2 addVectors(Vector2 vector1, Vector2 vector2){
		return new Vector2 (vector1.x + vector2.x, vector1.y + vector2.y);
	}

	public static Vector2 addVectors(Vector2 vector1, float scale){
		return new Vector2 (vector1.x + scale, vector1.y + scale);
	}
	
	public static Vector2 subtractVectors(Vector2 vector1, Vector2 vector2){
		return new Vector2 (vector1.x - vector2.x, vector1.y - vector2.y);
	}

	public static Vector2 subtractVectors(Vector2 vector1, float scale){
		return new Vector2 (vector1.x - scale, vector1.y - scale);
	}
	
	public static Vector2 multiplyVectors(Vector2 vector1, Vector2 vector2){
		return new Vector2 (vector1.x * vector2.x, vector1.y * vector2.y);
	}

	public static Vector2 multiplyVectors(Vector2 vector1, float scale){
		return new Vector2 (vector1.x * scale, vector1.y * scale);
	}
	
	public static Vector2 divideVectors(Vector2 vector1, Vector2 vector2){
		return new Vector2 (vector1.x / vector2.x, vector1.y / vector2.y);
	}

	public static Vector2 divideVectors(Vector2 vector1, float scale){
		return new Vector2 (vector1.x / scale, vector1.y / scale);
	}

	public static Vector2 flipY(Vector2 vector1){
		return new Vector2 (vector1.x, -vector1.y);
	}

	public static Vector3 add3rdDimension(Vector2 vector1, float z){
		return new Vector3 (vector1.x, vector1.y, z);
	}

}
