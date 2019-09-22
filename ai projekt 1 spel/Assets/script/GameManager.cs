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
    private GameObject carHolder;
	private List<GameObject> carList = new List<GameObject>();
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
    [SerializeField]
    private bool testingAi = false;

    private bool gameHaveFinished = false;

    [SerializeField]
    private PlayerScript playerScript;

    private List<GameObject> finishList = new List<GameObject>();

    [SerializeField]
    private Text victoryText;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Canvas difficultyButtons;

    public bool GameHasStarted {
		get{return gameHasStarted;}
	}

	// Use this for initialization
	void Start () {
        victoryText.gameObject.SetActive(false);
        if (useCreatedTrack)
        {
            tileScript.createTrack(trackName);
        }
        else
        {
            tileScript.createTrack();
        }
        for (int i = 0; i < carHolder.transform.childCount; i++)
        {
            carList.Add(carHolder.transform.GetChild(i).gameObject);
        }
    }

    public void saveTrack()
    {
        File.WriteAllText("Track", JsonConvert.SerializeObject(tileScript.TileArray));
    }

	public void startGame() {
		if (!gameHasStarted) {
            difficultyButtons.gameObject.SetActive(false);
            gameHasStarted = true;
			for (int i = 0; i < aiPlayers.Length; i++) {
				aiPlayers [i].prepareAI ();
			}
            if (testingAi)
            {
                StartCoroutine(test());
            }
            else
            {
                playerScript.activatePlayer();
                StartCoroutine(trueGame());
            }
		}
        startButton.gameObject.SetActive(false);
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
        for (int i = 0; i < carList.Count; i++) {
			GameObject currentCar = carList[i];
			currentCar.GetComponent<CarScript> ().moveCar ();
		}
        checkIfFinished();
    }

    public IEnumerator trueGame()
    {
        while(!gameHaveFinished)
        {
            yield return new WaitForSeconds (1f);
            if(!playerScript.PlayerIsActive)
            {
                for (int i = 0; i < aiPlayers.Length; i++)
                {
                    if (!aiPlayers[i].HaveFinished)
                    {
                        aiPlayers[i].nextCarAction();
                    }
                }
                    moveCars();
                playerScript.activatePlayer();
            }
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

    private void checkIfFinished()
    {
        for(int i = 0; i < carList.Count; i++)
        {
            if (tileScript.FinishDistance <= carList[i].transform.position.x)
            {
                gameHaveFinished = true;
                finishList.Add(carList[i].gameObject);
            }
        }
        if(gameHaveFinished)
        {
            printVictoryText();
        }
    }

    private void printVictoryText()
    {
        if(finishList.Count == 1)
        {
            victoryText.text = finishList[0].name + " Won!";
        }
        else
        {
            victoryText.text = "";
            for (int i = 0; i < finishList.Count; i++)
            {
                victoryText.text += finishList[i].name;
                if(i+1 != finishList.Count)
                {
                    victoryText.text += " and ";
                }
            }
            victoryText.text += " Won!";
        }
        victoryText.gameObject.SetActive(true);
    }
}
