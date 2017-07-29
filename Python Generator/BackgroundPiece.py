import random,math
from PieceClasses import *



def roundDown(x):
    return int(x)

def roundUp(x):
    return int(math.ceil(x))

def symmetrize(squares):
    for y in range(len(squares)):
        for x in range(len(squares[0])):
            squares[y][x]=int(squares[y][x] or squares[y][len(squares[0])-1-x])
    return squares

def generateRandomPieceSquares(width,height,numSquares):
    squares=[[0 for x in range(width)] for y in range(height)]
    i=0
    while i<numSquares:
        x,y=random.randint(0,width-1),random.randint(0,height-1)
        if not squares[y][x]:
            squares[y][x]+=1
            i+=1
    return squares

#positive high number: very stretched
#positive low number: slightly stretched

#negative high abs number: very shrinked
#negative low  abs number: slightly shrinked

def generatePieceSquares(width,height,stretchFactor=-.4):
    squares=[[0 for x in range(width)] for y in range(height)]
    squares[int(height/2)][int(width/2)]=1
    top,right,bottom,left=height,-1,-1,width

    while top!=0 or right!=width-1 or bottom!=height-1 or left!=0:
        top=height
        topSquares=[]
        for x in range(width):
            for y in range(height):
                if squares[y][x]:
                    topSquares.append([x,y])
                    break
                y+=1
            if y<top:
                top=y

        right=-1
        rightSquares=[]
        for y in range(height): #go right to left
            for x in range(width-1,-1,-1):
                if squares[y][x]:
                    rightSquares.append([x,y])
                    break
                x-=1
            if x>right:
                right=x

        bottom=-1
        bottomSquares=[]
        for x in range(width):
            for y in range(height-1,-1,-1):
                if squares[y][x]:
                    bottomSquares.append([x,y])
                    break
                y-=1
            if y>bottom:
                bottom=y

        left=width
        leftSquares=[]
        for y in range(height): #go right to left
            for x in range(width):
                if squares[y][x]:
                    leftSquares.append([x,y])
                    break
                x+=1
            if x<left:
                left=x

        sides=[top,width-1-right,height-1-bottom,left]
        sidesToChoose= [i for i, m in enumerate(sides) if m == max(sides)]
        side=random.choice(sidesToChoose)
        percentageOfSide=math.fabs(stretchFactor)
        if stretchFactor>0:
            percentageOfSide=1-percentageOfSide
        if side==0:#top
            topSquares.sort(key=lambda x:x[1])
            if stretchFactor>0:
                topSquares= topSquares[:roundUp(len(topSquares) * percentageOfSide)]
            else:
                topSquares= topSquares[roundDown(len(topSquares) * percentageOfSide):]
            square=random.choice(topSquares)
            if square[1]>0:
                squares[square[1]-1][square[0]]=1
        elif side==1:#right
            rightSquares.sort(key=lambda x:x[0],reverse=True)
            if stretchFactor>0:
                rightSquares= rightSquares[:roundUp(len(rightSquares) * percentageOfSide)]
            else:
                rightSquares= rightSquares[roundDown(len(rightSquares) * percentageOfSide):]
            square=random.choice(rightSquares)
            if square[0]<width-1:
                squares[square[1]][square[0]+1]=1
        elif side==2:#bottom
            bottomSquares.sort(key=lambda x:x[1],reverse=True)
            if stretchFactor>0:
                bottomSquares= bottomSquares[:roundUp(len(bottomSquares) * percentageOfSide)]
            else:
                bottomSquares= bottomSquares[roundDown(len(bottomSquares) * percentageOfSide):]
            square=random.choice(bottomSquares)
            if square[1]<height-1:
                squares[square[1]+1][square[0]]=1
        elif side==3:#left
            leftSquares.sort(key=lambda x:x[0])
            if stretchFactor>0:
                leftSquares= leftSquares[:roundUp(len(leftSquares) * percentageOfSide)]
            else:
                leftSquares= leftSquares[roundDown(len(leftSquares) * percentageOfSide):]
            square=random.choice(leftSquares)
            if square[0]>0:
                squares[square[1]][square[0]-1]=1
    return squares

# output2DBoolArray(generatePieceSquares(5,5))