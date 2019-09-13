using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CreateLine : TileScript {
	[SerializeField]
	private int numberOfTiles = 0;
	[SerializeField]
	private GameObject tileObject;
	[SerializeField]
	private GameObject finishTileObject;
	// Use this for initialization
	void Start () {
		Assert.IsFalse (numberOfTiles < finishTile || finishTile < 0);
		tileArray = new int[numberOfTiles,1];
		for (int i = 0; i < numberOfTiles; i++) {
			if (i != finishTile) {
				GameObject obj = Instantiate (tileObject, new Vector3 (1f * i, 0f, 0f), Quaternion.identity);
				tileArray [i,0] = 1;
			} else {
				GameObject obj = Instantiate (finishTileObject, new Vector3 (1f * i, 0f, 0f), Quaternion.identity);
				tileArray [i,0] = 2;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
