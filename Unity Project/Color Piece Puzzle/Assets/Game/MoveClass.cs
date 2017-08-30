using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveClass : MonoBehaviour {

	public class Move{
		public PieceClasses.Piece piece;
		public Vector2 startPosition;
		public Vector2 endPosition;

		public Move(PieceClasses.Piece piece, Vector2 startPosition, Vector2 endPosition){
			this.piece=piece;
			this.startPosition=startPosition;
			this.endPosition=endPosition;
		}
	}
}
