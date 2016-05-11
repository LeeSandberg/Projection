using UnityEngine;
using System.Collections;

public class ClickWallOneRobot : MonoBehaviour
{
	public int angle = 0;
	//public int angleSpeed = 5;
	public int maxHorizontalKeystone = 40;
	public int maxVerticalKeystone = 40;
	public float Dopt = 2;
	public float robotHeight = 1;
    private GameObject go1 = null;
    private MoveTo sn1 = null;
    private Vector3 projectionLocation;

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //generates a ray using input from mouse for direction (i believe the local origin(camera view) is used for origin)
            Debug.DrawRay(ray.origin, ray.direction, Color.green, 5f);     //in order to see the ray casted for debugging
            int layerMask = 1 << 8;                                         //this is bit shifting in order to mask or use particular layers only - used for the raycast below (walls declared as 8th layer)
            if (Physics.Raycast(ray, out hit, 100f, layerMask))             //ray as input(or origin,direction can be used, hit for output, 100 is reach of ray, layerMask indicates which layer the ray is used in (walls here)
            {
					

				projectionLocation = hit.point;
				Vector3 projectionDirection = Quaternion.AngleAxis (angle, transform.up)*hit.normal;
				Ray perpendicular = new Ray (hit.point, projectionDirection);
				Debug.DrawRay(projectionLocation, projectionDirection, Color.blue,5f);

				float D = Mathf.Sqrt((Dopt*Dopt) - Mathf.Pow((projectionLocation.y-robotHeight),2));		//to account for height of projection.
				Vector3 projectorLocation = perpendicular.GetPoint (D);

				//complete algorithm here


				sn1.Target = projectorLocation;                               
            }

        }
    }
}