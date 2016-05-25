using UnityEngine;
using System.Collections;

public class QuadTreeInterface3d : MonoBehaviour {

	QuadTree3d quadTree;
	GameObject itemParent;

	// Public variables declaration
	public int maxNodeDepth = 5;
	public int maxNodeObjects = 0;
	public int itemsPerAdd = 1;
	public int angle = 0;
	public float projectionAngle = 30f;
	public int angleRes = 5;
	public float DRes = .5f;
	public int maxHorizontalKeystone = 20;
	public int maxVerticalKeystone = 30;
	public float Dopt = 2;
	public float Dmax = 5;
	public float Dmin = 1;
	public float robotHeight = 1;
	public int baseNodeCost = 10;
	public int distanceBonusMultiplier = 2;
	public int angleBonusDivider = 10;

	// Private variables declaration
	private GameObject go1 = null;
	private MoveTo sn1 = null;
	private Vector3 projectionLocation;
	private float[,] envCostMatrix = new float[1000,6];
	private float leastCost = 10000000000000000000;
	private Vector3 leastCostIndex;
	private Vector3 destination;

	// Use this for initialization

	void Start () {
		Generate();
	}


	void Awake()
	{
		go1 = GameObject.Find("Robot01");
		if (go1 != null) {
			sn1 = go1.GetComponent<MoveTo>();
		}
		else {
			Debug.Log("find couldn't find anything!!!!");
		}
	}


