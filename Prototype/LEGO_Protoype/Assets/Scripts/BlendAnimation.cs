using UnityEngine;
using System.Collections;

public class BlendAnimation : MonoBehaviour {
	public bool blend;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(blend){
			animation.Blend("swim", 0.8f);
		}
	}
}
