using UnityEngine;

public class AIMenu:MonoBehaviour
{

    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(AIMenu));
    BaseLivingEntity[] entities;
    void Start() {
        entities = (BaseLivingEntity[])GameObject.FindObjectsOfType(typeof(BaseLivingEntity));
    }
    static int informationBoxWidth = 200;
    static int informationBoxHeight = 200;

    Rect informationBox;

	void OnGUI()
	{
        bool hasManagedEntities = entities != null && entities.Length != 0;
        if (hasManagedEntities)
        {
            GUI.Box(new Rect(0, 0,  300, entities.Length * 30 + 30), "AI Prototype Information");
        }
        else
        {
            GUI.Label(new Rect(0, 0, 300, 30), "Found 0 Entities in the tank");
        }

        for (int i = 0; i < entities.Length; i++)
        {
            GUI.Label(new Rect(0, 30 * i + 30, 20, 20), entities[i].id.ToString());
            GUI.Label(new Rect(20, 30 * i + 30, 50, 20), entities[i].informationBoxNeed);
            GUI.Label(new Rect(70, 30 * i + 30, 150, 20), entities[i].informationBoxBehaviour);
        }
     

		if (GUI.Button(new Rect(0,Screen.height - 40 , 100, 40),"Main Menu"))
		{
			logger.Log("GoTo: PrototypeMainMenu");
			Application.LoadLevel("PrototypeMainMenu");
		}

        if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 40), "ShowConsole"))
        {

            DebugConsole.IsOpen = !DebugConsole.IsOpen;
        }

	}

}
