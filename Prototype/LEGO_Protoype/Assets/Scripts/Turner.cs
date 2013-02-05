using UnityEngine;
using System.Collections;

public class Turner : MonoBehaviour {
	Vector3 previousPosition;

	void Awake() {
		previousPosition = transform.position;
	}

	void Start () {
		//empty
	}
	void Update () {
		//empty
	}
	
	void LateUpdate()
	{
		if(transform.position!=previousPosition){
			Quaternion targetRotation = Quaternion.LookRotation(transform.position - previousPosition, Vector3.up);
			transform.rotation = targetRotation;
			transform.rotation = Quaternion.AngleAxis(180, transform.up) * targetRotation;
			//transform.rotation = Quaternion.Slerp(targetRotation, tempRotation, Time.deltaTime * 2.0f); 			
		}
		previousPosition = transform.position;
	}
}
