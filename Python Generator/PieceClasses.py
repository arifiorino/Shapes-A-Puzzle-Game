
def output2DBoolArray(squaresOn,on="■ ",off="  ",start=""):#\nSquaresOn
    print(start)
    for y in range(len(squaresOn)):
        for x in range(len(squaresOn[0])):
            if squaresOn[y][x]:
                print(on,end="")
            else:
                print(off,end="")
        print()


def output2DArray(squares,start=""):#\nSquaresOn
    print(start)
    for y in range(len(squares)):
        for x in range(len(squares[0])):
            print(squares[y][x],end="")
        print()

class Board():
    def __init__(self,width,height,backgroundPiece,pieces):
        self.width,self.height=width,height
        self.backgroundPiece=backgroundPiece
        self.pieces=pieces
        self.updateSquares()
    def updateSquares(self):
        self.squares=[[0 for x in range(self.width)] for y in range(self.height)]
        pieceI=0
        for piece in self.pieces:
            for y in range(piece.height):
                for x in range(piece.width):
                    if piece.squaresOn[y][x]:
                        self.squares[y+piece.y][x+piece.x]=pieceI
            pieceI+=1
    def outputMe(self):
        output2DArray(self.squares)
class Piece():
    def __init__(self,width,height,squares,startPos):
        self.width,self.height=width,height
        self.squares=squares
