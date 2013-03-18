using UnityEngine;

public class AIMenu : MonoBehaviour {

    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(AIMenu));
    BaseLivingEntity[] entities;

    private float BTN_WIDTH = 250.0f;
    private float BTN_HEIGHT = 75.0f;

    static int informationBoxWidth = 200;
    static int informationBoxHeight = 200;

    private Rect informationBox;

    private bool infoHidden = false;


    void Start() {
        entities = (BaseLivingEntity[])GameObject.FindObjectsOfType(typeof(BaseLivingEntity));
        BTN_WIDTH = 150;
        BTN_HEIGHT = 40;
    }


    void OnGUI() {

        GUI.skin.button.fontStyle = FontStyle.Normal;
        GUI.skin.button.fontSize = 16;

        bool hasManagedEntities = entities != null && entities.Length != 0;
        if (GUI.Button(new Rect(0, Screen.height - 40, 100, 40), "Main Menu")) {
            logger.Log("GoTo: PrototypeMainMenu");
            Application.LoadLevel("PrototypeMainMenu");
        }

        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, 0, BTN_WIDTH, BTN_HEIGHT), "Console")) {


            DebugConsole.IsOpen = !DebugConsole.IsOpen;
        }
        if (!infoHidden) {
            if (hasManagedEntities) {
                GUI.Box(new Rect(0, 0, 300, entities.Length * 30 + 30), "AI Prototype Information");
            } else {
                GUI.Label(new Rect(0, 0, 300, 30), "Found 0 Entities in the tank");
            }

            for (int i = 0; i < entities.Length; i++) {
                GUI.Label(new Rect(0, 30 * i + 30, 20, 22), entities[i].id.ToString());
                GUI.Label(new Rect(20, 30 * i + 30, 50, 22), entities[i].informationBoxNeed);
                GUI.Label(new Rect(70, 30 * i + 30, 150, 22), entities[i].informationBoxBehaviour);
            }
        }

        
        if (GUI.Button(new Rect(infoHidden ? 0 : 300, 0, BTN_WIDTH, BTN_HEIGHT), infoHidden ? "Show Info" : "Hide Info")) {
            infoHidden = !infoHidden;
        }
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel("PrototypeMainMenu");
        }

        if (Input.GetKeyDown(KeyCode.Menu)) {
            Application.Quit();
        }
    }


}
