using com.funcom.legoxmlreader;
using UnityEngine;

public class XMLReaderMenu:MonoBehaviour 
{
	private GameObject Fish;
	
	private const int NUMBER_OF_BUTTON = 4;
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,0,Screen.width,30),"XML Reader Prototype");
		
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - 30,200,60),"Mario"))
		{
			InvokeFish("redmariofish");
		}
		
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - 30,200,60),"Fish"))
		{
			InvokeFish("horizontalfish");
		}
		
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - 30,200,60),"Bus"))
		{
			InvokeFish("thebus");
		}
		
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - 30,200,60),"Octopod"))
		{
			InvokeFish("octopod");
		}
		
		if (GUI.Button(new Rect(0,Screen.height - 40 , 100, 40),"Main Menu"))
		{
			Debug.Log("GoTo: PrototypeMainMenu");
			Application.LoadLevel("PrototypeMainMenu");
		}
	}
	
	private void InvokeFish(string xmlName)
	{
		if(Fish != null)
		{
			Destroy(Fish);
		}
		
        Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("XML/BaseFish/" + xmlName, typeof(TextAsset)) as TextAsset).text, "Fish");
		Fish.AddComponent("ViewDrag");
	}
}
