using UnityEngine;
using System.Collections;

public class StatsScreen : MonoBehaviour {
	GameObject fish;
	Camera oldCamera;
	
	public Camera sceneCamera;
	private bool drawStickerGui;
	private bool drawGui;
	private FishTap fishTap;
	
	private void Awake(){		
		fishTap = GameObject.Find("MainGameObject").GetComponent<FishTap>();
		Debug.Log("FishTap : " + fishTap);
		fishTap.drawGUI = true;

		this.oldCamera = Camera.main;
		this.oldCamera.tag = "";
		this.oldCamera.enabled = false;
	}
	
	// Update is called once per frame
	private void Update () {
		if(Input.GetKeyDown(KeyCode.X)){
			drawStickerGui = !drawStickerGui;
		}
	}
	
	private void OnGUI(){
		fishTap.drawGUI = true;
	}
}