using UnityEngine;
using System.Collections;

public class QuadTreeInterface3d : MonoBehaviour {

	QuadTree3d quadTree;
	public int maxNodeDepth = 5;
	public int maxNodeObjects = 0;
	public int itemsPerAdd = 1;
	GameObject itemParent;
	// Use this for initialization
	void Start () {
		Generate();
	}

	void Generate() {
		quadTree = new QuadTree3d(10, maxNodeDepth, maxNodeObjects, new Vector2(this.transform.position.x, this.transform.position.y));	//100
		itemParent = new GameObject("ObjectsInQuadTree");
	}

	void AddItem() {
		AddItem(new Vector3(Random.Range(-5f, 5f), .5f, Random.Range(-5f, 5f))); //Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0
	}

	void AddItem(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.parent = itemParent.transform;
		go.transform.position = position;
		quadTree.Add(go);
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.O)) {
			for(int item = 0; item < itemsPerAdd; item++) {
				AddItem();
			}
		}
		RaycastHit hit;
		if(Input.GetKeyDown(KeyCode.M)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //generates a ray using input from mouse for direction (i believe the local origin(camera view) is used for origin)
			Debug.DrawRay(ray.origin, ray.direction, Color.green, 15f);     //in order to see the ray casted for debugging
			int layerMask = 1 << 9;                                         //this is bit shifting in order to mask or use particular layers only - used for the raycast below (walls declared as 8th layer)
			if (Physics.Raycast(ray, out hit, 100f, layerMask))             //ray as input(or origin,direction can be used, hit for output, 100 is reach of ray, layerMask indicates which layer the ray is used in (walls here)
			{
				Vector3 temp = hit.point;
				temp.y = temp.y + .5f;
				AddItem(temp);
			}

		
		}

		if(Input.GetMouseButtonDown((int)MouseUtils.Button.Left)) {						//have to input transformations here
			quadTree.ClearSearch();														//Dmax, Dmin and angle have to be public variables
			/*calculate Dopt from projection size(public variable if mouse input)
			for Dopt dist from mouse clicked wall, create quad tree localising without increment in cost


			private ones feasibilityBonus[rows,columns];
			private ones optimalBonus[rows,columns];
			mouse input values converted into a node on the grid "input" containing input.x and input.y
			
			for(i = input.x-Dmax;i<input.x+Dmax;i++){
				for(j = input.y-Dmax;j<input.y+Dmax;i++){
			
					if (Dmin<dist(i,j)<Dmax){
						feasibilityBonus(i,j) = 10;
						if (dist(i,j) == Dopt){
							optimalBonus(i,j) = 100;
						}
					}
					if (envCost(node) less than envThreshold){  
						generateProjectionAngle();
						projectionCost[i,j] = Sum(cost of all nodes in projection angle);
					}
					cost[i,j] = envCost(node)/(optimalBonus[i,j]*feasibilityBonus[i,j]) * projectionCost[i,j];
				}
			}
			*/

			Vector3 clickPos = MouseUtils.GetMouseWorldPositionAtDepth(10);				
			GameObject nearest = quadTree.FindNearest(clickPos.x, clickPos.y, clickPos.z);
			if(nearest != null)
				Debug.DrawLine(clickPos, nearest.transform.position, Color.green, 5);
		}

	}

	void OnDrawGizmos() {
		Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.lossyScale);
		if(quadTree != null) {
			quadTree.Draw();
		}
	}
}
