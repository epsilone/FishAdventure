using UnityEngine;
using System.Collections;

public class WhatIsScreen : MonoBehaviour {
	public GameObject mainGameObject;
	
	public Camera previousCamera;
	public Camera sceneCamera;
	public Camera nextCamera;
	
	private Vector3 previousGameObjectPosition;
	public GameObject playerObject;
	
	public GameObject backgroundPlane;
	
	private GUIStyle fishStyle;
	private GUIStyle houseStyle;
	private GUIStyle toyStyle;	
	
	public Texture2D[] fishTexture;
	public Texture2D[] house;
	public Texture2D[] toy;
	
	private GUIStyle comeAliveStyle;
	public Texture2D[] comeAliveTexture;
	
	
	public bool drawButtons;

	void Start () {
		fishStyle = new GUIStyle();
		fishStyle.normal.background = fishTexture[0];
		fishStyle.active.background = fishTexture[1];
	
		houseStyle = new GUIStyle();
		houseStyle.normal.background = house[0];
		houseStyle.active.background = house[1];
		
		toyStyle = new GUIStyle();
		toyStyle.normal.background = toy[0];
		toyStyle.active.background = toy[1];
		
		comeAliveStyle = new GUIStyle();
		comeAliveStyle.normal.background = comeAliveTexture[0];
		comeAliveStyle.active.background = comeAliveTexture[1];
	}
	
	void OnEnable(){
		previousCamera.enabled = false;
		sceneCamera.enabled = true;
		
		previousGameObjectPosition = this.playerObject.transform.position;
		this.playerObject.transform.position = backgroundPlane.collider.bounds.center;
		this.playerObject.transform.position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, 14f);
		
		if(this.playerObject.GetComponent<Turner>()){
			this.playerObject.GetComponent<Turner>().enabled = false;
		}
		
		if(this.playerObject.GetComponent<BlendAnimation>()){
			this.playerObject.GetComponent<BlendAnimation>().enabled = false;
		}
		
		this.playerObject.animation.Rewind();
		this.playerObject.animation.Stop();
		
		mainGameObject.GetComponent<Scene01>().enabled = false;
		mainGameObject.GetComponent<BuildButton>().enabled = false;
		
		drawButtons = true;
	}
	
	void OnDisable(){
		//mainGameObject.GetComponent<BuildButton>().enabled = true;
		mainGameObject.GetComponent<Scene01>().enabled = true;
		if(sceneCamera != null){
			sceneCamera.enabled = false;
		}
		
		//gameObject.transform.position = previousGameObjectPosition;
		if(nextCamera != null){
			nextCamera.enabled = true;
		}
		
		
	}
	
	void OnGUI(){
		float texWidth = Screen.width * 0.15f;
		float texHeight = Screen.height * 0.2f;
		
		if(drawButtons){
			Rect houseRect = new Rect(Screen.width * 0.5f - texWidth * 0.5f , Screen.height - texHeight - Screen.height*0.05f, texWidth, texHeight);
			
			if(GUI.Button(houseRect, "", houseStyle)){
				Debug.Log("click on house ?!");
			}
			
			Rect fishRect = new Rect(Screen.width * 0.25f - texWidth * 0.5f , Screen.height - texHeight - Screen.height*0.05f, texWidth, texHeight);
			
			if(GUI.Button(fishRect, "", fishStyle)){
				GetComponent<StickerGUI>().enabled = true;
				drawButtons = false;
				Debug.Log("click on fish ?!");
			}
			
			Rect toyRect = new Rect(Screen.width * 0.75f - texWidth * 0.5f , Screen.height - texHeight - Screen.height*0.05f, texWidth, texHeight);
			
			if(GUI.Button(toyRect, "", toyStyle)){
				Debug.Log("click on toy ?!");
			}
		}
		
		Rect comeAliveRect = new Rect(Screen.width * 0.90f - texWidth * 0.5f , texHeight * 0.2f, texWidth, texHeight);
		
		if(GUI.Button(comeAliveRect, "", comeAliveStyle)){
			playerObject.transform.position = new Vector3(-114.1532f, 87.81333f, 0f);
			sceneCamera.enabled = false;
			nextCamera.enabled = true;
			GetComponent<StickerGUI>().enabled = false;
			
			playerObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			
			playerObject.animation.Play("birth");
			
			Debug.Log("click on comeAlive ?!");
			
			mainGameObject.GetComponent<Scene01>().FollowEntity(playerObject);
			this.enabled = false;
		}
		
	}
	
	void Update () {
		
	}
}
