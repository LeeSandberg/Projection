using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

    public float rotationSpeed = 5f;

	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("space")){
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
        }
    }
}
