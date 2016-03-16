using UnityEngine;
using System.Collections;
public class PieceClasses : MonoBehaviour{

	public class GameBoard{
		public GameObject gameObject;
		public Piece backgroundPiece;
		public Piece[] pieces;
		public bool[,] squaresOn;
		public Square[,] squares;
		public Vector2 size;
		public int numSquaresOn;

		public GameBoard(Vector2 initSize){
			size=initSize;
		}

		public void updateSquares(){
			squares = new Square[(int)size.x, (int)size.y];
			squaresOn = new bool[(int)size.x, (int)size.y];
			numSquaresOn = 0;
			foreach (Piece p in pieces)
				if (p.isOnBoard)
					for (int y=0; y<p.height; y++)
						for (int x=0; x<p.width; x++)
							if (p.squaresOn[x,y]){
								squares[(int)p.position.x+x, (int)p.position.y+y]= p.squares[x,y];
								squaresOn[(int)p.position.x+x, (int)p.position.y+y]=true;
								numSquaresOn++;
							}
			//output2dArray(squaresOn,"Update Squares");
		}

		public bool[,] squaresOnWithoutPiece(Piece piece){
			bool[,] mySquaresOn = new bool[(int)size.x, (int)size.y];
			foreach (Piece p in pieces)
				if (p!=piece)
					for (int y=0; y<p.height; y++)
						for (int x=0; x<p.width; x++)
							if (p.squaresOn[x,y]){
								mySquaresOn[(int)p.position.x+x, (int)p.position.y+y]=true;
							}
			return mySquaresOn;
		}

		public void pickUpOrDropPiece(Piece piece, bool pickUp=true){
			piece.isOnBoard= !pickUp;
			updateSquares();
			foreach (Piece p in pieces) {
				p.adaptToBoard(this);
			}
			if (!piece.isOnBoard)
				piece.makeCurves ();
		}

		public Vector2 movePiece(Piece piece, Vector2 newLocation, bool moveGameObject=true, bool checkIntersection=true){ //adapts to board too
			if (newLocation.x < 0)
				newLocation.x = 0;
			if (newLocation.y < 0)
				newLocation.y = 0;
			if (newLocation.x > size.x - piece.width)
				newLocation.x = size.x - piece.width;
			if (newLocation.y > size.y - piece.height)
				newLocation.y = size.y - piece.height;

			Vector2 oldPos = piece.position;
			piece.position = newLocation;
			if (checkIntersection && intersects (piece)){
				piece.position= oldPos;
				if (moveGameObject)
					piece.gameObject.transform.localPosition = new Vector2 (piece.position.x, -piece.position.y);
				return new Vector2(piece.position.x, -piece.position.y);
			}

			if (moveGameObject)
				piece.gameObject.transform.localPosition = new Vector2(piece.position.x, -piece.position.y);
			updateSquares();
			if (checkIntersection)
				foreach (Piece p in pieces) {
					p.adaptToBoard(this);
				}
			return new Vector2(piece.position.x, -piece.position.y);
		}

		public bool intersects(Piece piece){
			bool[,] mySquaresOn = squaresOnWithoutPiece (piece);
			if (mySquaresOn == null)
				return false;

			for (int x=0; x<piece.width; x++)
				for (int y=0; y<piece.height; y++)
					if (mySquaresOn [x + (int)piece.position.x, y + (int)piece.position.y] && piece.squaresOn [x, y]) {
						output2dArray (mySquaresOn,"Intersection "+piece.position.ToString());
						return true;
					}
			return false;
		}

		public bool isSolved(){
			for (int y=0; y<backgroundPiece.height; y++)
				for (int x=0; x<backgroundPiece.width; x++)
					if (backgroundPiece.squaresOn[x, y] && !squaresOn[x+(int)backgroundPiece.position.x, y+(int)backgroundPiece.position.y])
						return false;
			return true;
		}

		public void setSpritesOrderInLayer(string sortingLayer="Pieces"){
			int i = 0;
			foreach (PieceClasses.Piece piece in pieces){
				piece.setSortingLayer(sortingLayer);
				for (int y=0;y<piece.height;y++)
					for (int x=0;x<piece.width;x++)
						if (piece.squaresOn[x,y])
							piece.squares[x,y].gameObject.GetComponent<SpriteRenderer>().sortingOrder=i;
						
				i++;
			}
		}
	}
	
	public class Piece{
		public Animation animation;
		public GameObject gameObject;
		public Square[,] squares;
		public bool[,] squaresOn;
		public Color color;
		public Vector2 position; //location of top left corner on grid
		public int width, height;
		public bool isOnBoard = true, isAnimating = false;
		public int numSquaresOn=0;
		
