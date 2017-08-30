import random,math,json

def randomDirection():
    direction=[0,0]
    direction[random.randint(0,1)]=random.choice([-1,1])
    return direction

def distanceTo(p1,p2):
    return math.sqrt((p1[0]-p2[0])**2+(p1[1]-p2[1])**2)

def randomSquareWithTendency(squaresPos, tendencySquarePos,smallest=1.0,power=3):
    # print("got:",squaresPos,"tending towards:",tendencySquarePos)
    probabilities=[distanceTo(squarePos,tendencySquarePos) for squarePos in squaresPos]
    largest=max(probabilities)
    probabilities=[largest-x+smallest for x in probabilities] #Flip because more distance is less probability
    probabilities=[x**power for x in probabilities] #Stretch data

    # print(probabilities)
    # print("max:",probabilities.index(max(probabilities)),max(probabilities),squaresPos[probabilities.index(max(probabilities))])
    r=random.random()*sum(probabilities)
    for i in range(len(probabilities)):
        if sum(probabilities[:i+1])>r:
            # print("picked:",i,probabilities[i],squaresPos[i])
            # input()
            return squaresPos[i]

def randomAdjacentSquareWithTendency(squarePos, tendencySquarePos,squares2D=None):
    sides=[[squarePos[0],squarePos[1]-1],[squarePos[0]+1,squarePos[1]],[squarePos[0],squarePos[1]+1],[squarePos[0]-1,squarePos[1]]]
    newSquarePos=randomSquareWithTendency(sides,tendencySquarePos)
    if not squares2D is None:
        width,height=len(squares2D[0]),len(squares2D)
        while (not newSquarePos[0] in range(width)) or (not newSquarePos[1] in range(height)) or squares2D[newSquarePos[1]][newSquarePos[0]]!=0:
            # print(not newSquarePos[0] in range(width), not newSquarePos[1] in range(height), squares2D[newSquarePos[1]][newSquarePos[0]]!=0)
            # print(squares2D[newSquarePos[1]][newSquarePos[0]])
            sides.remove(newSquarePos)

            if len(sides)<=0:
                return None

            newSquarePos=randomSquareWithTendency(sides,tendencySquarePos)

    return newSquarePos

def slightRandom(number,d=1):
    return round(number+(random.random()*2*d)-d)

def addVectors(v1,v2):
    return [v1[0]+v2[0],v1[1]+v2[1]]

def subtractVectors(v1,v2):
    return [v1[0]-v2[0],v1[1]-v2[1]]

def randomZeroSquare2D(squares, tendency=False):
    width,height=len(squares[0]),len(squares)
    possiblePos=[]
    for y in range(height):
        for x in range(width):
            if squares[y][x]==0:
                possiblePos.append([x,y])
    square=None
    while square is None or squares[square[1]][square[0]]!=0:
        if tendency:
            square=randomSquareWithTendency(possiblePos,[width/2,height/2],.1,4)
            # print("picked:",square)
        else:
            square=[random.randint(0,len(squares[0])-1),random.randint(0,len(squares)-1)]
    return square

def randomSquare2D(squares, tendency=False):
    width,height=len(squares[0]),len(squares)
    possiblePos=[]
    for y in range(height):
        for x in range(width):
            if squares[y][x]!=0:
                possiblePos.append([x,y])
    square=None
    while square is None or squares[square[1]][square[0]]==0:
        if tendency:
            square=randomSquareWithTendency(possiblePos,[width/2,height/2],1,3)
            # print("picked:",square)
        else:
            square=[random.randint(0,len(squares[0])-1),random.randint(0,len(squares)-1)]
    return square

def output2DBoolArray(squaresOn,on="■ ",off="  ",start=""):
    print(start)
    for y in range(len(squaresOn)):
        for x in range(len(squaresOn[0])):
            if squaresOn[y][x]:
                print(on,end="")
            else:
                print(off,end="")
        print()

def output2DArray(squares,start=""):
    print(start)
    for y in range(len(squares)):
        for x in range(len(squares[0])):
            print(squares[y][x],end="")
        print()

