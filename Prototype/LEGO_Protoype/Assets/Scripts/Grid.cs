using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
	public GameObject target;
	public int columns = 14;

	private LineRenderer vL;
	// Use this for initialization
	void Start () {
		//Mesh m = target.GetComponent<MeshFilter>().mesh;
		//vL = target.AddComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		/*
		for(int c = 0; c < columns; c++){
			vL.SetVertexCount(2);
			vL.SetColors(Color.red, Color.green);
			vL.SetWidth(4.0f, 4.0f);
			vL.SetPosition(0, new Vector3(target.transform.position.x + c*3, transform.position.y+100, transform.position.z-4));
			vL.SetPosition(1, new Vector3(target.transform.position.x + c*3, transform.position.y-100, transform.position.z-4));

			vL.transform.parent = target.transform;

		}
		*/
	}
}
