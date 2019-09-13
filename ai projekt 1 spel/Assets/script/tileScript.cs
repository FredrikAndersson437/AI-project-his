using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour {
	[SerializeField]
	protected int[,] tileArray;
	[SerializeField]
	private int finishTile = 0;

	public int[,] TileArray {
		get { return tileArray; }
	}

	public int FinishTile {
		get { return finishTile; }
	}
}
