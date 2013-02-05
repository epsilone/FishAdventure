using UnityEngine;
using System.Collections;
using com.funcom.legoxmlreader;

public class TestReader : MonoBehaviour 
{
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width-100,123,100,30),"test"))
		{
            LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("redmariofish", typeof(TextAsset)) as TextAsset).text, "test");
		}
	}
}
