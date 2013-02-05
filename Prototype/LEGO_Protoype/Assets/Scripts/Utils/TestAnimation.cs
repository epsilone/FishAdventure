using UnityEngine;
using System.Collections;

public class TestAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A))
		{	
			if(!animation.isPlaying){
				animation.Play();
			}else{
				animation.Stop();
			}
		}
	}
}
