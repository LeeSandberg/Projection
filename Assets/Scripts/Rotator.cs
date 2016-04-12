using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public float rotationSpeed = 45f;

	public bool turn = false;


	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("r"))
		{
			turn = !turn;
		}

		if (turn) { 
			transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
		}
	}
}
