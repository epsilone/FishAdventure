using UnityEngine;
using System.Collections;

public class RenderCameraTexture : MonoBehaviour {
	public GameObject target;
	private WebCamTexture camTexture;
	
	private float startTime;
	 
	void Start () {
		camTexture = new WebCamTexture();
		camTexture.Play();
		startTime = Time.time;
	}
	
	void Update () {
		if(Time.time - startTime <= 8){
			if(target){
				target.renderer.material.mainTexture = camTexture;
			}
		}else{
			enabled = false;
			GameObject.Find("WhatIs").GetComponent<WhatIsScreen>().enabled = true;
		}
	}
	
	void OnGUI() {
		if(!target){
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), camTexture);
		}
	}
}
