using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {

    private NavMeshAgent agent = null;
    public float smoothing = 7f;

    public Vector3 Target
    {
        get {
            return target;
        }  
        set {
            target = value;
            agent.destination = target;
        }

    }
    private Vector3 target;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

}






