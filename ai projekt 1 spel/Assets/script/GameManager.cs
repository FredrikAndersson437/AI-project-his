using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

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

    [SerializeField]
    private bool useCreatedTrack;
    [SerializeField]
    private string trackName;

    [SerializeField]
    private int turnCounter = 0;
    [SerializeField]
    private Text turnCounterText;

	public bool GameHasStarted {
		get{return gameHasStarted;}
	}

	// Use this for initialization
	void Start () {
        if (useCreatedTrack)
        {
            tileScript.createTrack(trackName);
        }
        else
        {
            tileScript.createTrack();
        }
	}

    public void saveTrack()
    {
        File.WriteAllText("Track", JsonConvert.SerializeObject(tileScript.TileArray));
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
		if(!gameHasStarted && Input.GetMouseButtonDown(0)) {
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
        turnCounter++;
        turnCounterText.text = "Turn: " + turnCounter;
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
