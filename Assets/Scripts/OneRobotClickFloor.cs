using UnityEngine;
using System.Collections;

public class OneRobotClickFloor : MonoBehaviour {


    private GameObject go1 = null;
    private MoveTo sn1 = null;

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
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)){
			sn1.Target = hit.point;
			//agent.SetDestination(hit.point);
			}

		}
	}
}