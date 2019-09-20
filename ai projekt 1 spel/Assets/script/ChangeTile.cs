using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTile : MonoBehaviour {

	[SerializeField]
	private bool isObstacle = false;
	[SerializeField]
	private TileScript tileScript;
	// Use this for initialization

	public TileScript TileScript {
		set{tileScript = value;}
	}

    public bool IsObstacle
    {
        set { isObstacle = value; }
    }


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void changeTile() {
		if (isObstacle) {
			isObstacle = false;
			tileScript.changeTileStatus (new Vector2Int ((int)transform.position.x, (int)transform.position.y), 1);
			GetComponent<SpriteRenderer> ().color = Color.white;
		} else {
			isObstacle = true;
			tileScript.changeTileStatus (new Vector2Int ((int)transform.position.x, (int)transform.position.y), -1);
			GetComponent<SpriteRenderer> ().color = Color.black;
		}
	}
}
