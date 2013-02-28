using UnityEngine;
using com.funcom.legoxmlreader;
using Com.Funcom.FishAdventure.Component.Entity.Living.Type;
using System.Collections;

public class CustomizationMenu : MonoBehaviour {
    private const int NUMBER_OF_BUTTON = 2;
    private const float BTN_WIDTH = 200.0f;
    private const float BTN_HEIGHT = 60.0f;

    private GameObject staticCube;
    private GameObject Fish;

    private bool switchFace = false;

    private Texture2D[] faceTextures;
    private GameObject[] faces; // front and back synced faces
    private Material faceMaterial, cubeMaterial;
    private int faceIndex = 0;
    float currentTime, switchTime = 1.5f;
    float planeSize = 1.0f;

	// Use this for initialization
	void Start () {
	
	}

    void OnGUI() {
        GUI.Label(new Rect(0, 0, Screen.width, 30), "Customization Prototype");
        

        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - (BTN_HEIGHT * 0.5f), BTN_WIDTH, BTN_HEIGHT), "Static Cube Decal")) {
            if (Fish != null) {
                Destroy(Fish);
            }
            invokeCube();
            switchFace = true;
            currentTime = 0;
            transform.position = new Vector3(0, 0, -2);

        }
        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f), BTN_WIDTH, BTN_HEIGHT), "Procedural Fish Decal")) {
            if (staticCube != null) {
                Destroy(staticCube);                
            }
            InvokeFish("redmariofish-onecolor");
            GenericUtil.CombineChildMeshesToFilter(Fish);
            Fish.AddComponent("ProceduralSwimAnimation");
            switchFace = true;
            currentTime = 0;
            transform.position = new Vector3(0, 8, -15);
            
        }

        if (GUI.Button(new Rect(0, Screen.height - 40, 100, 40), "Main Menu")) {
            Debug.Log("GoTo: PrototypeMainMenu");
            Application.LoadLevel("PrototypeMainMenu");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Fish != null || staticCube != null) {



            if (switchFace) {
                currentTime += Time.deltaTime;
                if (currentTime > switchTime) {
                    faceIndex++;
                    faceIndex %= faceTextures.Length;

                    for (int i = 0; i < faces.Length; i++) {
                        faces[i].renderer.material.SetTexture("_MainTex", faceTextures[faceIndex]);
                        faces[i].renderer.material.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
                    }
                    currentTime = 0;
                }
            }
        }
	}


    private void invokeCube(){
        if (staticCube != null) {
            Destroy(staticCube);
        }  
        
        staticCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        staticCube.transform.localScale = new Vector3(1f, 1f, 0.1f);
        staticCube.name = "CubicFish";

        string materialName = "BasicRed.mat";
        faceMaterial = (Material)Resources.LoadAssetAtPath("Assets/Resources/Materials/" + materialName, typeof(Material));
        staticCube.renderer.material = faceMaterial;
        staticCube.AddComponent("ViewDrag");
        initFaces(staticCube);
    }

    private void InvokeFish(string xmlName) {
        if (Fish != null) {
            Destroy(Fish);
        }        

        Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("XML/BaseFish/" + xmlName, typeof(TextAsset)) as TextAsset).text, "Fish");
        Fish.AddComponent("ViewDrag");

        initFaces(Fish);
    }

    private void initFaces(GameObject parent) {
        
        faceTextures = new Texture2D[3];
        for (int i = 0; i < faceTextures.Length; i++) {
            string textureString = "face_0" + (i + 1) + ".png";
            faceTextures[i] = (Texture2D)Resources.LoadAssetAtPath("Assets/Resources/Textures/" + textureString, typeof(Texture2D));
        }

        string materialName = "faceMaterial.mat";
        faceMaterial = (Material)Resources.LoadAssetAtPath("Assets/Resources/Materials/" + materialName, typeof(Material));

        faces = new GameObject[2];
        for (int i = 0; i < faces.Length; i++) {
            planeSize = parent.Equals(Fish)? 2.5f :0.25f;
            faces[i] = createSimplePlane(i % 2 == 0);
            //faces[i] = GameObject.CreatePrimitive(PrimitiveType.Plane);
            faces[i].name = (i % 2 == 0 ? "Back" : "Front") + "_Face";

            float zPos = parent.transform.localScale.z *(parent.Equals(Fish)? 1.0f: 0.501f);

            if (parent.Equals(Fish)) {
                faces[i].transform.position = new Vector3(-2.5f, 8, (i % 2 == 0 ? 1 : -1) * zPos);                 
            } else if (parent.Equals(staticCube)) {
                faces[i].transform.position = new Vector3(-0.25f,0.1f, (i % 2 == 0 ? 1 : -1) * zPos);
                
            }

            //faces[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            faces[i].transform.Rotate(new Vector3(0, (i % 2 == 0 ? 0 : 1) * 180f, 90));
            //            faces[i].n
            faces[i].renderer.material = faceMaterial;

            faces[i].transform.parent = parent.transform;
        }

        faceMaterial.SetTexture("_MainTex", faceTextures[faceIndex]);
        faceMaterial.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));

    }

    private GameObject createSimplePlane(bool back) {
        
        Mesh m = new Mesh();        

        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3((i % 3 == 0 ? -1 : 1) * planeSize, (i < 2 ? -1 : 1) * planeSize, 0.01f); // (-1, -1, 0), (1,-1,0), (1, 1, 0), (-1, 1, 0)

            int uvIndex = back ? vertices.Length - i - 1 : i;
            uvs[uvIndex] = new Vector2((i < 2 ? 0 : 1), i % 3 == 0 ? 0 : 1);  // (0, 0), (0,1), (1,1), (1,0)
        }


        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();
        GameObject obj = new GameObject((back ? "Back" : "Front") + "_Face");
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshCollider>();
        MeshFilter filter = (MeshFilter)obj.GetComponent(typeof(MeshFilter));
        filter.mesh = m;

        return obj;
    }
}
