import random
from BackgroundPiece import *
from PieceClasses import *

def slightRandom(number,d=2):
    return number+random.randint(-int(d),int(d))

def addVectors(v1,v2):
    return [v1[0]+v2[0],v1[1]+v2[1]]

def randomSquare(squares):
    x,y=random.randint(0,len(squares[0])-1),random.randint(0,len(squares)-1)
    while squares[y][x]!=1:
        x,y=random.randint(0,len(squares[0])-1),random.randint(0,len(squares)-1)
    return [x,y]

def randomDirection():
    direction=[0,0]
    direction[random.randint(0,1)]=random.choice([-1,1])
    return direction

def getBorderSquares(squares):
    width,height=len(squares[0]),len(squares)
    borderSquares=[]
    for x in range(width):
        for y in range(height):
            if squares[y][x]==1:
                if not squares[y][x] in borderSquares:
                    borderSquares.append([x,y])
                break

    for y in range(height): #go right to left
        for x in range(width-1,-1,-1):
            if squares[y][x]==1:
                if not squares[y][x] in borderSquares:
                    borderSquares.append([x,y])
                break

    for x in range(width):
        for y in range(height-1,-1,-1):
            if squares[y][x]==1:
                if not squares[y][x] in borderSquares:
                    borderSquares.append([x,y])
                break

    for y in range(height): #go right to left
        for x in range(width):
            if squares[y][x]==1:
                if not squares[y][x] in borderSquares:
                    borderSquares.append([x,y])
                break
    return borderSquares

def splitSquaresInHalf(squares):
    start=random.choice(getBorderSquares(squares))


def splitBackgroundSquares(backgroundSquares, numSquaresInPiece):
    # output2DArray(backgroundSquares,"Start of split")
    bWidth,bHeight=len(backgroundSquares[0]),len(backgroundSquares)
    numSquares=sum([sum(row) for row in backgroundSquares])
    numPieces=int(numSquares/numSquaresInPiece)
    # numSquaresInPiece=int(numSquares/numPieces)
    # print("Each piece has:",numSquaresInPiece)
    for pieceI in range(numPieces-1):
        thisNumSquaresInPiece=slightRandom(numSquaresInPiece,1)
        # output2DArray(backgroundSquares)
        # print("On piece:",pieceI+2)
        pieceSquares=[random.choice(getBorderSquares(backgroundSquares))]
        backgroundSquares[pieceSquares[-1][1]][pieceSquares[-1][0]]=pieceI+2
        # print("Starting at:",pieceSquares[-1])
        while len(pieceSquares)<thisNumSquaresInPiece:
            count=0
            while len(pieceSquares)<thisNumSquaresInPiece and count<numSquares*100:#checking for infinite loop
                newSquare=addVectors(random.choice(pieceSquares),randomDirection())
                if newSquare[0] in range(bWidth-1) and newSquare[1] in range(bHeight-1):#checking if piece fits
                    if backgroundSquares[newSquare[1]][newSquare[0]]==1 and not (newSquare in pieceSquares):
                        backgroundSquares[newSquare[1]][newSquare[0]]=pieceI+2
                        pieceSquares.append(newSquare)
                count+=1
            if len(pieceSquares)<thisNumSquaresInPiece:
                pieceSquares.append(random.choice(getBorderSquares(backgroundSquares)))
                backgroundSquares[pieceSquares[-1][1]][pieceSquares[-1][0]]=pieceI+2
                # print("Starting at:",pieceSquares[-1])
    # output2DArray(backgroundSquares,"end of split")
    return backgroundSquares

# s=splitBackgroundSquares(generatePieceSquares(4,6),4)
# for y in range(len(s)):
#     for x in range(len(s[0])):
#         if s[y][x]==1:
#             s[y][x]=0
# output2DArray(s,"\nFinal")
