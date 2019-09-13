using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private int turn;
	[SerializeField]
	private GameObject[] carArray = new GameObject[1];

	// Use this for initialization
	void Start () {
		StartCoroutine (test ());
	}
		
	// Update is called once per frame
	void Update () {
		
	}

	public void moveCars () {
		for (int i = 0; i < carArray.Length; i++) {
			GameObject currentCar = carArray [i];
			int currentSpeed = currentCar.GetComponent<CarScript> ().CurrentSpeed;
			currentCar.transform.Translate(new Vector3 (currentSpeed*1f, 0f, 0f));
		}
	}

	public IEnumerator test() {
		while (true) {
			yield return new WaitForSeconds (1f);
			moveCars ();
			print ("Move Cars");
		}
	}
}
