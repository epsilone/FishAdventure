using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickerGUI : MonoBehaviour
{
	public Camera stickerCamera;

	private string currentSticker;
	
	public Texture2D eyeTexture;
	public Texture2D mouthTexture;
	public Texture2D moustacheTexture;
	
	private GUIStyle eyeStyle;
	private GUIStyle mouthStyle;
	private GUIStyle moustacheStyle;

	private GameObject dragSticker;

	private Rect stickerContainer;
	
	public Texture2D stickerContainerBackground;
	private Rect stickerContainerRect;
	
	const int BORDER = 50;

	public bool drawGUI;
	
	private Plane interactionPlane;
	
	private Dictionary<string, GameObject> currentStickers;
	
	private void Awake ()
	{
		drawGUI = false;
		stickerContainer = new Rect(0, 0, Screen.width, Screen.height);

		if(stickerContainerBackground == null){
			stickerContainerBackground =  new Texture2D ((int)stickerContainer.width, (int)stickerContainer.height);
		}
		
		interactionPlane = new Plane(Vector3.forward, new Vector3(280f, 0.2f, 13.75f));
		
		currentSticker = "";
		
		currentStickers = new Dictionary<string, GameObject>();
	}

	private void Start ()
	{
		eyeStyle = new GUIStyle();
		eyeStyle.normal.background = eyeTexture;
		
		mouthStyle = new GUIStyle();
		mouthStyle.normal.background = mouthTexture;
		
		moustacheStyle = new GUIStyle();
		moustacheStyle.normal.background = moustacheTexture;
	}
	
	private void Update(){
		UpdateDragger();
	}
	
	private void OnGUI (){
		if(dragSticker == null){
			//GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), stickerContainerBackground);
		
			float texWidth = Screen.width * 0.18f;
			float texHeight = Screen.height * 0.25f;
			Rect buttonPos;
			
			Rect eyeRect = new Rect(Screen.width * 0.5f - texWidth * 0.5f , Screen.height - texHeight - Screen.height*0.05f, texWidth, texHeight);
			
			if(GUI.Button(eyeRect, "", eyeStyle)){
				Debug.Log("click on Eye ?!");
				currentSticker = "Eye";
				InitSticker(currentSticker, eyeRect);
			}
			
			Rect mouthRect = new Rect(Screen.width * 0.25f - texWidth * 0.25f , Screen.height - texHeight - Screen.height*0.05f, texWidth, texHeight);
			
			if(GUI.Button(mouthRect, "", mouthStyle)){
				Debug.Log("click on mouthRect ?!");
				currentSticker = "Mouth";
				InitSticker(currentSticker, mouthRect);
			}
			
			Rect moustacheRect = new Rect(Screen.width * 0.75f - texWidth * 0.75f , Screen.height - texHeight - Screen.height*0.05f, texWidth, texHeight);
			
			if(GUI.Button(moustacheRect, "", moustacheStyle)){
				Debug.Log("click on Moustache ?!");
				currentSticker = "Moustache";
				InitSticker(currentSticker, moustacheRect);
			}
		}
	}
	
	private void InitSticker(string name, Rect buttonPos){
		if(!currentStickers.ContainsKey(currentSticker)){
			dragSticker = InstantiateSticker(currentSticker);
			if(dragSticker != null){
				Vector3 pos = stickerCamera.ScreenToWorldPoint(new Vector3(buttonPos.x, buttonPos.y, 0f));
				Debug.Log("Rect = " + buttonPos + ";\n pos = " + pos);
				dragSticker.transform.position = new Vector3(pos.x, pos.y, 13.75f);
				currentStickers.Add(currentSticker, dragSticker);
			}
		}else {
			currentSticker = "";
			return;
		}
	}
	
	public void UpdateDragger(){
		if(Input.touchCount > 0) {	
				Touch theTouch = Input.GetTouch(0); //	Cache Touch (0)
				
				switch(theTouch.phase) {
					case TouchPhase.Began:
						Debug.Log("Touch Began");
						if(dragSticker == null) {
							if(stickerCamera.pixelRect.Contains(theTouch.position)) {
								Ray ray = stickerCamera.ScreenPointToRay(theTouch.position);
								
								RaycastHit hit;
								if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9)){
									if(hit.collider.gameObject.tag.Equals("Sticker")){
										dragSticker = hit.collider.gameObject;
									}
								}
							}
						}
					break;
						
					case TouchPhase.Moved:
						//Debug.Log("Touch Moved");
						if(dragSticker != null) {
							Vector3 touchScreenPos = theTouch.position;
							touchScreenPos.y = Screen.height - theTouch.position.y;
							
							float z = 13.75f;
							
							//Debug.Log("TheTouch = " + theTouch.position);
							
							touchScreenPos = stickerCamera.ScreenToWorldPoint(new Vector3(touchScreenPos.x, touchScreenPos.y, 13.75f));
						
							dragSticker.transform.position = new Vector3(touchScreenPos.x, touchScreenPos.y, z);
							//Debug.Log("dragSticker pos = " + dragSticker.transform.position);
							
							if(stickerCamera.pixelRect.Contains(theTouch.position)) {
								Ray ray = stickerCamera.ScreenPointToRay(theTouch.position);
								float distance;
								if(interactionPlane.Raycast(ray, out distance)) {
									dragSticker.transform.position = ray.GetPoint(distance);
									float gridCubeWidth = 0.2f, gridCubeHeight = 0.2f;
									
									dragSticker.transform.position = new Vector3(
										Mathf.Floor(dragSticker.transform.position.x / gridCubeWidth) * gridCubeWidth, 
										Mathf.Floor(dragSticker.transform.position.y / gridCubeHeight) * gridCubeHeight,
										dragSticker.transform.position.z
									);
								}
							}
							
						}
					break;
						
					case TouchPhase.Ended:
						//Debug.Log("Touch Ended");
						
						if(dragSticker != null){
							float distance; 
							GameObject hitObject;
							string collisionTag = "";
							
							bool collision = SceneUtility.TestStickerCollision(dragSticker.collider.bounds, out distance, out hitObject);
							
							if(collision && hitObject != null){
								Debug.Log("Collision = " + collision + "; hitObject.transform.name  = " + hitObject.transform.name);
							
								if(hitObject.transform != null){
									collisionTag = hitObject.transform.gameObject.tag; // get tag only if hit
									Debug.Log("Tag = " + collisionTag);
								}
							}
							
							if(collision && collisionTag.Equals("Player")){
								Debug.Log("Sticker on the player");
								AttachSticker(hitObject, dragSticker);
								
								dragSticker.transform.position = new Vector3(dragSticker.transform.position.x, 
								                                             dragSticker.transform.position.y, 
							                                            	 hitObject.transform.position.z - hitObject.transform.collider.bounds.extents.z - 0.15f);
							        
								//Quaternion rot = Quaternion.AngleAxis(180, transform.up) * dragSticker.transform.rotation;
								
								GameObject copy = (GameObject)Instantiate(
							    	dragSticker, 
									new Vector3(dragSticker.transform.position.x, 
							         		   dragSticker.transform.position.y, 
							           		   hitObject.transform.position.z - hitObject.transform.collider.bounds.extents.z + 0.50f),
											   dragSticker.transform.rotation		           		    
							    );
							    
								Debug.Log("intersecting. distance = " + distance );
							}else{
								foreach(KeyValuePair<string, GameObject> k in currentStickers){
									if(k.Value == dragSticker){
										currentSticker = k.Key;
										Debug.Log("Current sticker = " + currentSticker);
										break;
									}
								}
								
								Object.Destroy(dragSticker);
								currentStickers.Remove(currentSticker);
								dragSticker = null;
								currentSticker = "";
							}
							
							dragSticker = null;
						}
					break;
				}
			}	
	}
	
	public GameObject InstantiateSticker(string stickerName){
		Object temp = Resources.Load("Content/Prefabs/Stickers/" + stickerName);
		if(!temp){
			Debug.Log("Error while loading " + stickerName);
			return null;
		}else{
			return GameObject.Instantiate(temp) as GameObject;
		}
	}

	public void AttachSticker(GameObject gameObject, GameObject sticker){
		Transform bone = gameObject.transform.FindChild("c_Spine01_jnt");
		if(!bone){
			Debug.Log("Bone not found on " + gameObject.name);
			return;
		}
		
		sticker.transform.parent = bone;
	}
}