	void Generate() {
		quadTree = new QuadTree3d(50, maxNodeDepth, maxNodeObjects, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z));	//(groundSize, maxNodeDepth, maxNodeObjects, groundCenter/*new Vector2(this.transform.position.x, this.transform.position.y)*/);
		itemParent = new GameObject("ObjectsInQuadTree");
	}


	void AddItem() {
		AddItem(new Vector3(Random.Range(-5f, 5f), .25f, Random.Range(-5f, 5f))); //Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0
	}


	void AddItem(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.localScale = new Vector3(.2f, .2f, .2f);
		go.transform.parent = itemParent.transform;
		go.transform.position = position;
		go.AddComponent<NavMeshObstacle> ();									//can edit area being carved out here
		go.GetComponent<NavMeshObstacle> ().carving = true;						//used for carving out navmesh
		quadTree.Add(go);
	}


	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.O)) {
			for(int item = 0; item < itemsPerAdd; item++) {
				AddItem();
			}
			quadTree = quadTree.Balance (quadTree);
		}

		RaycastHit hit;
		if(Input.GetKeyDown(KeyCode.M)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //generates a ray using input from mouse for direction (i believe the local origin(camera view) is used for origin)
			Debug.DrawRay(ray.origin, ray.direction, Color.green, 15f);     //in order to see the ray casted for debugging
			int layerMask = 1 << 9;                                         //this is bit shifting in order to mask or use particular layers only - used for the raycast below (walls declared as 8th layer)
			if (Physics.Raycast(ray, out hit, 100f, layerMask)) {           //ray as input(or origin,direction can be used, hit for output, 100 is reach of ray, layerMask indicates which layer the ray is used in (walls here)
				Vector3 temp = hit.point;
				temp.y = temp.y + .25f;
				AddItem(temp);
			}
			quadTree = quadTree.Balance (quadTree);
		}
		else if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //generates a ray using input from mouse for direction (i believe the local origin(camera view) is used for origin)
			Debug.DrawRay(ray.origin, ray.direction, Color.green, 5f);      //in order to see the ray casted for debugging
			int layerMask = 1 << 8;                                         //this is bit shifting in order to mask or use particular layers only - used for the raycast below (walls declared as 8th layer)
			if (Physics.Raycast(ray, out hit, 100f, layerMask)) {           //ray as input(or origin,direction can be used, hit for output, 100 is reach of ray, layerMask indicates which layer the ray is used in (walls here)
				int i = 0;
				projectionLocation = hit.point;

				for (angle = -maxHorizontalKeystone; angle < maxHorizontalKeystone; angle += angleRes) {
				Debug.Log("ANGLE is " + angle);
					Vector3 direction = Quaternion.AngleAxis (angle, transform.up) * hit.normal;
					Ray perpendicular = new Ray (hit.point, direction);
					Debug.DrawRay (projectionLocation, direction, Color.blue, 5f);

					float anglePower = Mathf.Abs (angle / angleBonusDivider);
					float anglePowerMax = Mathf.Abs (maxHorizontalKeystone / angleBonusDivider);





					for (float D = Dmin; D <= Dmax; D = D + DRes) {
						float effectiveD = Mathf.Sqrt ((D * D) - Mathf.Pow ((projectionLocation.y - robotHeight), 2));
						Debug.Log ("Value for D is " + D);
						Vector3 projectorLocation = perpendicular.GetPoint (effectiveD);
						//Debug.Log ("Value for effectiveD is " + effectiveD);

						//Ray projectionLine = new Ray (projectorLocation, 
						float x = projectorLocation.x;
						float z = projectorLocation.z;
						QuadTree3d node = quadTree.GetNodeContaining (x, z);
						float envCost = node.nodeCost;
						int depth = node.currentDepth;
						Debug.Log ("depth of the node is " + depth);
						Debug.Log ("cost of the node is " + envCost);

						envCostMatrix [i, 0] = x;
						envCostMatrix [i, 1] = z;
						envCostMatrix [i, 2] = envCost;

						//Add Dopt bonus as envCostMatrix[i,3] and modify envCostMatrix[i,5] to include bonus
						float distancePowerMax = Mathf.RoundToInt(Mathf.Abs(Dmax - Dmin))*distanceBonusMultiplier;
						float distancePower = Mathf.RoundToInt(Mathf.Abs(Dopt - D))*distanceBonusMultiplier;

						envCostMatrix [i, 3] = Mathf.Pow (10f, distancePowerMax - distancePower);
						Debug.Log ("the distance bonus is " + envCostMatrix [i, 3]);

						envCostMatrix [i, 3] *= Mathf.Pow (10f, anglePowerMax - anglePower);
						Debug.Log ("the total bonus is " + envCostMatrix [i, 3]);





						Vector3 oppDirection = Quaternion.AngleAxis (180f, transform.up) * direction;
						float projectionCost = 0f;
						float pointsAdded = 0f;
						for (float theta = -projectionAngle / 2; theta < projectionAngle / 2; theta += angleRes) {
							Vector3 projectionDirection = Quaternion.AngleAxis (theta, transform.up) * oppDirection;
							Ray projectionLine = new Ray (projectorLocation, projectionDirection);
							Debug.DrawRay (projectorLocation, projectionDirection, Color.cyan, 0.5f);
							for (float res = .5f; res < 0.8*effectiveD; res += DRes){
								
								Vector3 projectionCostPoint = projectionLine.GetPoint(res);
								QuadTree3d check = quadTree.GetNodeContaining(projectionCostPoint.x, projectionCostPoint.z);
								float costToAdd = check.nodeCost;
								projectionCost += costToAdd;
								pointsAdded++;
							}
						}
						projectionCost = projectionCost / pointsAdded;
						Debug.Log ("Total projection angle cost is " + projectionCost);

						envCostMatrix [i, 4] = projectionCost;
						envCostMatrix [i, 5] = envCostMatrix [i, 2]/envCostMatrix[i, 3] + envCostMatrix [i, 4];
						Debug.Log ("NET COST IS " + envCostMatrix [ i, 5]);

						if (envCostMatrix[i, 5] < leastCost) {
							leastCost = envCostMatrix[i, 5];
							Debug.Log ("least cost is " + leastCost);
							leastCostIndex.x = envCostMatrix[i, 0];
							leastCostIndex.y = 0.5f;
							leastCostIndex.z = envCostMatrix[i, 1];
							Debug.Log ("LeastCost location is " + leastCostIndex);
						}



							
/*						if (envCost < leastCost) {
							leastCost = envCost;
							Debug.Log ("least cost is " + leastCost);
							leastCostIndex.x = projectorLocation.x;
							leastCostIndex.y = 0.5f;
							leastCostIndex.z = projectorLocation.z;
							Debug.Log ("LeastCost location is " + leastCostIndex);
						} 
*/						
						i = i + 1;


					}
					i = i + 1;
				}



					//complete algorithm here

/*				float D = Mathf.Sqrt((Dopt*Dopt) - Mathf.Pow((projectionLocation.y-robotHeight),2));		//to account for height of projection.
				Debug.Log ("Value for D is " + D);
				Vector3 projectorLocation = perpendicular.GetPoint (D);
				float x = projectorLocation.x;
				float z = projectorLocation.z;
				QuadTree3d node  = quadTree.GetNodeContaining(x,z);
				float envCost = node.nodeCost;
				int depth = node.currentDepth;
				Debug.Log ("depth of the node is " + depth);
				Debug.Log ("cost of the node is " + envCost);
				sn1.Target = projectorLocation;
*/
				destination = leastCostIndex;
				Debug.Log ("DESTINATION is " + destination);
				Debug.DrawLine (projectionLocation, destination, Color.red, 5f);
				sn1.Target = leastCostIndex;
				leastCost = 1000000000000000;

			}

		}

/*		if(Input.GetMouseButtonDown((int)MouseUtils.Button.Left)) {						//have to input transformations here
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


			Vector3 clickPos = MouseUtils.GetMouseWorldPositionAtDepth(10);				
			GameObject nearest = quadTree.FindNearest(clickPos.x, clickPos.y, clickPos.z);
			if(nearest != null)
				Debug.DrawLine(clickPos, nearest.transform.position, Color.green, 5);
		}
	*/

	}

	void OnDrawGizmos() {
		Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.lossyScale);
		if(quadTree != null) {
			quadTree.Draw();
		}
	}
}
