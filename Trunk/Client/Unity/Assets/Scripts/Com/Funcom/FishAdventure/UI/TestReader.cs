using com.funcom.legoxmlreader;
using Com.Funcom.FishAdventure.Component.Entity.Living.Type;
using UnityEngine;

public class TestReader:MonoBehaviour 
{
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width-100,123,100,30),"Invoke"))
		{
            GameObject fishGameObject = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("redmariofish", typeof(TextAsset)) as TextAsset).text, "RedFish");
            fishGameObject.AddComponent("MoveSample");
            fishGameObject.AddComponent("RotateSample");
            Fish fish = fishGameObject.AddComponent("Fish") as Fish;
		}
	}
}
