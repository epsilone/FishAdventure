#region Copyright
/***************************************************
Author: Keven Poulin
Company: Funcom
Project: FishAdventure (LEGO)
****************************************************/
#endregion

#region Usings
using UnityEngine;
using System.Collections;
#endregion


public class PrototypeMainMenu:MonoBehaviour 
{
	#region Member Variables
	private const int NUMBER_OF_BUTTON = 4;
	private float BTN_WIDTH = 200.0f;
	private float BTN_HEIGHT = 60.0f;
	#endregion
	
	void Start()
	{
        BTN_WIDTH = Screen.width * 0.33f;
        BTN_HEIGHT = Screen.height * 0.15f;
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,0,Screen.width,30),"Prototype Main Menu");

	    GUI.skin.button.fontSize = Screen.height/72*3;

		if (GUI.Button(new Rect((float)(Screen.width * 0.5f) - (BTN_WIDTH * 0.5f), (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"XML Reader"))
		{
			Debug.Log("GoTo: XML Reader");
			Application.LoadLevel("XMLReader");
		}
		if (GUI.Button(new Rect((float)(Screen.width * 0.5f) -5- (BTN_WIDTH ), (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Animation Attempts"))
		{
			Debug.Log("GoTo: Animation");
			Application.LoadLevel("Animation");
		}
		if (GUI.Button(new Rect((float)(Screen.width * 0.5f) +5, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Animation 2.0"))
		{
			Debug.Log("GoTo: AnimationV2");
			Application.LoadLevel("AnimationV2");
		}
		if (GUI.Button(new Rect((float)(Screen.width * 0.5f) - (BTN_WIDTH * 0.5f), (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - (BTN_HEIGHT * 0.5f)+5,BTN_WIDTH,BTN_HEIGHT),"Fish Movement &\n Behaviour (AI)"))
		{
			Debug.Log("GoTo: AI");
			Application.LoadLevel("AI");
		}
		if (GUI.Button(new Rect((float)(Screen.width * 0.5f) - (BTN_WIDTH * 0.5f), (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Customization"))
		{
			Debug.Log("GoTo: Customization");
			Application.LoadLevel("Customization");
		}
	}
}
