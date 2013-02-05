using UnityEngine;
using System.Collections;

public class Scene01 : MonoBehaviour {
	public GameObject fish;
	public PhysicsMotor motor;

	private CameraManager cameraManager;
	
	public Camera mainCamera;
	public Camera statsCamera;
	public Camera whatisCamera;

	public StickerGUI gui;

	private GameObject introText;
	
	private enum ESTATE {INTRO, GAMEPLAY, STATS, WHATIS, STICKER, FOLLOW, POSTFOLLOW} // {0 = intro, 1 = gameplay, ...}
	private ESTATE state;
	 
	private bool scrolling;
	
	private bool drawStickerGui;
	
	private Swipe swipe;
	
	private float startTime;
	private bool following;
	
	void Awake(){
		drawStickerGui = false;
		
		swipe = GameObject.Find("MainGameObject").GetComponent<Swipe>();
		this.cameraManager = GetComponent<CameraManager>();
		mainCamera.transform.position = new Vector3(-112f, 88f, -7.73f);
		
		//Camera.main.transform.position = new Vector3(-40, 0, -15);
		
		this.state = ESTATE.INTRO;
		
		introText = GameObject.Find("IntroText");
		introText.active = false;
		scrolling = false; //scrolling the camera ?
		//cameraManager.StartScroll(new Vector3(0, 0, -10), 20);
	} 
		
	private void Update(){
		handleInput();
		
		if(state == ESTATE.INTRO){
			introText.active = true;
			if(Time.time >= 3f){
				state = ESTATE.GAMEPLAY;
				Debug.Log("Waiting ... finished" + (Time.time));
			}
			
		} else if(state == ESTATE.GAMEPLAY ){
			if(cameraManager != null && !scrolling){
				cameraManager.StartScroll(new Vector3(-112f, 79f, -7.73f), 5);
				scrolling = true;
			}
			
			if(cameraManager.cameraMode == CameraManager.CAMERA_MODE.STATIC){
				//scrolling = false;
				introText.active = false;
				GetComponent<BuildButton>().enabled = true;
				//drawStickerGui = true;
	
			}
		} else if(state == ESTATE.STATS){
			mainCamera.enabled = false;
			this.statsCamera.enabled = true;
		} else if(state == ESTATE.WHATIS){
		
		} else if(state == ESTATE.STICKER){
		
		} else if (state == ESTATE.FOLLOW){
			if(!fish.animation.IsPlaying("birth") && !following){
				fish.AddComponent<BlendAnimation>();
				fish.GetComponent<BlendAnimation>().enabled = true;
				fish.GetComponent<BlendAnimation>().blend = true;
				
				//fish.AddComponent<Turner>();
				fish.GetComponent<Turner>().enabled = true;
				fish.animation.Play("SwimAroundTheWorld");
				
				following = true;
			}
			
			if(following && Time.time - startTime >= 18){
				cameraManager.cameraMode = CameraManager.CAMERA_MODE.STATIC;
				state = ESTATE.POSTFOLLOW;
				GetComponent<Swipe>().enabled = true;
				
			}
		} else if (state == ESTATE.POSTFOLLOW){
			
		}
		
		//dragRigid.UpdateDragger(fish);
	}
	
	public void FollowEntity(GameObject entity){
		state = ESTATE.FOLLOW;
		startTime = Time.time;
		fish = entity;
		cameraManager.FollowEntity(entity);
	}
	
	private void handleInput(){
		if(Input.GetKeyDown(KeyCode.A)){
			state = ESTATE.INTRO;
		}else if(Input.GetKeyDown(KeyCode.S)){
			state = ESTATE.GAMEPLAY;
		}else if(Input.GetKeyDown(KeyCode.D)){
			state = ESTATE.STATS;
		}else if(Input.GetKeyDown(KeyCode.F)){
			state = ESTATE.STICKER;
			drawStickerGui = !drawStickerGui;
		}else if(Input.GetKeyDown(KeyCode.G)){
			state = ESTATE.WHATIS;
		}
	}
	
	private void OnGUI(){
	}
}