using UnityEngine;
using System.Collections;

public class ClickWallOneRobot : MonoBehaviour {

	public float distance = 10f;
	private GameObject go1 = null;
	private MoveTo sn1 = null;
	private Vector3 temp;
	void Awake()
	{

		go1 = GameObject.Find("Robot01");

		if (go1 != null)
		{
			sn1 = go1.GetComponent<MoveTo>();
		}
		else
		{
			Debug.Log("find couldn't find anything!!!!");

		}

	}

	void Update()
	{

		RaycastHit hit;
		Ray targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		//Debug.DrawRay(transform.position, Vector3(0,-1,0
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(targetRay, out hit)){
				if (hit.collider.tag == "walls") {
					//temp.x = hit.point.x - distance;
					//temp.y = hit.point.y - distance;
					//temp.z = hit.point.z - distance;
				
					sn1.Target = hit.point;
				//agent.SetDestination(hit.point);
				}
			}
		}
	}
}