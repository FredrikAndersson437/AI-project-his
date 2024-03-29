﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileScript : MonoBehaviour {
	[SerializeField]
	protected int[,] tileArray;
	[SerializeField]
	protected Vector2Int[] finishTile;
	[SerializeField]
	protected int finishDistance;

	public int[,] TileArray {
		get { return tileArray; }
	}

	public Vector2Int[] FinishTile {
		get { return finishTile; }
	}

    public int FinishDistance
    {
        get { return finishDistance; }
    }

    public void changeTileStatus (Vector2Int position, int tileStatusChange) {
		tileArray [position.x, position.y] = tileStatusChange;
	}

	public abstract void createTrack ();

    public abstract void createTrack(string fileName);
}
