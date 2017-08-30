import json, re
from GenerateLevel import *
level={}

#piece: size, squaresOn, color

# colorPalette=[[0,160,176],[204,51,63],[106,74,60],[57,121,37],[237,201,81],[235,104,65],[11,39,75],[125,118,98],[157,107,53],[107,174,200]]

pack1Palette=[[0,1,2,5],[3,4],[6,7,8,9]]
pack2Palette=[[1,4,5,2],[3,8],[0,6,7,9]]
pack3Palette=[[0,3,9],[2,5,8],[1,4,6,7]]
pack4Palette=[[4,5,9],[3,1,0],[2,6,7,8]]
pack5Palette=[[4,6,9,0],[8,2],[5,1,3,7]]

allPalettes=[pack1Palette,pack2Palette,pack3Palette,pack4Palette,pack5Palette]

def trimBoardAndPositionPieces(boardDict):
    backgroundPieceSize=boardDict['backgroundPiece']['size']
    boardSize=boardDict['boardSize']

    if backgroundPieceSize[0]%2 != boardSize[0]%2:
        boardSize[0]-=1
    if backgroundPieceSize[1]%2 != boardSize[1]%2:
        boardSize[1]-=1

    levelCount=0
    levelDone=False
    while levelCount<100 and not levelDone:
        levelCount+=1
        board=[[0 for x in range(boardSize[0])] for y in range(boardSize[1])]
        bgX=int((boardSize[0]-backgroundPieceSize[0])/2)
        bgY=int((boardSize[1]-backgroundPieceSize[1])/2)

        for bgDY in range(backgroundPieceSize[1]):
            for bgDX in range(backgroundPieceSize[0]):
                if boardDict['backgroundPiece']['squaresOn'][bgDY*backgroundPieceSize[0]+bgDX] == 1:
                    board[bgY+bgDY][bgX+bgDX]=1


        piecesSorted=sorted(boardDict['pieces'],key=lambda piece: piece['size'][0]*piece['size'][1], reverse=True)
        piecesSorted=piecesSorted.copy()


        pieceI=0
        done=False

        while not done and pieceI<len(piecesSorted): # A for loop that cuts off after a piece doesn't find it's spot

            piece=piecesSorted[pieceI]

            pieceCount=0
            intersects=True
            while intersects and pieceCount<10000: # Checks all options for that piece
                pieceX=random.randint(0,boardSize[0]-piece['size'][0])
                pieceY=random.randint(0,boardSize[1]-piece['size'][1])
                intersects=False
                for pieceDY in range(piece['size'][1]):
                    for pieceDX in range(piece['size'][0]):
                        if piece['squaresOn'][pieceDY*piece['size'][0]+pieceDX]==1:
                            changes=[[-1,0],[1,0],[0,-1],[0,1],[0,0]]
                            for change in changes: #Check all squares around it too...
                                if pieceY+pieceDY+change[1] in range (0,boardSize[1]) and pieceX+pieceDX+change[0] in range (0,boardSize[0]) and board[pieceY+pieceDY+change[1]][pieceX+pieceDX+change[0]]==1:
                                    intersects=True
                pieceCount+=1

            if pieceCount==10000: #Didn't find any options left
                done=True
            else: #Draw it on the board
                piecesSorted[pieceI]["startPos"]=[pieceX,pieceY]
                for pieceDY in range(piece['size'][1]):
                        for pieceDX in range(piece['size'][0]):
                            if piece['squaresOn'][pieceDY*piece['size'][0]+pieceDX]==1:
                                board[pieceY+pieceDY][pieceX+pieceDX]=1
            pieceI+=1

        if done: #try again
            levelCount+=1
        else: #finish
            boardDict["pieces"]=piecesSorted
            levelDone=True

    output2DBoolArray(board,off='+ ')

    if levelCount==100:
        input("No positions found.")


def generateLevel(packI, width, height):
    # random.shuffle(colorPalette)

    colorPalette=[]
    for section in allPalettes[packI]:
        random.shuffle(section)
        for color in section:
            colorPalette.append(color)

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
    backgroundPiece={"size":[backgroundWidth, backgroundHeight], "color":10}
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

        pieceDict={"size":[maxX-minX+1,maxY-minY+1],"squaresOn":pieceSquares,"color":colorPalette[pieceI-1],"startPos":[0,0]}
        boardPiecesDicts.append(pieceDict)
        pieceI+=1

        pieceSquaresPos=[]
        for y in range(len(boardSquares)):
            for x in range(len(boardSquares[0])):
                if boardSquares[y][x]==pieceI:
                    pieceSquaresPos.append([x,y])

    if len(boardPiecesDicts)==0:
        input("Problem!")

    boardDict["pieces"]=boardPiecesDicts

    trimBoardAndPositionPieces(boardDict)

    return boardDict

def makeDictPretty(d):
  r=json.dumps(d, indent=4, sort_keys=True)
  anyLeft=True
  while anyLeft:
    m=re.search('\n+ +[\d}\]]',r)
    if m is None:
      return r
    r=r[0:m.start()]+r[m.end()-1:]
  return r

for packI in range(1,6):
    for levelI in range(1,51):
        print("\n\nLevel:"+str(levelI))
        file=open("Levels/Pack "+str(packI)+"/Pack"+str(packI)+"Level"+str(levelI)+".txt",'w')
        width=int(7+(levelI*.1))
        height=int((16/9)*width)
        print("Size:",str(width)+",",height)

        # level=""
        # while level=="":
        #     try:
        level=makeDictPretty(generateLevel(packI-1, width,height))
            # except Exception as a:
            #     print(a)

        file.write(level)
        file.close()
print()
