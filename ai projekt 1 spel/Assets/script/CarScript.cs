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
	[SerializeField]
	private int maximumSpeedForHeightChange = 2;

	public TileScript tileScript;

	public int CurrentSpeed {
		get{ return currentSpeed;}
	}
	public int MaxSpeed {
		get{ return maxSpeed;}
	}
	public int MaximumSpeedForHeightChange {
		get{ return maximumSpeedForHeightChange;}
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

	public void moveUp() {
		if (transform.position.y + 1 < tileScript.TileArray.GetLength(1) && currentSpeed <= maximumSpeedForHeightChange && !haveDoneTurn) {
			transform.Translate(new Vector3 (0f, 1f, 0f));
			haveDoneTurn = true;
		}
	}

	public void moveDown() {
		if (0 < transform.position.y && currentSpeed <= maximumSpeedForHeightChange && !haveDoneTurn) {
			transform.Translate(new Vector3 (0f, -1f, 0f));
			haveDoneTurn = true;
		}
	}

	public void moveCar() {
		if (haveDoneTurn) {
			transform.Translate(new Vector3 (currentSpeed*1f, 0f, 0f));
			haveDoneTurn = false;
		}
	}
}
