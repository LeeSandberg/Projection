﻿using UnityEngine;
using System.Collections;

public static class TickTack
{
    public static bool EveryOtherClick = false;

    public static bool GlobalValue
    {
        get
        {
            return EveryOtherClick;
        }
        set
        {
            EveryOtherClick = value;
        }
    }

}




public class ClickFloor : MonoBehaviour
{
    private GameObject go1 = null;
    private GameObject go2 = null;
    private SmartNavmeshAgent sn1 = null;
    private SmartNavmeshAgent sn2 = null;


    void Awake()
    {
        TickTack.EveryOtherClick = false;

        Debug.Log("Awake in ClickFloors started");
        go1 = GameObject.Find("NavMeshRobot1");
        go2 = GameObject.Find("NavMeshRobot2");

        if (go1 != null && go2 != null)
        {
            Debug.Log("MyCylinder was found using Find.");
            sn1 = go1.GetComponent <SmartNavmeshAgent>();
            sn2 = go2.GetComponent <SmartNavmeshAgent>();
            Debug.Log("Tried to get sn SmartNavmeshAgent!");
        }
        else
        {
            Debug.Log("find couldn't find anything!!!!");

        }

        //other = (PropertiesAndCoroutines) go.GetComponent(typeof(PropertiesAndCoroutines));
        Debug.Log("onAwake ended");
    }


    void Update()
    {
        //Debug.Log ("Running...");

        if (Input.GetMouseButtonDown(0))
        {
            System.Console.WriteLine("Entered game settings menu.");
            Debug.Log("Mouse button down next make ray.");

            Debug.Log("Camera name:");
            Debug.Log(Camera.main.name);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.Log("Now try physics and Raycast to get RaycastHit...");

            RaycastHit hit;

            bool result = Physics.Raycast(ray, out hit);

            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);

            if (result == false)
            {
                Debug.Log("We click on nothing!");
            }
            else
            {
                Debug.Log("OK physics worked and we clicked on something, see if we got the plane..");

                if (hit.collider.gameObject == gameObject) 
                {
                     Debug.Log ("hit!");
                    if (TickTack.EveryOtherClick == false)
                    {
                        if (sn1 == null)
                        {
                            Debug.Log("Couldn't get script sn == null !!!");
                        }
                        else
                        {
                            Debug.Log("Found script! Using it!");
                            sn1.Target = hit.point;
                            Debug.Log("Success!!");
                        }

                        TickTack.EveryOtherClick = true;
                    }
                    else
                    {
                        if (sn2 == null)
                        {
                            Debug.Log("Couldn't get script sn == null !!!");
                        }
                        else
                        {
                            Debug.Log("Found script! Using it!");
                            sn2.Target = hit.point;
                            Debug.Log("Success!!");
                        }

                        TickTack.EveryOtherClick = false;
                    }

                }
            }
        }
    }
}

