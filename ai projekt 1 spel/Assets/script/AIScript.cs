using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AIScript : MonoBehaviour {

    [SerializeField]
    private CarScript carScript;
    [SerializeField]
    private TileScript tileScript;

    [SerializeField]
    private List<TileForList> openlist = new List<TileForList>();
    [SerializeField]
    private HashSet<TileForClosedList> closedlist = new HashSet<TileForClosedList>();

    [SerializeField]
    private Stack<TileForList> optimalSolution = new Stack<TileForList>();

    private Vector2Int carPos;

    [SerializeField]
    private bool debugMode = false;

    private bool haveFinished = false;

    private int lowestTotalCost;

    [SerializeField]
    private bool createDebugTiles = false;
    [SerializeField]
    private GameObject debugTile;

    // Use this for initialization

    public bool HaveFinished {
        get { return haveFinished; }
    }
    void Start() {
    }

    public void prepareAI() {
        carPos = new Vector2Int((int)carScript.transform.position.x, (int)carScript.transform.position.y);
        lowestTotalCost = -1;
        Stopwatch s = new Stopwatch();
        s.Start();
        for (int i = 0; i < tileScript.FinishTile.GetLength(0); i++) {
            Vector2Int targetPosition = tileScript.FinishTile[i];
            TileForList startTile = new TileForList(carPos, 0, calculateDistanceToTarget(carPos, targetPosition), carScript.CurrentSpeed, null, "Start Tile", targetPosition);
            if (debugMode) {
                print(startTile.ToString());
            }
            openlist.Clear();
            closedlist.Clear();
            openlist.Add(startTile);
            findOptimalPath();
        }
        s.Stop();
        Debug.Log("prepareAI time taken" + s.Elapsed.TotalSeconds);
    }

    private void findOptimalPath() {
        int sentinel = 5000;
        while (0 < openlist.Count && 0 < sentinel) {
            if (debugMode)
            {
                print("sentinel: " + sentinel);
            }
            sentinel--;
            TileForList currentTile = openlist[0];
            openlist.RemoveAt(0);
            closedlist.Add(new TileForClosedList(currentTile));
            if (debugMode) {
                print("currentTile: " + currentTile.ToString());
            }
            if(currentTile.targetPosition.x < currentTile.position.x)
            {
                continue;
            }
            if (currentTile.targetPosition.x == currentTile.position.x && currentTile.position.y == currentTile.targetPosition.y && (currentTile.tileTotalCost < lowestTotalCost || lowestTotalCost == -1)) {
                lowestTotalCost = currentTile.tileTotalCost;
                Debug.Log("lowestTotalCost: " + lowestTotalCost);
                currentTile.thisAction = "Finish";
                createPath(currentTile);
                break;
            }
            if (createDebugTiles)
            {
                Instantiate(debugTile, new Vector3(currentTile.position.x, currentTile.position.y, -1), Quaternion.identity);
            }
            if (canAccelerate(currentTile.speed, currentTile.position)) {
                int nextSpeed = currentTile.speed;
                nextSpeed++;
                int[] makeMovesArray = makeMoves(nextSpeed, currentTile, 0);
                nextSpeed = makeMovesArray[0];
                int nextXPosition = makeMovesArray[1];
                int nextYPosition = currentTile.position.y;
                addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Accelerate");
            }
            if (canDeaccelerate(currentTile.speed, currentTile.position)) {
                int nextSpeed = currentTile.speed;
                nextSpeed--;
                int[] makeMovesArray = makeMoves(nextSpeed, currentTile, 0);
                nextSpeed = makeMovesArray[0];
                int nextXPosition = makeMovesArray[1];
                int nextYPosition = currentTile.position.y;
                addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Deaccelerate");
            }
            if (canDoNothing(currentTile.speed, currentTile.position)) {
                int nextSpeed = currentTile.speed;
                int[] makeMovesArray = makeMoves(nextSpeed, currentTile, 0);
                nextSpeed = makeMovesArray[0];
                int nextXPosition = makeMovesArray[1];
                int nextYPosition = currentTile.position.y;
                addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Do Nothing");
            }
            if (canMoveUp(currentTile.speed, currentTile)) {
                int nextSpeed = currentTile.speed;
                int[] makeMovesArray = makeMoves(nextSpeed, currentTile, +1);
                nextSpeed = makeMovesArray[0];
                int nextXPosition = makeMovesArray[1];
                int nextYPosition = currentTile.position.y + 1;
                addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Move Up");
            }
            if (canMoveDown(currentTile.speed, currentTile)) {
                int nextSpeed = currentTile.speed;
                int[] makeMovesArray = makeMoves(nextSpeed, currentTile, -1);
                nextSpeed = makeMovesArray[0];
                int nextXPosition = makeMovesArray[1];
                int nextYPosition = currentTile.position.y - 1;
                addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Move Down");
            }
            //			for (int i = -1; i < 2; i++) {
            //				int xOffset = i;
            //				if (currentTile.position.x + xOffset < 0 || 
            //					currentTile.position.x + xOffset >= tileScript.TileArray.GetLength (0)) {
            //					continue;
            //				}
            //				for (int j = -1; j < 2; j++) {
            //					int yOffset = j;
            //					if (currentTile.position.y + yOffset < 0 || 
            //						currentTile.position.y + yOffset >= tileScript.TileArray.GetLength (1)) {
            //						continue;
            //					}
            //
            //				}
            //			}
            openlist.Sort((a, b) => a.tileTotalCost.CompareTo(b.tileTotalCost));
        }
        if (openlist.Count == 0)
        {
            Debug.Log("Openlist is empty");
        }
        if (sentinel == 0)
        {
            Debug.Log("sentinel reached zero");
        }
    }

    private int calculateDistanceToTarget(Vector2Int currentPosition, Vector2Int finishPosition) {
        return (int)((finishPosition - currentPosition).magnitude);
    }

    private bool checkClosedList(TileForClosedList tileForList) {
        //		for (int i = 0; i < closedlist.Count; i++) {
        //			if (closedlist [i].position == currentPos) {
        //				return true;
        //			}
        //		}
        if (closedlist.Contains(tileForList)) {
            return true;
        }
        return false;
    }

    private void addToOpenList(Vector2Int nextPos, int nextTileCost, int nextTileSpeed, TileForList currentTile, string previousAction) {
        int nextTileTotalCost = nextTileCost + calculateDistanceToTarget(nextPos, currentTile.targetPosition);
        TileForList newTile = new TileForList(nextPos, nextTileCost, nextTileTotalCost, nextTileSpeed, currentTile, previousAction, currentTile.targetPosition);
        if (!checkClosedList(new TileForClosedList(newTile)) && !openlist.Contains(newTile)) {
            openlist.Add(newTile);
        }
    }

    private void sortOpenList() {
        //      for (int i = 1; i < openlist.Count; i++) {
        //	currentTile = openlist [i];
        //	int j = i - 1;
        //	while (j >= 0 && currentTile.tileTotalCost < openlist [j].tileTotalCost) {
        //		openlist [j + 1] = openlist [j];
        //		j--;
        //	}
        //	openlist [j + 1] = currentTile;
        //}
        openlist.Sort((TileForList currentTile, TileForList compareTile) => currentTile.tileTotalCost.CompareTo(compareTile.tileTotalCost));
    }

    private void createPath(TileForList finishTile) {
        Debug.Log("createPath");
        int sentinel = 500;
        optimalSolution.Push(finishTile);
        if (debugMode) {
            print("Optimal Path");
            print(finishTile.ToString());
        }
        TileForList previousTile = finishTile.previousTile;
        previousTile.thisAction = finishTile.previousAction;
        while (previousTile != null && 0 < sentinel) {
            sentinel--;
            optimalSolution.Push(previousTile);
            if (debugMode) {
                print(previousTile.ToString());
            }
            TileForList nextPreviousTile;
            nextPreviousTile = previousTile.previousTile;
            if (nextPreviousTile != null) {
                nextPreviousTile.thisAction = previousTile.previousAction;
            }
            previousTile = nextPreviousTile;
        }
        if (debugMode) {
            print("End Optimal Path");
        }
    }

    private class TileForList {
        public Vector2Int position;
        public int tileCost;
        public int tileTotalCost;
        public int speed;
        public TileForList previousTile;
        public string previousAction;
        public string thisAction;
        public Vector2Int targetPosition;

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(TileForList))
            {
                TileForList objTile = (TileForList)obj;
                if (objTile.position == this.position && objTile.speed == this.speed && objTile.tileTotalCost >= this.tileTotalCost)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() + speed.GetHashCode() * 11 + tileTotalCost.GetHashCode() * 19;
        }

        public TileForList(Vector2Int position, int tileCost, int tileTotalCost, int speed,
            TileForList previousTile, string previousAction, Vector2Int targetPosition) {
            this.position = position;
            this.tileCost = tileCost;
            this.tileTotalCost = tileTotalCost;
            this.speed = speed;
            if (previousTile != null) {
                this.previousTile = previousTile;
            } else {
                this.previousTile = null;
            }
            this.previousAction = previousAction;
            this.targetPosition = targetPosition;
        }

        public TileForList(TileForList copy) : this(copy.position, copy.tileCost, copy.tileTotalCost, copy.speed,
            copy.previousTile, copy.previousAction, copy.targetPosition)
        {

        }
        public string ToString() {
            return "position: " + position + ", tileCost: " + tileCost + ", tileTotalCost: " + tileTotalCost
                + ", speed: " + speed + ", previousTile: " + previousTile + ", previousAction: " + previousAction
                + ", thisAction: " + thisAction + ", targetPosition: " + targetPosition;
        }
    }

    private class TileForClosedList : TileForList
    {
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public TileForClosedList(Vector2Int position, int tileCost, int tileTotalCost, int speed,
            TileForList previousTile, string previousAction, Vector2Int targetPosition)
            : base(position, tileCost, tileTotalCost, speed, previousTile, previousAction, targetPosition)
        {

        }

        public TileForClosedList(TileForList copy) : base(copy)
        {

        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(TileForList))
            {
                TileForList objTile = (TileForList)obj;
                if (objTile.position == this.position && objTile.speed == this.speed && objTile.tileTotalCost == this.tileTotalCost)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private bool canAccelerate(int currentSpeed, Vector2Int position) {
        if (currentSpeed < carScript.MaxSpeed)
        {
            if (currentSpeed < 0 || !tileArrayCheckOutOfBounds(1,true, true) && 0 <= currentSpeed && tileScript.TileArray[position.x + 1, position.y] != -1) {
                return true;
            }
        }
        return false;
    }

    private bool canDeaccelerate(int currentSpeed, Vector2Int position) {
        if (carScript.Minspeed < currentSpeed)
        {
            if (0 < currentSpeed || currentSpeed <= 0 && 0 < position.x && tileScript.TileArray[position.x - 1, position.y] != -1)
                return true;
        }
        return false;
    }

    private bool canDoNothing(int currentSpeed, Vector2Int position) {
        if (currentSpeed < 0 || !tileArrayCheckOutOfBounds(1, true, true) && tileScript.TileArray[position.x + 1, position.y] != -1)
        {
            if (0 <= currentSpeed || 0 < position.x && tileScript.TileArray[position.x - 1, position.y] != -1)
            {
                return true;
            }
        }
        return false;
    }

    private bool canMoveUp(int currentSpeed, TileForList currentTile) {
        Vector2Int position = currentTile.position;
        if (position.y + 1 < tileScript.TileArray.GetLength(1) && currentTile.previousAction != "Move Down")
        {
            if (currentSpeed <= carScript.MaximumSpeedForHeightChange && tileScript.TileArray[position.x, position.y + 1] != -1) {
                return true;
            }
        }
        return false;
    }

    private bool canMoveDown(int currentSpeed, TileForList currentTile) {
        Vector2Int position = currentTile.position;
        if (0 < position.y && currentTile.previousAction != "Move Up")
        {
            if (currentSpeed <= carScript.MaximumSpeedForHeightChange && tileScript.TileArray[position.x, position.y - 1] != -1) {
                return true;
            }
        }
        return false;
    }

    private int[] makeMoves(int currentSpeed, TileForList currentTile, int yPositionChange) {
        int currentXPosition = currentTile.position.x;
        int numberOfMoves = Mathf.Abs(currentSpeed);
        Vector2Int nextPos = currentTile.position;
        nextPos.y += yPositionChange;
        for (int movesDone = 0; movesDone < numberOfMoves; movesDone++) {
            Vector2Int previousPos = nextPos;
            if (0 < currentSpeed)
            {
                nextPos.x++;
            } else
            {
                nextPos.x--;
            }
            if (checkCollision(nextPos)) {
                currentSpeed = 0;
                nextPos = previousPos;
                break;
            }
        }
        return new int[] { currentSpeed, nextPos.x };
    }

    private bool checkCollision(Vector2Int position)
    {
        if (position.x < 0 || position.y < 0 || tileScript.TileArray.GetLength(0) <= position.x ||
            tileScript.TileArray.GetLength(1) <= position.y || tileScript.TileArray[position.x, position.y] == -1)
            return true;
        return false;
    }

    private TileForList getNextMove() {
        return optimalSolution.Pop();
    }

    public void nextCarAction() {
        if (debugMode) {
            print("NextCarAction");
        }
        TileForList nextMove = getNextMove();
        Debug.Log(nextMove.ToString());
        string nextAction = nextMove.thisAction;

        if (nextAction.Equals("Finish")) {
            if (debugMode) {
                print("Car has finished");
            }
            haveFinished = true;
        } else if (nextAction.Equals("Accelerate")) {
            if (debugMode) {
                print("AccelerateCar");
            }
            carScript.accelerate();
        } else if (nextAction.Equals("Deaccelerate")) {
            if (debugMode) {
                print("DeaccelerateCar");
            }
            carScript.deaccelerate();
        } else if (nextAction.Equals("Do Nothing")) {
            if (debugMode) {
                print("Do No action with car");
            }
            carScript.doNothing();
        } else if (nextAction.Equals("Move Up")) {
            if (debugMode) {
                print("Move car up");
            }
            carScript.moveUp();
        } else if (nextAction.Equals("Move Down")) {
            if (debugMode) {
                print("Move car down");
            }
            carScript.moveDown();
        } else {
            if (debugMode) {
                print("AI could not find an action with this name: " + nextAction);
            }
        }
        if (debugMode) {
            print("End NextCarAction");
        }
    }

    private bool tileArrayCheckOutOfBounds(int value, bool x, bool max)
    {
        if(x) {
            if(max)
            {
                return tileScript.TileArray.GetLength(0) <= value;
            } else
            {
                return value < 0;
            }
        }
        else
        {
            if (max)
            {
                return tileScript.TileArray.GetLength(1) <= value;
            }
            else
            {
                return value < 0;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
