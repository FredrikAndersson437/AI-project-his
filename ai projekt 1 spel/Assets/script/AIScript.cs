using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour {

	[SerializeField]
	private CarScript carScript;
	[SerializeField]
	private TileScript tileScript;

	[SerializeField]
	private List<TileForList> openlist;
	[SerializeField]
	private HashSet<TileForList> closedlist;

	private Stack<Vector2> optimalSolution;

	private Vector2 finishPosition;
	private Vector2Int carPos;

	// Use this for initialization
	void Start () {
		carPos = new Vector2Int (carScript.transform.position.x, carScript.transform.position.y);
		openlist.Add (carPos);
		finishPosition = new Vector2 (tileScript.FinishTile, 0);
	}

	void findOptimalPath() {
		while (0 < openlist.Count) {
			TileForList currentTile = openlist [0];
			openlist.RemoveAt (0);
			if (currentTile.position == finishPosition) {
				createPath (currentTile);
			}
			closedlist.Add (currentTile);
			for (int i = -1; i < 2; i++) {
				int xOffset = i;
				if (currentTile.position.x + xOffset < 0 || currentTile.position.x + xOffset > tileScript.TileArray.GetLength (0)) {
					continue;
				}
				for (int j = -1; j < 2; j++) {
					int yOffset = j;
					if (currentTile.position.y + ýOffset < 0 || currentTile.position.y + yOffset > tileScript.TileArray.GetLength (1)) {
						continue;
					}
					Vector2 currentPos = currentTile.position;

					if(!checkClosedList(currentPos)) {
					Vector2 nextPos = new Vector2 (currentPos ().x + xOffset, currentPos ().y + yOffset);
					int nextTileCost = currentTile.tileCost + 1;
					int nextTileTotalCost = nextTileCost + calculateDistanceToTarget (nextPos);
					int nextTileSpeed = carScript.CurrentSpeed;
						openlist.Add(new TileForList(nextPos, nextTileCost, nextTileTotalCost, nextTileSpeed, currentTile));
					}
				}
			}
			openlist.Sort ((a, b) => a.tileTotalCost.CompareTo(b.tileTotalCost));
		}
	}

	private int calculateDistanceToTarget(Vector2 currentPosition) {
		return (finishPosition - currentPosition).magnitude;
	}

	private bool checkClosedList(Vector2Int currentPos) {
		for (int i = 0; i < closedlist.Count; i++) {
			if (closedlist [i].position == currentPos) {
				return true;
			}
		}
		return false;
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
		optimalSolution.Push (finishTile);
		TileForList previousTile = finishTile.previousTile;
		while (previousTile.position != carPos) {
			optimalSolution.Push (previousTile);
		}
	}

	private class TileForList {
		public Vector2Int position;
		public int tileCost;
		public int tileTotalCost;
		public int speed;
		public TileForList previousTile;
		TileForList(Vector2Int position, int tileCost, int tileTotalCost, int speed, TileForList previousTile) {
			this.position = position;
			this.tileCost = tileCost;
			this.tileTotalCost = tileTotalCost;
			this.speed = speed;
			this.previousTile = previousTile;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
