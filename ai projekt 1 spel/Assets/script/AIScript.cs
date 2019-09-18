using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour {

	[SerializeField]
	private CarScript carScript;
	[SerializeField]
	private TileScript tileScript;

	[SerializeField]
	private List<TileForList> openlist = new List<TileForList>();
	[SerializeField]
	private HashSet<TileForList> closedlist = new HashSet<TileForList>();

	[SerializeField]
	private Stack<TileForList> optimalSolution = new Stack<TileForList>();

	private Vector2Int carPos;

	[SerializeField]
	private bool debugMode = false;

	private bool haveFinished = false;

	private int lowestTotalCost;

	// Use this for initialization

	public bool HaveFinished {
		get {return haveFinished;}
	}

	void Start () {
	}

	public void prepareAI() {
		carPos = new Vector2Int ((int)carScript.transform.position.x, (int)carScript.transform.position.y);
		lowestTotalCost = -1;
		for (int i = 0; i < tileScript.FinishTile.GetLength (0); i++) {
			Vector2Int targetPosition = tileScript.FinishTile [i];
			TileForList startTile = new TileForList (carPos, 0, calculateDistanceToTarget (carPos, targetPosition), carScript.CurrentSpeed, null, "Start Tile", targetPosition);
			if (debugMode) {
				print (startTile.ToString ());
			}
			openlist.Clear ();
			closedlist.Clear ();
			openlist.Add (startTile);
			findOptimalPath ();
		}
	}

	private void findOptimalPath() {
		int sentinel = 500;
		while (0 < openlist.Count && 0 < sentinel ) {
			print ("sentinel: " + sentinel);
			sentinel--;
			TileForList currentTile = openlist [0];
			openlist.RemoveAt (0);
			if (debugMode) {
				print ("currentTile: " + currentTile.ToString ());
			}
			if (currentTile.position == currentTile.targetPosition && (currentTile.tileTotalCost < lowestTotalCost || lowestTotalCost == -1)) {
				lowestTotalCost = currentTile.tileTotalCost;
				currentTile.thisAction = "Finish";
				createPath (currentTile);
				break;
			}
			closedlist.Add (currentTile);
			if(canAccelerate(currentTile.speed)) {
				int nextSpeed = currentTile.speed;
				nextSpeed++;
				int [] makeMovesArray = makeMoves(nextSpeed, currentTile, 0);
				nextSpeed = makeMovesArray [0];
				int nextXPosition = makeMovesArray [1];
				int nextYPosition = currentTile.position.y;
				addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Accelerate");
			}
			if (canDeaccelerate (currentTile.speed)) {
				int nextSpeed = currentTile.speed;
				nextSpeed--;
				int [] makeMovesArray = makeMoves(nextSpeed, currentTile, 0);
				nextSpeed = makeMovesArray [0];
				int nextXPosition = makeMovesArray [1];
				int nextYPosition = currentTile.position.y;
				addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Deaccelerate");
			}
			if(canDoNothing()) {
				int nextSpeed = currentTile.speed;
				int [] makeMovesArray = makeMoves(nextSpeed, currentTile, 0);
				nextSpeed = makeMovesArray [0];
				int nextXPosition = makeMovesArray [1];
				int nextYPosition = currentTile.position.y;
				addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Do Nothing");
			}
			if (canMoveUp (currentTile.speed, currentTile.position.y)) {
				int nextSpeed = currentTile.speed;
				int [] makeMovesArray = makeMoves(nextSpeed, currentTile, +1);
				nextSpeed = makeMovesArray [0];
				int nextXPosition = makeMovesArray [1];
				int nextYPosition = currentTile.position.y + 1;
				addToOpenList(new Vector2Int(nextXPosition, nextYPosition), currentTile.tileCost + 1, nextSpeed, currentTile, "Move Up");
			}
			if (canMoveDown(currentTile.speed, currentTile.position.y)) {
				int nextSpeed = currentTile.speed;
				int [] makeMovesArray = makeMoves(nextSpeed, currentTile, -1);
				nextSpeed = makeMovesArray [0];
				int nextXPosition = makeMovesArray [1];
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
			openlist.Sort ((a, b) => a.tileTotalCost.CompareTo(b.tileTotalCost));
		}
	}

	private int calculateDistanceToTarget(Vector2Int currentPosition, Vector2Int finishPosition) {
		return (int)((finishPosition - currentPosition).magnitude);
	}

	private bool checkClosedList(TileForList tileForList) {
//		for (int i = 0; i < closedlist.Count; i++) {
//			if (closedlist [i].position == currentPos) {
//				return true;
//			}
//		}
		if (closedlist.Contains (tileForList)) {
			return true;
		}
		return false;
	}

	private void addToOpenList(Vector2Int nextPos, int nextTileCost, int nextTileSpeed, TileForList currentTile, string previousAction) {
		int nextTileTotalCost = nextTileCost + calculateDistanceToTarget (nextPos, currentTile.targetPosition);
		TileForList newTile = new TileForList (nextPos, nextTileCost, nextTileTotalCost, nextTileSpeed, currentTile, previousAction, currentTile.targetPosition);
		if(!checkClosedList(newTile)) {
			openlist.Add(newTile);
		}
	}

	private void sortOpenList() {
		TileForList currentTile;
		for (int i = 1; i < openlist.Count; i++) {
			currentTile = openlist [i];
			int j = i - 1;
			while (j >= 0 && currentTile.tileTotalCost < openlist [j].tileTotalCost) {
				openlist [j + 1] = openlist [j];
				j--;
			}
			openlist [j + 1] = currentTile;
		}
	}

	private void createPath(TileForList finishTile) {
		int sentinel = 500;
		optimalSolution.Push (finishTile);
		if (debugMode) {
			print ("Optimal Path");
			print (finishTile.ToString ());
		}
		TileForList previousTile = finishTile.previousTile;
		previousTile.thisAction = finishTile.previousAction;
		while (previousTile != null && 0 < sentinel) {
			sentinel--;
			optimalSolution.Push (previousTile);
			if (debugMode) {
				print (previousTile.ToString ());
			}
			TileForList nextPreviousTile;
			nextPreviousTile = previousTile.previousTile;
			if (previousTile.previousTile != null) {
				nextPreviousTile.thisAction = previousTile.previousAction;
			}
			previousTile = nextPreviousTile;
		}
		if (debugMode) {
			print ("End Optimal Path");
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
		public TileForList(Vector2Int position, int tileCost, int tileTotalCost, int speed,
			TileForList previousTile, string previousAction, Vector2Int targetPosition) {
			this.position = position;
			this.tileCost = tileCost;
			this.tileTotalCost = tileTotalCost;
			this.speed = speed;
			if(previousTile != null) {
			this.previousTile = previousTile;
			} else {
				this.previousTile = null;
			}
			this.previousAction = previousAction;
			this.targetPosition = targetPosition;
		}
		public string ToString() {
			return "position: " +  position + ", tileCost: " + tileCost + ", tileTotalCost: " + tileTotalCost
				+ ", speed: " + speed + ", previousTile: " + previousTile + ", previousAction: " + previousAction 
				+ ", thisAction: " +  thisAction + ", targetPosition: " +  targetPosition;
		}
	}

	private bool canAccelerate(int currentSpeed) {
		if (currentSpeed < carScript.MaxSpeed)
			return true;
		return false;
	}

	private bool canDeaccelerate(int currentSpeed) {
		if (0 < currentSpeed)
			return true;
		return false;
	}

	private bool canDoNothing() {
		return true;
	}

	private bool canMoveUp(int currentSpeed, int currentY) {
		if (currentSpeed <= carScript.MaximumSpeedForHeightChange) {
			if (currentY + 1< tileScript.TileArray.GetLength (1)) {
				return true;
			}
		}
		return false;
	}

	private bool canMoveDown(int currentSpeed, int currentY) {
		if (currentSpeed <= carScript.MaximumSpeedForHeightChange) {
			if (0 < currentY) {
				return true;
			}
		}
		return false;
	}

	private int[] makeMoves(int currentSpeed, TileForList currentTile, int yPositionChange) {
		int currentXPosition = currentTile.position.x;
		for (int movesDone = 0; movesDone < currentSpeed; movesDone++) {
			if (tileScript.TileArray [currentXPosition + 1, currentTile.position.y + yPositionChange] == -1) {
				currentSpeed = 0;
				break;
			} else {
				currentXPosition++;
			}
		}
		return new int[] { currentSpeed, currentXPosition };
	}

	private TileForList getNextMove() {
		return optimalSolution.Pop ();
	}

	public void nextCarAction() {
		if(debugMode) {
			print ("NextCarAction");
		}
		string nextAction = getNextMove ().thisAction;

		if (nextAction.Equals ("Finish")) {
			if (debugMode) {
				print ("Car has finished");
			}
			haveFinished = true;
		} else if (nextAction.Equals ("Accelerate")) {
			if (debugMode) {
				print ("AccelerateCar");
			}
			carScript.accelerate ();
		} else if (nextAction.Equals ("Deaccelerate")) {
			if (debugMode) {
				print ("DeaccelerateCar");
			}
			carScript.deaccelerate ();
		} else if (nextAction.Equals ("Do Nothing")) {
			if (debugMode) {
				print ("Do No action with car");
			}
			carScript.doNothing ();
		} else if (nextAction.Equals ("Move Up")) {
			if (debugMode) {
				print ("Move car up");
			}
			carScript.moveUp ();
		} else if (nextAction.Equals ("Move Down")) {
			if (debugMode) {
				print ("Move car down");
			}
			carScript.moveDown ();
		} else {
			if (debugMode) {
				print ("AI could not find an action with this name: " + nextAction);
			}
		}
		if(debugMode) {
			print ("End NextCarAction");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
