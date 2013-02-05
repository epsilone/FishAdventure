using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishTap : MonoBehaviour {
	public GameObject backgroundPlane;
		
	public bool drawGUI {get; set;}
	
	private GUISprite healthBar;
	private GUISprite healthBar2;
	
	private Dictionary<string, GUISprite> guiSprites;
	
	private const int BORDER = 10;
	
	private Rect healthContainer;
	
	private Rect buttonRect;

	private Color32 healthBarColorBackground = new Color32(174, 153, 94, 255);
	private Color32 healthBarColorForeground = new Color32(109, 212, 240, 255);

		//private GameObject tapPlane;
	// Use this for initialization
	void Start () {
		drawGUI = false;
		guiSprites = new Dictionary<string, GUISprite>();
		
		//healthBarColorBackground = new Color32(154, 153, 94, 255);
		//healthBarColorForeground = new  Color32(109, 212, 240, 255);
		
		//createButton();
		createHealthBars();
		//tapPlane = GameObject.Find("TapPlane");
		//if(tapPlane != null){
		//	tapPlane.active = false;
		//}else{
		//	Debug.Log("TapPlane not found!");
		//}	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.touchCount > 0){
			Touch touch = Input.touches[0];
			Vector2 position = touch.position;
			position.y = Screen.height - position.y;
			
			switch(touch.phase){
				case(TouchPhase.Began):
				
					Ray ray = Camera.main.ScreenPointToRay(touch.position);
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit)){
						GameObject hitGameObject = hit.collider.transform.gameObject;
						Debug.Log("Hit! => " + hitGameObject.name);
						Debug.Log("Point = " + hit.point);
						if(hitGameObject.tag.Equals("Player")){
							iTween.PunchPosition(hitGameObject, new Vector3(1.5f, 0f, 0f), 5);
							
							//setup object
							
							
							//tapPlane.active = true;
							drawGUI = true;
						}
					}
					
					if(buttonRect.Contains(position)){
						Debug.Log("Click");
						//tapPlane.active = false;
						drawGUI = false;
					}
				break;
			}
		}
	}
	
	void OnGUI(){
		if(drawGUI){
			//createHealthBars();
			drawHealthBars();
			
		}
	}
	
	private void drawHealthBars(){
		foreach(KeyValuePair<string, GUISprite> sprite in guiSprites){
			Debug.Log("Drawing " + sprite.Key);
			sprite.Value.Draw();
		}
	}
	
	private void createHealthBars(){
		guiSprites.Clear();
		
		float healthContainerWidth = Screen.width * 0.2f;
		float healthContainerHeight = Screen.height * 0.3f;
		healthContainer = new Rect(Screen.width - healthContainerWidth, BORDER, Screen.width - healthContainerWidth - BORDER, healthContainerHeight);
	
		int xOffset = (int)healthContainer.x;
		int yOffset = (int)healthContainer.y;
		
		for(int i = 0 ; i < 5; ++i){
			Rect spriteContainer = new Rect(xOffset, yOffset, 175, 30);
			Texture2D texture =  new Texture2D((int)spriteContainer.width, (int)spriteContainer.height);
			GUISprite sprite =  new GUISprite(spriteContainer, texture, healthBarColorBackground, healthBarColorBackground);
			
			//Rect spriteContainer2 = new Rect(xOffset, yOffset, Random.Range(10, 70), 15);
			Rect spriteContainer2 = new Rect(xOffset, yOffset, 12*i+12, 30);
			Texture2D texture2 =  new Texture2D((int)spriteContainer2.width, (int)spriteContainer2.height);
			GUISprite sprite2 =  new GUISprite(spriteContainer2, texture2, healthBarColorForeground, healthBarColorForeground);
			
			guiSprites.Add("health_red_"+i, sprite);
			guiSprites.Add("health_green_"+i, sprite2);
			
			yOffset+=45;
		}
	}   
	
	/*private void createButton(){
		tex_build_icon_path = "file://" + Application.dataPath + "/Resources/Content/Textures/tex_build_icon_128x128.png";
		
		WWW www = new WWW(tex_build_icon_path);
		while(!www.isDone){
			//wait
		}
		tex_build_icon = new Texture2D(64, 64);
		www.LoadImageIntoTexture(tex_build_icon);
		
		buttonRect = new Rect(
		Screen.width - tex_build_icon.width - 5,
		Screen.height - tex_build_icon.height - 5, 
		tex_build_icon.width,
		tex_build_icon.height);	
	}*/
}
