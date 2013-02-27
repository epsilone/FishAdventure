using UnityEngine;

public class AIMenu:MonoBehaviour 
{
	void OnGUI()
	{
		GUI.Label(new Rect(0,0,Screen.width,30),"AI Prototype");
		
		if (GUI.Button(new Rect(0,Screen.height - 40 , 100, 40),"Main Menu"))
		{
			Debug.Log("GoTo: PrototypeMainMenu");
			Application.LoadLevel("PrototypeMainMenu");
		}
	}
}
