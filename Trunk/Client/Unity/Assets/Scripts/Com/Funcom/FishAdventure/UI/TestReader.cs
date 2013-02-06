using UnityEngine;
using System.Collections;
using com.funcom.legoxmlreader;
using Com.Funcom.FishAdventure.Component.Fish;
using Com.Funcom.FishAdventure.Component.Fish.Profile;

public class TestReader:MonoBehaviour 
{
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width-100,123,100,30),"Invoke"))
		{
            Portfolio portfolio = new Portfolio();

            GameObject fishGameObject = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("redmariofish", typeof(TextAsset)) as TextAsset).text, "RedFish");
            Fish fish = fishGameObject.AddComponent("Fish") as Fish;
            fish.SetPortfolio(portfolio);
		}
	}
}
