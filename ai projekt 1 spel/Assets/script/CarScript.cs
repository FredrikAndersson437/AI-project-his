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
    public int Minspeed
    {
        get { return minSpeed; }
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
        else
        {
            Debug.Log("Car not allowed to accelerate");
        }
    }

	public void deaccelerate() {
		if (minSpeed < currentSpeed && !haveDoneTurn) {
			currentSpeed--;
			haveDoneTurn = true;
		}
        else
        {
            Debug.Log("Car not allowed to deaccelerate");
        }
    }

	public void doNothing() {
        if (!haveDoneTurn)
        {
            haveDoneTurn = true;
        }
	}

	public void moveUp() {
        Vector3 previousPos = transform.position;
		if (transform.position.y + 1 < tileScript.TileArray.GetLength(1) && currentSpeed <= maximumSpeedForHeightChange && !haveDoneTurn) {
			transform.Translate(new Vector3 (0f, 1f, 0f));
            if(checkCollision())
            {
                transform.position = previousPos;
            }
            haveDoneTurn = true;
		}
        else
        {
            Debug.Log("Car not allowed to move up");
        }
	}

	public void moveDown() {
        Vector3 previousPos = transform.position;
        if (0 < transform.position.y && currentSpeed <= maximumSpeedForHeightChange && !haveDoneTurn) {
			transform.Translate(new Vector3 (0f, -1f, 0f));
            if (checkCollision())
            {
                transform.position = previousPos;
            }
            haveDoneTurn = true;
		}
        else
        {
            Debug.Log("Car not allowed to move down");
        }
    }

    public bool checkCollision()
    {
        if(tileScript.TileArray[(int)transform.position.x, (int)transform.position.y] == -1)
        {
            return true;
        }
        return false;
    }

	public void moveCar() {
		if (haveDoneTurn) {
            int numberOfMoves = Mathf.Abs(currentSpeed);
            Debug.Log(numberOfMoves);
            for (int moves = 0; moves < numberOfMoves; moves++)
            {
                Vector3 previousPos = transform.position;
                if (0 < currentSpeed)
                {
                    transform.Translate(new Vector3(1f, 0f, 0f));
                } else
                {
                    transform.Translate(new Vector3(-1f, 0f, 0f));
                }
                if(checkCollision())
                {
                    currentSpeed = 0;
                    transform.position = previousPos;
                    break;
                }
            }
			haveDoneTurn = false;
		}
        else
        {
            Debug.Log("Car has not done its turn");
        }
	}
}
