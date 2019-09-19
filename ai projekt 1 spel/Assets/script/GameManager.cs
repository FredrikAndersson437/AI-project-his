using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour {

	private int turn;
	[SerializeField]
	private GameObject[] carArray = new GameObject[2];
	[SerializeField]
	private AIScript[] aiPlayers = new AIScript[2];
	[SerializeField]
	TileScript tileScript;

	bool AIPlayersFinished = false;
	private bool gameHasStarted = false;

	public bool GameHasStarted {
		get{return gameHasStarted;}
	}

	// Use this for initialization
	void Start () {
		tileScript.createTrack ();
	}

	public void startGame() {
		if (!gameHasStarted) {
			gameHasStarted = true;
			for (int i = 0; i < aiPlayers.Length; i++) {
				aiPlayers [i].prepareAI ();
			}
			StartCoroutine (test ());
		}
	}

	public void changeTile() {
		if(Input.GetMouseButtonDown(0)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hit.collider != null && hit.collider.gameObject.GetComponent<ChangeTile>() != null) {
				hit.collider.gameObject.GetComponent<ChangeTile> ().changeTile ();
			}
		}
	}
		
	// Update is called once per frame
	void Update () {
		changeTile ();
	}

	public void moveCars () {
		for (int i = 0; i < carArray.Length; i++) {
			GameObject currentCar = carArray [i];
			currentCar.GetComponent<CarScript> ().moveCar ();
		}
	}

	public IEnumerator test() {
		while (!AIPlayersFinished) {
			yield return new WaitForSeconds (1f);
			AIPlayersFinished = true;
			for (int i = 0; i < aiPlayers.Length; i++) {
				if (!aiPlayers[i].HaveFinished) {
					AIPlayersFinished = false;
					aiPlayers[i].nextCarAction ();
				}
			}
			moveCars ();
			print ("Move Cars");
		}
		print ("Ai player have finished");
	}
}
