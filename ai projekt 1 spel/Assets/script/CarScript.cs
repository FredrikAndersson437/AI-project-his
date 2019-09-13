using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour {
	[SerializeField]
	private int maxSpeed = 3;
	[SerializeField]
	private int minSpeed = 0;
	[SerializeField]
	private int currentSpeed = 0;

	public int CurrentSpeed {
		get{ return currentSpeed;}
	}

	private bool haveDoneTurn = false;
	// Use this for initialization

	public void accelerate() {
		if (currentSpeed < maxSpeed && !haveDoneTurn) {
			currentSpeed++;
			haveDoneTurn = true;
		}
	}

	public void deaccelerate() {
		if (minSpeed < currentSpeed && !haveDoneTurn) {
			currentSpeed--;
			haveDoneTurn = true;
		}
	}

	public void doNothing() {
		haveDoneTurn = true;
	}
}