def squares2DToSquaresPos(array2D):
    squaresPos=[]
    for y in range(len(array2D)):
        for x in range(len(array2D[0])):
            if array2D[y][x]!=0:
                squaresPos.append([x,y])
    return squaresPos

def squares2DToSquares(squares2D):
    squares=[]
    for row in squares2D:
        squares.extend(row)
    return squares

def mean(array):
    return sum(array)/len(array)

def meanSquare(squares2D):
    squaresPos=squares2DToSquaresPos(squares2D)
    return [mean([s[0] for s in squaresPos]),mean([s[1] for s in squaresPos])]

def generateLevelSquares(levelWidth, levelHeight):
    backgroundWidth, backgroundHeight= round(levelWidth * .6), round(levelWidth * .6)
    if levelWidth%2 != backgroundWidth%2:
        backgroundWidth-=1

    backgroundSquares2D=[[0 for x in range(backgroundWidth)] for y in range(backgroundHeight)]
    numSquares=round(backgroundWidth*backgroundHeight*(math.pi/4)*.8)
    print("Number of squares:",numSquares)
    numRandomSquares=max(slightRandom(numSquares/8,1.5),1) #(1 in 8)+-2 are random
    print("Number of random squares:",numRandomSquares)
    numPieces= round(numSquares/3)
    print("Number of pieces:",numPieces)
    piecesSquaresPos=[[] for pieceI in range(numPieces)]
    for pieceI in range(numPieces):
        rSquare=randomZeroSquare2D(backgroundSquares2D, True)#.4
        backgroundSquares2D[rSquare[1]][rSquare[0]]=pieceI+1
        piecesSquaresPos[pieceI].append(rSquare)
    while sum([len(pieceSquaresPos) for pieceSquaresPos in piecesSquaresPos])<numSquares:

        numberOfSquaresForPieceI=[]
        for pieceI in range(numPieces):
            count=0
            for y in range(backgroundHeight):
                for x in range(backgroundWidth):
                    if backgroundSquares2D[y][x]==pieceI+1:
                        count+=1
            numberOfSquaresForPieceI.append(count)
        # print(numberOfSquaresForPieceI)
        piecesToChooseFrom=[pieceI for pieceI in range(numPieces) if numberOfSquaresForPieceI[pieceI]<=mean(numberOfSquaresForPieceI)+1.5 and numberOfSquaresForPieceI[pieceI]<5]
        # print(piecesToChooseFrom)
        chosenPieceI=random.choice(piecesToChooseFrom)

        # print("Chose piece:",chosenPieceI+1)

        s=randomSquareWithTendency(piecesSquaresPos[chosenPieceI],[backgroundWidth/2,backgroundHeight/2])
        newSquare=randomAdjacentSquareWithTendency(s,[backgroundWidth/2,backgroundHeight/2],backgroundSquares2D)
        count=0
        while newSquare is None and count<10000:
            s=randomSquareWithTendency(piecesSquaresPos[chosenPieceI],[backgroundWidth/2,backgroundHeight/2])
            newSquare=randomAdjacentSquareWithTendency(s,[backgroundWidth/2,backgroundHeight/2],backgroundSquares2D)
            count+=1

        if count>=10000:
            newSquare=randomZeroSquare2D(backgroundSquares2D, True)
            # print("Creating new start:",chosenPieceI+1,newSquare)

        piecesSquaresPos[chosenPieceI].append(newSquare)
        backgroundSquares2D[newSquare[1]][newSquare[0]]=chosenPieceI+1

    output2DArray(backgroundSquares2D)

    for randomSquareI in range(numRandomSquares):
        randomSquarePos=randomSquare2D(backgroundSquares2D)#,True


        boardString=json.dumps(backgroundSquares2D)

        if boardString.count(str(backgroundSquares2D[randomSquarePos[1]][randomSquarePos[0]]))!=1: #If it's not the last one left
            backgroundSquares2D[randomSquarePos[1]][randomSquarePos[0]]=random.randint(0,numPieces-1)
        print("Random Square at:",randomSquarePos[0],",",randomSquarePos[1])

    output2DArray(backgroundSquares2D)
    return backgroundSquares2D

# generateLevelSquares(6,8)