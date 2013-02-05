using UnityEngine;
using System.Collections;

public class LimitCamera : MonoBehaviour {
	public GameObject boundingBox;

	void Start () {
	
	}
	
	void Update () {
		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x, boundingBox.renderer.bounds.min.x, boundingBox.renderer.bounds.max.x),
		    Mathf.Clamp(transform.position.y, boundingBox.renderer.bounds.min.y, boundingBox.renderer.bounds.max.y),
			transform.position.z);
	}
}
