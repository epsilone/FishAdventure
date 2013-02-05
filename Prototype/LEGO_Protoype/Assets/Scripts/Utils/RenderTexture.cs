using UnityEngine;
using System.Collections;

public class RenderTexture : MonoBehaviour {
	public Camera cam;
	UnityEngine.RenderTexture newTex;
	public GameObject[] targetObjects;
	
	
	void Start () {
		newTex = new UnityEngine.RenderTexture(256,256,0);
		newTex.isPowerOfTwo = true;
		newTex.Create();
		
		Debug.Log("Called by" + name);
		
		foreach(GameObject go in targetObjects){
			go.renderer.material.mainTexture = newTex;
		}
		cam.targetTexture = newTex;
	}
	
	void Update () {
		newTex.Create();
	}
}
