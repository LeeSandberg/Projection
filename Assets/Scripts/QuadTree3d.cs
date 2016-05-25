using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadTree3d {

	static int childCount = 4;
	static int maxObjectCount = 100;
	static int maxDepth;
	static float baseNodeCost = 1000;

	//Used for visual debugging/demonstation
	private bool searched = false;

	private QuadTree3d nodeParent;
	private QuadTree3d[] childNodes;

	private List<GameObject> objects = new List<GameObject>();

	public static List<QuadTree3d> leaves = new List<QuadTree3d> ();			// For balancing - contains all leaves

	public int currentDepth = 0;
	public float nodeCost;

	private Vector3 nodeCenter;															//
	private Rect nodeBounds = new Rect();

	private float nodeSize = 0f;

	public QuadTree3d(float worldSize, int maxNodeDepth, int maxNodeObjects, Vector3 center) : this(worldSize, 0, center, null) {
		maxDepth = maxNodeDepth;
		maxObjectCount = maxNodeObjects;
	}

	private QuadTree3d(float size, int depth, Vector3 center, QuadTree3d parent)
	{
		this.nodeSize = size;
		this.currentDepth = depth;
		this.nodeCenter = center;
		this.nodeParent = parent;

		if(this.currentDepth == 0) {
			this.nodeBounds = new Rect(center.x - size, center.z - size, size*2, size*2);
		} else {
			this.nodeBounds = new Rect(center.x - (size/2), center.z - (size/2), size, size);
		}
		this.nodeCost = baseNodeCost  * Mathf.Pow(10,this.currentDepth);
	}

	public bool Add(GameObject go)
	{
		if (this.nodeBounds.Contains(go.transform.position))
		{
			return this.Add(go, new Vector2(go.transform.position.x, go.transform.position.z)) != null;			//
		}
		return false;
	}



	private QuadTree3d Add(GameObject obj, Vector3 objCenter)													//
	{
		if (this.childNodes != null)
		{
			// Four nodes
			//
			//   ^ z plus
			// ╒═╤═╕
			// │2│3│
			// ╞═╪═╡ > x plus
			// │0│1│
			// ╘═╧═╛

			int index = (objCenter.x < this.nodeCenter.x ? 0 : 1) //Add one to select between  3,1. Add zero to select between 2,0.
				+ (objCenter.y < this.nodeCenter.z ? 0 : 2);//Add two to select between  2,3. Add zero to select between 0,1.

			return this.childNodes[index].Add(obj, objCenter);
		}
		//We've reached a root


/*		if (this.currentDepth < maxDepth && this.objects.Count + 1 > maxObjectCount) {
			//If adding this object puts this node past its limit, and we're not at the 
			// maximum depth, split this node and redistribute its objects to its children
			Split (nodeSize);
			//Insert nodeCost here instead??????
			foreach (GameObject nodeObject in objects) {
				Add (nodeObject);
			}
			this.objects.Clear ();

			//And don't forget to add the object that caused us to split!
			return Add (obj, objCenter);
		} 
		else {
			//Otherwise, just add this object to this node pool
			this.objects.Add(obj);
		}

*/

		if (this.objects.Count + 1 > maxObjectCount) {
			//If adding this object puts this node past its limit, and we're not at the 
			// maximum depth, split this node and redistribute its objects to its children
			if (this.currentDepth < maxDepth) {
				Split (nodeSize);

				foreach (GameObject nodeObject in objects) {
					Add (nodeObject);
				}
				this.objects.Clear ();

				//And don't forget to add the object that caused us to split!
				return Add (obj, objCenter);
			} else {
				this.nodeCost = this.nodeCost * 100;
			}
		}
		else {
			//Otherwise, just add this object to this node pool
			this.objects.Add(obj);

		}
		return this;



	}


	public bool Remove(GameObject obj)
	{
		if(objects.Contains(obj)) {
			objects.Remove(obj);
			return true;
		}
		else if(childNodes != null) {
			foreach(QuadTree3d child in childNodes) {
				if(child.Remove(obj))
					return true;
			}
		}
		return false;
	}






	public bool AddLeaf(QuadTree3d leaf)
	{
		leaves.Add (leaf);
		return true;

	}

	public bool RemoveLeaf(QuadTree3d leaf)
	{
		if (leaves.Contains (leaf)) {
			leaves.Remove (leaf);
			return true;
		} else {
			return false;
		}
	}






	public QuadTree3d Balance(QuadTree3d quad){

		while (leaves.Count>0){
			QuadTree3d toBalance = leaves [0];
	
			QuadTree3d n1 = GetNodeContaining ((toBalance.nodeCenter.x - toBalance.nodeSize), toBalance.nodeCenter.z);
			Debug.Log ("BALANCING");
			//Debug.Log ("nodecenter" + nodeCenter);
			//Debug.Log ("nodeSize" + nodeSize);
			int neighbour1Depth = n1.currentDepth;
			Debug.Log ("neighbor1 depth " + neighbour1Depth);
			while (toBalance.currentDepth > (neighbour1Depth+1)) {
				BalanceSplit (n1);
				neighbour1Depth++;
				Debug.Log ("neighbor1 successful");
			}
	
			QuadTree3d n2 = GetNodeContaining ((toBalance.nodeCenter.x + toBalance.nodeSize), toBalance.nodeCenter.z);
			Debug.Log ("BALANCING");
			//Debug.Log ("nodecenter" + nodeCenter);
			//Debug.Log ("nodeSize" + nodeSize);
			int neighbour2Depth = n2.currentDepth;
			Debug.Log ("neighbor2 depth " + neighbour2Depth);
			while (toBalance.currentDepth > (neighbour2Depth+1)) {
				BalanceSplit (n2);
				neighbour2Depth++;
				Debug.Log ("neighbor2 successful");
			}
	
			QuadTree3d n3 = GetNodeContaining (toBalance.nodeCenter.x, (toBalance.nodeCenter.z - toBalance.nodeSize));
			Debug.Log ("BALANCING");
			//Debug.Log ("nodecenter" + nodeCenter);
			//Debug.Log ("nodeSize" + nodeSize);
			int neighbour3Depth = n3.currentDepth;
			Debug.Log ("neighbor3 depth " + neighbour3Depth);
			while (toBalance.currentDepth > (neighbour3Depth+1)) {
				BalanceSplit (n3);
				neighbour3Depth++;
				Debug.Log ("neighbor3 successful");
			}
	
			QuadTree3d n4 = GetNodeContaining (toBalance.nodeCenter.x, (toBalance.nodeCenter.z + toBalance.nodeSize));
			Debug.Log ("BALANCING");
			//Debug.Log ("nodecenter" + nodeCenter);
			//Debug.Log ("nodeSize" + nodeSize);
			int neighbour4Depth = n4.currentDepth;
			Debug.Log ("neighbor4 depth " + neighbour4Depth);
			while (toBalance.currentDepth > (neighbour4Depth+1)) {
				BalanceSplit (n4);
				neighbour4Depth++;
				Debug.Log ("neighbor4 successful");
			}
	
	
			quad.RemoveLeaf(leaves[0]);
			Debug.Log ("LEAVES " + leaves.Count);
			//Debug.Log ("LEAF0 " + leaves[0].nodeCost);
			//Debug.Log ("LEAF1 " + leaves[1].nodeCenter);
	
	
		}
		return quad;

	}
	
	
	
	private void Split(float parentSize)
	{
		QuadTree3d toRemove = this;
		this.childNodes = new QuadTree3d[QuadTree3d.childCount];
		int depth = this.currentDepth + 1;
		float quarter = parentSize / 4f;

		this.childNodes[0] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(-quarter, 0, -quarter), this);
		this.childNodes[1] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(quarter, 0, -quarter), this);
		this.childNodes[2] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(-quarter, 0, quarter), this);
		this.childNodes[3] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(quarter, 0, quarter), this);

		RemoveLeaf (toRemove);

		AddLeaf (this.childNodes[0]);
		AddLeaf (this.childNodes[1]);
		AddLeaf (this.childNodes[2]);
		AddLeaf (this.childNodes[3]);

		Debug.Log ("LEAF0 " + leaves[0].nodeCost);
		Debug.Log ("LEAF1 " + leaves[1].nodeCenter);
		Debug.Log ("LEAF2 " + leaves[2].nodeCenter);
		Debug.Log ("LEAF3 " + leaves[3].nodeCenter);
		Debug.Log("LEAVES " + leaves.Count);



	}



	private void BalanceSplit(QuadTree3d toSplit)
	{
		float parentSize = toSplit.nodeSize;
		toSplit.childNodes = new QuadTree3d[QuadTree3d.childCount];
		int depth = toSplit.currentDepth + 1;
		float quarter = parentSize / 4f;

		toSplit.childNodes[0] = new QuadTree3d(parentSize/2, depth, toSplit.nodeCenter + new Vector3(-quarter, 0, -quarter), toSplit);
		toSplit.childNodes[1] = new QuadTree3d(parentSize/2, depth, toSplit.nodeCenter + new Vector3(quarter, 0, -quarter), toSplit);
		toSplit.childNodes[2] = new QuadTree3d(parentSize/2, depth, toSplit.nodeCenter + new Vector3(-quarter, 0, quarter), toSplit);
		toSplit.childNodes[3] = new QuadTree3d(parentSize/2, depth, toSplit.nodeCenter + new Vector3(quarter, 0, quarter), toSplit);

		RemoveLeaf (toSplit);

		AddLeaf (toSplit.childNodes[0]);
		AddLeaf (toSplit.childNodes[1]);
		AddLeaf (toSplit.childNodes[2]);
		AddLeaf (toSplit.childNodes[3]);
	}



	/*	public GameObject FindNearest(Vector3 position) {
		return FindNearest(position.x, position.y, position.z);
	}

	public GameObject FindNearest(float x, float y, float z) {
		double maxDistance = double.MaxValue;
		return FindNearest(x, y, z, ref maxDistance);
	}

	private GameObject FindNearest(float x, float y, float z, ref double shortestDistance)
	{
		GameObject closest = null;

		//Reached a root node, check its objects
		if (childNodes == null)
		{
			searched = true;
			//We're a root node, check the objects we have
			foreach (GameObject obj in objects)
			{
				double distance = Mathf.Sqrt(
					Mathf.Pow(x - obj.transform.position.x, 2.0f) +
					Mathf.Pow(y - obj.transform.position.y, 2.0f) +
					Mathf.Pow(z - obj.transform.position.z, 2.0f));

				if ((distance > shortestDistance)) 
					continue;

				shortestDistance = distance;
				closest = obj;
			}
			return closest;
		}

		//Keep stepping into the children until we reach a root (above)
		foreach (QuadTree3d child in childNodes)
		{
			double childDistance = GeneralUtils.DistanceToRectEdge(child.nodeBounds, x, y);
			if (childDistance > shortestDistance) 
				continue;

			GameObject tmpObject = child.FindNearest(x, y, z, ref shortestDistance);
			if (tmpObject != null)
				closest = tmpObject;
		}
		return closest;
	}
*/
/*	private QuadTree3d GetNodeContaining(float x, float y) {
		if (this.childNodes != null)
		{
			// Find the index of the child that contains the center of the object
			int index = (x < this.nodeCenter.x ? 0 : 1) 
				+ (y < this.nodeCenter.z ? 0 : 2);

			return this.childNodes[index].GetNodeContaining(x, y);
		} else {
			return this;
		}
	}
*/



	public QuadTree3d GetNodeContaining(float x, float z) {
		if (this.childNodes != null) {
			// Find the index of the child that contains the center of the object
			int index = (x < this.nodeCenter.x ? 0 : 1)
			            + (z < this.nodeCenter.z ? 0 : 2);

			return this.childNodes [index].GetNodeContaining (x, z);
		} else {
			return this;
		}
	}

	public void ClearSearch() {
		searched = false;
		if(childNodes != null) {
			foreach(QuadTree3d child in childNodes) {
				child.ClearSearch();
			}
		}
	}

	public void Clear() {
		objects.Clear();
		if(childNodes != null) {
			foreach(QuadTree3d child in childNodes) {
				child.Clear();
			}
			childNodes = null;
		}
	}


	public void Draw() {



		Gizmos.DrawWireCube(nodeCenter, new Vector3(nodeSize, 0, nodeSize));

/*		if(searched) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(nodeCenter, (nodeSize/2));
			Gizmos.color = Color.white;
		}
		*/

		if(childNodes != null) {
			foreach(QuadTree3d child in childNodes) {
				child.Draw();
			}
		}
	}
}




