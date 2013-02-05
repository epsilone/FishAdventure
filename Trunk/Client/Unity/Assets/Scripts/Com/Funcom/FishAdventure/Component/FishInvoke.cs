using UnityEngine;
using System.Collections;
using com.funcom.legoxmlreader;

public class FishInvoke:MonoBehaviour
{
    private GameObject _previousFish;
	
	void OnGUI()
	{
        //LEFT
        if (GUI.Button(new Rect(0, ((Screen.height / 6) * 1) - 15 , 100, 30), "Fish 1"))
		{
            InvokeFish("Fish_1");
		}

        if (GUI.Button(new Rect(0, ((Screen.height / 6) * 2) - 15, 100, 30), "Fish 2"))
        {
            InvokeFish("Fish_2");
        }

        if (GUI.Button(new Rect(0, ((Screen.height / 6) * 3) - 15, 100, 30), "Fish 3"))
        {
            InvokeFish("Fish_3");
        }

        if (GUI.Button(new Rect(0, ((Screen.height / 6) * 4) - 15, 100, 30), "Fish 4"))
        {
            InvokeFish("Fish_4");
        }

        if (GUI.Button(new Rect(0, ((Screen.height / 6) * 5) - 15, 100, 30), "Fish 5"))
        {
            InvokeFish("Fish_5");
        }

        //RIGHT
        if (GUI.Button(new Rect(Screen.width - 100, ((Screen.height / 6) * 1) - 15, 100, 30), "Fish 6"))
        {
            InvokeFish("Fish_6");
        }

        if (GUI.Button(new Rect(Screen.width - 100, ((Screen.height / 6) * 2) - 15, 100, 30), "Fish 7"))
        {
            InvokeFish("Fish_7");
        }

        if (GUI.Button(new Rect(Screen.width - 100, ((Screen.height / 6) * 3) - 15, 100, 30), "Fish 8"))
        {
            InvokeFish("Fish_8");
        }

        if (GUI.Button(new Rect(Screen.width - 100, ((Screen.height / 6) * 4) - 15, 100, 30), "Fish 9"))
        {
            InvokeFish("Fish_9");
        }

        if (GUI.Button(new Rect(Screen.width - 100, ((Screen.height / 6) * 5) - 15, 100, 30), "Fish 10"))
        {
            InvokeFish("Fish_10");
        }
	}

    private void DestroyFish()
    {
        if (!_previousFish)
        {
            Destroy(_previousFish);
        }
    }

    private void InvokeFish(string aFishName)
    {
        DestroyFish();
        _previousFish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load(aFishName, typeof(TextAsset)) as TextAsset).text, aFishName);
        _previousFish.AddComponent("ViewDrag");
    }
}