		public Piece(bool[,] initPiecesOn, Color initColor, Vector2 initPosition){
			position=initPosition;
			color=initColor;
			squaresOn=initPiecesOn;
			width = squaresOn.GetLength(0);
			height=squaresOn.GetLength(1);
			squares= new Square[width, height];
			makeCurves();
			for (int y=0; y<height; y++)
				for (int x=0; x<width; x++)
					if (squaresOn[x,y])
						numSquaresOn++;
		}


		public void makeCurves(){
			for (int y=0; y<height; y++)
				for (int x=0; x<width; x++)
					if (squaresOn[x,y]){
						bool topRight= !((x<width-1 && squaresOn[x+1,y]) || (y>0 && squaresOn[x,y-1]));
						bool topLeft= !((x>0 && squaresOn[x-1,y]) || (y>0 && squaresOn[x,y-1]));
						bool bottomRight= !((x<width-1 && squaresOn[x+1,y]) || (y<height-1 && squaresOn[x,y+1]));
						bool bottomLeft= !((x>0 && squaresOn[x-1,y]) || (y<height-1 && squaresOn[x,y+1]));
						if (squares[x,y]==null)
							squares[x,y]=new Square(new bool[]{topRight, topLeft, bottomLeft, bottomRight});
						else
							squares[x,y].setup(new bool[]{topRight, topLeft, bottomLeft, bottomRight});
						squares[x,y].color=this.color;
				}
		}
		public void adaptToBoard(GameBoard board){
			for (int y=(int)position.y; y<position.y+height; y++)
				for (int x=(int)position.x; x<position.x+width; x++)
					if (board.squaresOn[x,y] && squaresOn[x-(int)position.x,y-(int)position.y]){
						bool topRight= !((x<board.size.x-1 && board.squaresOn[x+1,y]) || (y>0 && board.squaresOn[x,y-1]));
						bool topLeft= !((x>0 && board.squaresOn[x-1,y]) || (y>0 && board.squaresOn[x,y-1]));
						bool bottomRight= !((x<board.size.x-1 && board.squaresOn[x+1,y]) || (y<board.size.y-1 && board.squaresOn[x,y+1]));
						bool bottomLeft= !((x>0 && board.squaresOn[x-1,y]) || (y<board.size.y-1 && board.squaresOn[x,y+1]));
						squares[x-(int)position.x,y-(int)position.y].setup(new bool[]{topRight, topLeft, bottomLeft, bottomRight});
					}
		}
		public bool intersects(Vector2 point){
			for (int y=0; y<height; y++)
				for (int x=0; x<width; x++)
					if (squaresOn[x,y]){
						Vector2 center=VectorMethods.addVectors(squares[x,y].gameObject.transform.localPosition, gameObject.transform.localPosition);
						Bounds bounds = new Bounds (center, new Vector2 (1,1));
						if (bounds.Contains(point))
							return true;
					}
			return false;
		}

		public void setSortingLayer(string s){
			for (int y=0; y<height; y++)
				for (int x=0; x<width; x++)
					if (squaresOn [x, y]) {
						squares[x,y].gameObject.GetComponent<SpriteRenderer>().sortingLayerName=s;
					}
		}
	}

	public class Square{
		public GameObject gameObject;
		public bool[] cornersOn;
		public int[,] piecePosition;
		public int numCorners;
		public int rotation;
		public int prefabNum;
		public Color color;
		public Square(bool[] initCornersOn){
			setup(initCornersOn);
		}
		public void setup(bool[] initCornersOn){
			cornersOn=initCornersOn;
			numCorners=0;
			foreach (bool c in cornersOn)
				if (c)
					numCorners++;
			prefabNum=numCorners;
			rotation=0;
			if (numCorners==1){
				for (int i=0;i<4;i++){
					if (cornersOn[i])
						rotation=i*90;
				}
			}
			else if (numCorners==2){
				for (int i=0;i<4;i++)
					if (cornersOn[i] && cornersOn[(i+1)%4])
						rotation=i*90;
				if (cornersOn[0]&&cornersOn[2] || cornersOn[1]&&cornersOn[3])
					prefabNum=5;
				if (cornersOn[1]&&cornersOn[3])
					rotation=90;
			}
			else if (numCorners==3){
				for (int i=0;i<4;i++)
					if (cornersOn[i] && cornersOn[(i+1)%4] && cornersOn[(i+2)%4])
						rotation=i*90;
			}
		}
	}

	public static void output2dArray(bool[,] array, string start=""){
		if (start != "")
			start += "\n";
		string output = start;
		for (int y=0;y<array.GetLength(1);y++){
			for (int x=0;x<array.GetLength(0);x++)
				if (array[x,y])
					output+="x";
				else
					output+="-";
			output+="\n";
		}
		Debug.Log(output);
	}
}
