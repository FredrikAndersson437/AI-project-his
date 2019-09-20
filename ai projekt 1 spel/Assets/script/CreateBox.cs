using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using System.IO;

public class CreateBox : TileScript {
	[SerializeField]
	private int numberOfTilesWidth = 0;
	[SerializeField]
	private int numberOfTilesHeight = 0;
	[SerializeField]
	private GameObject tileObject;
	[SerializeField]
	private GameObject finishTileObject;
	// Use this for initialization
	void Start () {
	}

	public override void createTrack() {
		Assert.IsFalse (numberOfTilesWidth < finishDistance || finishDistance < 0 || numberOfTilesHeight < 1);
		tileArray = new int[numberOfTilesWidth,numberOfTilesHeight];
		finishTile = new Vector2Int[numberOfTilesHeight];
		for (int i = 0; i < numberOfTilesWidth; i++) {
			for (int j = 0; j < numberOfTilesHeight; j++) {
				if (i != finishDistance) {
					GameObject obj = Instantiate (tileObject, new Vector3 (1f * i, 1f * j, 0f), Quaternion.identity);
					obj.GetComponent<ChangeTile> ().TileScript = this;
					tileArray [i, j] = 1;
				} else {
					GameObject obj = Instantiate (finishTileObject, new Vector3 (1f * i, 1f * j, 0f), Quaternion.identity);
					tileArray [i, j] = 2;
					finishTile [j] = new Vector2Int (i, j);
				}
			}
		}
	}

    public override void createTrack(string fileName)
    {
        finishTile = new Vector2Int[numberOfTilesHeight];
        tileArray = JsonConvert.DeserializeObject<int[,]>(File.ReadAllText(fileName));
        for(int i = 0; i < tileArray.GetLength(0); i++)
        {
            for(int j = 0; j < tileArray.GetLength(1); j++)
            {
                if(tileArray[i,j] == 1)
                {
                    GameObject obj = Instantiate(tileObject, new Vector3(1f * i, 1f * j, 0f), Quaternion.identity);
                    obj.GetComponent<ChangeTile>().TileScript = this;
                }
                else if (tileArray[i, j] == -1)
                {
                    GameObject obj = Instantiate(tileObject, new Vector3(1f * i, 1f * j, 0f), Quaternion.identity);
                    obj.GetComponent<ChangeTile>().GetComponent<SpriteRenderer>().color = Color.black;
                    obj.GetComponent<ChangeTile>().IsObstacle = true;
                    obj.GetComponent<ChangeTile>().TileScript = this;
                }
                else if(tileArray[i, j] == 2)
                {
                    GameObject obj = Instantiate(finishTileObject, new Vector3(1f * i, 1f * j, 0f), Quaternion.identity);
                    finishTile[j] = new Vector2Int(i, j);
                }
            }
        }
    }



	// Update is called once per frame
	void Update () {

	}
}

