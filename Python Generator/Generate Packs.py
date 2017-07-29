import json
from GenerateLevel import *
level={}

#piece: size, squaresOn, color

colorPalette=[[0,160,176],[106,74,60],[204,51,63],[235,104,65],[237,201,81],[57,121,37],[11,39,75],[125,118,98],[157,107,53],[107,174,200]]

def generateLevel(width, height, symmetry=False):
    random.shuffle(colorPalette)
    boardDict={"boardSize":[width, height]}

    boardSquares=generateLevelSquares(width,height)
    backgroundSquaresPos=squares2DToSquaresPos(boardSquares)

    minX=min([s[0] for s in backgroundSquaresPos])
    maxX=max([s[0] for s in backgroundSquaresPos])
    minY=min([s[1] for s in backgroundSquaresPos])
    maxY=max([s[1] for s in backgroundSquaresPos])
    backgroundSquaresPos=[[s[0]-minX,s[1]-minY] for s in backgroundSquaresPos]

    backgroundSquares2D=[[0 for x in range(maxX-minX+1)] for i in range(maxY-minY+1)]
    for y in range(maxY-minY+1):
        for x in range(maxX-minX+1):
            if [x,y] in backgroundSquaresPos:
                backgroundSquares2D[y][x]=1
    backgroundSquares=[]
    for row in backgroundSquares2D:
        backgroundSquares.extend(row)
    backgroundWidth,backgroundHeight=len(backgroundSquares2D[0]),len(backgroundSquares2D)


    print("Background size:",backgroundWidth,backgroundHeight)
    backgroundPiece={"size":[backgroundWidth, backgroundHeight], "color":[220,220,220]}
    output2DBoolArray(backgroundSquares2D)
    backgroundSquares=[]
    for row in backgroundSquares2D:
        backgroundSquares.extend(row)
    backgroundPiece["squaresOn"]=backgroundSquares
    boardDict["backgroundPiece"]=backgroundPiece


    boardPiecesDicts=[]
    pieceI=1
    pieceSquaresPos=[]
    for y in range(len(boardSquares)):
        for x in range(len(boardSquares[0])):
            if boardSquares[y][x]==pieceI:
                pieceSquaresPos.append([x,y])

    while len(pieceSquaresPos)>0:
        minX=min([s[0] for s in pieceSquaresPos])
        maxX=max([s[0] for s in pieceSquaresPos])
        minY=min([s[1] for s in pieceSquaresPos])
        maxY=max([s[1] for s in pieceSquaresPos])
        pieceSquaresPos=[[s[0]-minX,s[1]-minY] for s in pieceSquaresPos]
        pieceSquares2D=[[0 for x in range(maxX-minX+1)] for i in range(maxY-minY+1)]

        for y in range(maxY-minY+1):
            for x in range(maxX-minX+1):
                if [x,y] in pieceSquaresPos:
                    pieceSquares2D[y][x]=1
        pieceSquares=[]
        for row in pieceSquares2D:
            pieceSquares.extend(row)

        pieceDict={"size":[maxX-minX+1,maxY-minY+1],"squaresOn":pieceSquares,"color":colorPalette[pieceI-1],"startPos":[minX,maxY]}
        boardPiecesDicts.append(pieceDict)
        pieceI+=1

        pieceSquaresPos=[]
        for y in range(len(boardSquares)):
            for x in range(len(boardSquares[0])):
                if boardSquares[y][x]==pieceI:
                    pieceSquaresPos.append([x,y])

    boardDict["pieces"]=boardPiecesDicts
    return boardDict

for packI in range(1,2):
    for levelI in range(1,51):
        print("Level:"+str(levelI))
        file=open("Levels/Pack"+str(packI)+"Level"+str(levelI)+".txt",'w')
        width=int(7+(levelI*.1))
        height=int((16/9)*width)

        level=""
        while level=="":
            try:
                level=json.dumps(generateLevel(width,height))
            except Exception as a:
                print(a)

        file.write(level)
        file.close()
print()