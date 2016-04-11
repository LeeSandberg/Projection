using UnityEngine;
using System.Collections;

public class CustomerAgent : MonoBehaviour {
    private NavMeshAgent agent = null;


    public float smoothing = 7f;

    public Vector3 Target
    {
        get { return target; }  // Why tagrget and not Target?

        // Lite magi...detta är smart att starta coroutiner inne från set på en variabel.
        // Sparar mycket prestanda. Märk att den ser först till att stoppa eventuell samma coruntiner som kör först.
        set
        {
            target = value; // Vart kom value i från? Kanske från Vector3 typen?

            agent.destination = target;



            //    StopCoroutine("Movement");  // Stop kan bara användas om coroutinen startarts som en sträng dvs "Movement".
            //    StartCoroutine("Movement", target);
        }

    }

    private Vector3 target;  // Detta är inte samma variable som Target...med stort T.

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        //Debug.Log ("Awake started");


        //Debug.Log ("onAwake ended");
    }



    IEnumerator Movement(Vector3 target) // Lite förvirrande att paramentern heter target också.
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target, smoothing * Time.deltaTime);

            yield return null;
        }

        //yield return new WaitForSeconds(2);


    }

}