using com.funcom.legoxmlreader;
using UnityEngine;

public class XMLReaderMenu:MonoBehaviour 
{
	private GameObject Fish;
	
	private const int NUMBER_OF_BUTTON = 5;

    private float BTN_WIDTH = 200.0f;
    private float BTN_HEIGHT = 60.0f;

    private const int HIGH_Y = 7, LOW_Y = 2, FIXED_Z = -15;

    void Start()
    {

        BTN_WIDTH = Screen.width * 0.2f;
        BTN_HEIGHT = Screen.height * 0.12f;
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
        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, ((Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - 30, BTN_WIDTH, BTN_HEIGHT), "Mario"))
		{
			InvokeFish("redmariofish");
            gameObject.transform.position = new Vector3(cameraPos.x, HIGH_Y, FIXED_Z);
		}

        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, ((Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - 30, BTN_WIDTH, BTN_HEIGHT), "Fish"))
		{
			InvokeFish("horizontalfish");
            gameObject.transform.position = new Vector3(cameraPos.x, LOW_Y, FIXED_Z);
		}

        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, ((Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - 30, BTN_WIDTH, BTN_HEIGHT), "Bus"))
		{
			InvokeFish("thebus");
            gameObject.transform.position = new Vector3(cameraPos.x, LOW_Y, FIXED_Z);
		}

        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, ((Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - 30, BTN_WIDTH, BTN_HEIGHT), "Octopod"))
		{
			InvokeFish("octopod");
            gameObject.transform.position = new Vector3(cameraPos.x, 2*LOW_Y, FIXED_Z);
		}

        GUI.skin.button.fontStyle = FontStyle.Bold;
        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, ((Screen.height / (NUMBER_OF_BUTTON + 1)) * 5) - 30, BTN_WIDTH, BTN_HEIGHT), "Hero Fish"))
		{
			InvokeFish("hero_fish");
            gameObject.transform.position = new Vector3(cameraPos.x, LOW_Y, FIXED_Z);
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