/*
GUI.DrawTexture (stickerContainer, stickerContainerBackground);
		for (int i = 0; i < textures.Length; i++){
			Texture tex = textures[i];
			Rect stickerRect = new Rect (stickerContainer.x + (stickerContainer.width/2) - tex.width, stickerContainer.y+10, tex.width, tex.height);
	
			if (dragSticker == null || dragSticker != tex) {
				GUI.DrawTexture (stickerRect, tex);
				
				Vector3 position;
				if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
					position = Input.GetTouch (0).position;
					position.y = Screen.height - position.y;

					if(stickerRect.Contains (position))
					{
						dragSticker = tex;
					}
				}
			}
		}
		
		if (dragSticker != null) {
			if (Input.touchCount > 0) {
				Vector3 position;
				position = Input.GetTouch (0).position;
				position.y = Screen.height - position.y;

				if(Input.GetTouch (0).phase == TouchPhase.Moved)
				{
					if (!stickerContainer.Contains (position)) {
						dragSticker = null;
						Debug.Log ("Sticker outside of the container");
						return;
					}
				}

				//Rect dragRect = new Rect (position.x - (dragSticker.GetSize ().x / 2), position.y - (dragSticker.GetSize ().y / 2), dragSticker.GetSize ().x, dragSticker.GetSize ().y);
				//GUI.DrawTexture (dragRect, dragSticker.GetTexture (), ScaleMode.ScaleToFit);
			}

			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
				dragSticker = null;
				return;
			}
		}
*/
/*

	private void drawInventory(){
		foreach(KeyValuePair<string, GUISprite> sprite in guiSprites){
			if(sprite.Value.isPressed()){
				sprite.Value.setColor(colorOn);
				if(sprite.Key.Equals("left")){
					XOffset += 5;
				}else if(sprite.Key.Equals("right")){
					XOffset -= 5;
				}
			}else{
				if(sprite.Value.getColor() != colorOff)
					sprite.Value.setColor(colorOff);
			}
			sprite.Value.Draw();
		}
	}
*/

