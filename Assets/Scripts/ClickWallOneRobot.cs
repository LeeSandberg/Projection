using UnityEngine;
using System.Collections;

public class ClickWallOneRobot : MonoBehaviour
{


    private GameObject go1 = null;
    private MoveTo sn1 = null;
    private Vector3 modifier;
    public float clearance = 2f;

/*	QuadTree quadTree;
	public int maxNodeDepth = 5;
	public int maxNodeObjects = 0;
	public int itemsPerAdd = 1;
	GameObject itemParent;

	void Start () {
		Generate();
	}

	void Generate() {
		quadTree = new QuadTree(100, maxNodeDepth, maxNodeObjects, new Vector2(this.transform.position.x, this.transform.position.y));
		itemParent = new GameObject("ObjectsInQuadTree");
	}

	void AddItem() {
		AddItem(new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0));
	}

	void AddItem(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.parent = itemParent.transform;
		go.transform.position = position;
		quadTree.Add(go);
	}

*/
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
                modifier.x = hit.point.x - clearance;                              // the algorithm for calculating optimum robot position from projection location should be input as a function here to replace these 3 lines.
                modifier.y = hit.point.y - clearance;
                modifier.z = hit.point.z - clearance;

                sn1.Target = modifier;                               
            }

        }
    }

/*	void OnDrawGizmos() {
		if(quadTree != null) {
			quadTree.Draw();
		}
	}
*/
}