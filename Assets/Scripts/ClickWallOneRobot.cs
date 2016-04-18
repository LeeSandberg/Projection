using UnityEngine;
using System.Collections;

public class ClickWallOneRobot : MonoBehaviour
{


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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //generates a ray using input from mouse for direction (i believe the local origin(camera view) is used for origin)
            Debug.DrawRay(ray.origin, ray.direction, Color.green, 15f);     //in order to see the ray casted for debugging
            int layerMask = 1 << 8;                                         //this is bit shifting in order to mask or use particular layers only - used for the raycast below (walls declared as 8th layer)
            if (Physics.Raycast(ray, out hit, 100f, layerMask))             //ray as input(or origin,direction can be used, hit for output, 100 is reach of ray, layerMask indicates which layer the ray is used in (walls here)
            {
                sn1.Target = hit.point;
            }

        }
    }
}