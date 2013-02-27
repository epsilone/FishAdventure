using com.funcom.legoxmlreader;
using UnityEngine;

public class UIInvokeFish:MonoBehaviour 
{
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width-100,123,100,30),"Invoke"))
		{
            GameObject fishGameObject = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("redmariofish", typeof(TextAsset)) as TextAsset).text, "RedFish");
            fishGameObject.AddComponent("CombineMeshes");
			fishGameObject.AddComponent("SwimBehaviour");
			fishGameObject.AddComponent("ViewDrag");
		}
	}
}
