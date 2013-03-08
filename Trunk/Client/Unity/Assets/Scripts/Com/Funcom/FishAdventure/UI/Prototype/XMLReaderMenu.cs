using com.funcom.legoxmlreader;
using UnityEngine;

public class XMLReaderMenu:MonoBehaviour 
{
	private GameObject Fish;
	
	private const int NUMBER_OF_BUTTON = 5;

    private float BTN_WIDTH = 200.0f;
    private float BTN_HEIGHT = 60.0f;

    private const int HIGH_Y = 7, LOW_Y = 4;

    void Start()
    {

        BTN_WIDTH = Screen.width * 0.33f;
        BTN_HEIGHT = Screen.height * 0.2f;
    }
	
	void OnGUI()
	{
	    Vector3 cameraPos = gameObject.transform.position;

        GUI.skin.button.fontSize = 16;
        GUI.Label(new Rect(0, 0, Screen.width, 30), "XML Reader Prototype");
        GUI.skin.button.fontStyle = FontStyle.Normal;
        if (GUI.Button(new Rect(0, Screen.height - 40, 100, 40), "Main Menu")) {
            Debug.Log("GoTo: PrototypeMainMenu");
            Application.LoadLevel("PrototypeMainMenu");
        }
        GUI.skin.button.fontSize = Screen.height / 72 * 3;
        if (GUI.Button(new Rect(Screen.width - 200, ((Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - 30, 200, 60), "Mario"))
		{
			InvokeFish("redmariofish");
            gameObject.transform.position = new Vector3(cameraPos.x, HIGH_Y, cameraPos.z);
		}
		
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - 30,200,60),"Fish"))
		{
			InvokeFish("horizontalfish");
            gameObject.transform.position = new Vector3(cameraPos.x, LOW_Y, cameraPos.z);
		}
		
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - 30,200,60),"Bus"))
		{
			InvokeFish("thebus");
            gameObject.transform.position = new Vector3(cameraPos.x, LOW_Y, cameraPos.z);
		}
		
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - 30,200,60),"Octopod"))
		{
			InvokeFish("octopod");
            gameObject.transform.position = new Vector3(cameraPos.x, LOW_Y, cameraPos.z);
		}

        GUI.skin.button.fontStyle = FontStyle.Bold;
		if (GUI.Button(new Rect(Screen.width-200,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 5) - 30,200,60),"Hero Fish"))
		{
			InvokeFish("hero_fish");
            gameObject.transform.position = new Vector3(cameraPos.x, LOW_Y, cameraPos.z);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel("PrototypeMainMenu");
        }

        if (Input.GetKeyDown(KeyCode.Menu)) {
            Application.Quit();
        }
    }
}
