using UnityEngine;
using System.Collections;

public class ScreenDebug : MonoBehaviour {

	string log = "";
	GUIStyle style = new GUIStyle();
	
	void Start()
	{
		style.normal.textColor = Color.black;
	}
	
	public void Log(string text)
	{
		Debug.Log("Logging : "+text);
		log += "\n"+text;
	}
	
	public void ShowLog()
	{
		GUI.Label(new Rect(0,0,Screen.width,Screen.height),log,style);
	}
	
	public string LogContent
	{
		get { return log; }
	}
}
