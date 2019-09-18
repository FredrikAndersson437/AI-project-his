using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	[SerializeField]
	private float edgeSize = 10f; 
	[SerializeField]
	private float cameraMoveSpeed = 10f;
	[SerializeField]
	private float xMinBound = -10f;
	[SerializeField]
	private float xMaxBound = 70f;
	[SerializeField]
	private float yMinBound = -10f;
	[SerializeField]
	private float yMaxBound = 10f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.mousePosition.x > Screen.width + edgeSize && transform.position.x < xMaxBound) {
			transform.Translate(new Vector3(cameraMoveSpeed * Time.deltaTime, 0f, 0f));
		} else if (Input.mousePosition.x < edgeSize && transform.position.x > xMinBound) {
			transform.Translate(new Vector3(-cameraMoveSpeed * Time.deltaTime, 0f, 0f));
		}
		if (Input.mousePosition.y > Screen.height + edgeSize && transform.position.y < yMaxBound) {
			transform.Translate(new Vector3(0f, cameraMoveSpeed * Time.deltaTime, 0f));
		} else if (Input.mousePosition.y < edgeSize && transform.position.y > yMinBound) {
			transform.Translate(new Vector3(0f, -cameraMoveSpeed * Time.deltaTime, 0f));
		}
	}
}
