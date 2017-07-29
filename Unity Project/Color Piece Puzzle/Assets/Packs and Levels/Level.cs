using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Level{
	public bool solved = false, unlocked=false;
	public int difficulty=0;
	public GameObject button;
//	public PieceClasses.GameBoard gameBoard;
	public PieceClasses.Piece backgroundPiece;

	public Level(bool unlocked, bool solved, int difficulty){
		this.unlocked = unlocked;
		this.solved = solved;
		this.difficulty = difficulty;
	}
}