/*
				QuadTree3d neighbour1 = GetNodeContaining ((nodeCenter.x - nodeSize), nodeCenter.z);
				Debug.Log ("BALANCING");
				//Debug.Log ("nodecenter" + nodeCenter);
				//Debug.Log ("nodeSize" + nodeSize);
				Debug.Log ("current depth " + this.currentDepth);
				Debug.Log ("neighbor1 depth " + neighbour1.currentDepth);
				if (this.currentDepth > (neighbour1.currentDepth+1)) {
					balanceSplit (neighbour1);
					Debug.Log ("neighbor1 successful");
				}
				QuadTree3d neighbour2 = GetNodeContaining ((nodeCenter.x + nodeSize), nodeCenter.z);
				Debug.Log ("neighbor2 depth " + neighbour1.currentDepth);

				Debug.DrawLine (neighbour1.nodeCenter, neighbour2.nodeCenter, Color.red, 5f);

				if (this.currentDepth > (neighbour2.currentDepth+1)) {
					balanceSplit (neighbour2);
					Debug.Log ("neighbor2 successful");

				}
				QuadTree3d neighbour3 = GetNodeContaining (nodeCenter.x, (nodeCenter.z - nodeSize));
				Debug.Log ("neighbor3 depth " + neighbour1.currentDepth);

				if (this.currentDepth > (neighbour3.currentDepth+1)) {
					balanceSplit (neighbour3);
					Debug.Log ("neighbor3 successful");

				}
				QuadTree3d neighbour4 = GetNodeContaining (nodeCenter.x, (nodeCenter.z + nodeSize));
				Debug.Log ("neighbor4 depth " + neighbour1.currentDepth);

				if (this.currentDepth > (neighbour4.currentDepth+1)) {
					balanceSplit (neighbour4);
					Debug.Log ("neighbor4 successful");

				}
*/