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

	private void Split(float parentSize)
	{
		this.childNodes = new QuadTree3d[QuadTree3d.childCount];
		int depth = this.currentDepth + 1;
		float quarter = parentSize / 4f;

		this.childNodes[0] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(-quarter, 0, -quarter), this);
		this.childNodes[1] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(quarter, 0, -quarter), this);
		this.childNodes[2] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(-quarter, 0, quarter), this);
		this.childNodes[3] = new QuadTree3d(parentSize/2, depth, this.nodeCenter + new Vector3(quarter, 0, quarter), this);
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
		if (this.childNodes != null)
		{
			// Find the index of the child that contains the center of the object
			int index = (x < this.nodeCenter.x ? 0 : 1) 
				+ (z < this.nodeCenter.z ? 0 : 2);

			return this.childNodes[index].GetNodeContaining(x, z);
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

