using UnityEngine;
using System.Collections;

public class BuildButton : MonoBehaviour {
	public Texture2D texture;
	public Vector2 buttonSize;
	private GUIStyle style;
	
	public GameObject WhatIsObject;
	private WhatIsScreen whatIsScript;
	
	private Rect position;
	private Vector2 onScreenSize;
	
	private bool state;
	void Awake() {
		style = new GUIStyle();
		style.normal.background = texture;
		
		onScreenSize = new Vector2(buttonSize.x, buttonSize.y);
		
		whatIsScript  = WhatIsObject.GetComponent<WhatIsScreen>();
	}
	
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		onScreenSize.x =  Screen.width * buttonSize.x;
		onScreenSize.y = Screen.height * buttonSize.y;
		
		position = new Rect(Screen.width - onScreenSize.x - 5, Screen.height - onScreenSize.y - 5 , onScreenSize.x, onScreenSize.y );
		
		if(GUI.Button(position, "", style)){
			if(!state){
				GetComponent<Swipe>().enabled = false;
				GetComponent<FishTap>().enabled = false;
				
				RenderCameraTexture renderCamComp = GetComponent<RenderCameraTexture>();
				renderCamComp.enabled = true;
						
				enabled = false;
				state = true;
			}else{
				whatIsScript.enabled = false;
				
				GetComponent<Swipe>().enabled = true;
				GetComponent<FishTap>().enabled = true;
				
				state = false;
			}
			Debug.Log("What is clicked");
		}
	}
}
