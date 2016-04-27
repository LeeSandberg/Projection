using UnityEngine;
using System.Collections;

public class QuadTreeInterface : MonoBehaviour {

	QuadTree quadTree;
	public int maxNodeDepth = 5;
	public int maxNodeObjects = 0;
	public int itemsPerAdd = 1;
	GameObject itemParent;
	// Use this for initialization
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
	
	// Update is called once per frame
	void Update () {

				if(Input.GetKeyDown(KeyCode.O)) {
			for(int item = 0; item < itemsPerAdd; item++) {
				AddItem();
			}
		}

		if(Input.GetKeyDown(KeyCode.A)) {
			AddItem(MouseUtils.GetMouseWorldPositionAtDepth(10));
		}

		if(Input.GetMouseButtonDown((int)MouseUtils.Button.Left)) {
			quadTree.ClearSearch();
			Vector3 clickPos = MouseUtils.GetMouseWorldPositionAtDepth(10);
			GameObject nearest = quadTree.FindNearest(clickPos.x, clickPos.y, clickPos.z);
			if(nearest != null)
				Debug.DrawLine(clickPos, nearest.transform.position, Color.green, 5);
		}
	}

	void OnDrawGizmos() {
		if(quadTree != null) {
			quadTree.Draw();
		}
	}
}
