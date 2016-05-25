using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	private Vector3 projectionTarget;
	// Update is called once per frame
	void Update () {

		RaycastHit hit;
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			int layerMask = 1 << 8;
			if (Physics.Raycast (ray, out hit, 100f, layerMask)) {
				projectionTarget = hit.point;
			}
		}
		transform.LookAt (projectionTarget);
	}
}
