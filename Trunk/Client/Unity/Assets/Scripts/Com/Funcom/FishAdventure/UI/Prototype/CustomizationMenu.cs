using UnityEngine;
using com.funcom.legoxmlreader;
using System.Collections;

public class CustomizationMenu : MonoBehaviour {
    private const int NUMBER_OF_BUTTON = 2;
    private float BTN_WIDTH = 200.0f;
    private float BTN_HEIGHT = 60.0f;

    private GameObject staticCube;
    private GameObject Fish;

    private bool switchFace = false;

    private Texture2D[] eyeTextures, mouthTextures, decorationTextures;

    private GameObject[,] faces; // front and back synced faces
    private Material[] faceMaterials;
    private Material cubeMaterial;
    float[] currentTime = { 0, 0, 0 }, switchTime = { 1.2f, 1.5f, 0.5f };
    float planeSize = 1.0f;
    private int[] maxTextures = { 10, 10, 2 }, indices = { 0, 0, 0 };

    private System.Random randomizer;

    // Use this for initialization
    void Start() {
        randomizer = new System.Random();
        BTN_WIDTH = Screen.width * 0.33f;
        BTN_HEIGHT = Screen.height * 0.2f;
    }

    void OnGUI() {
        GUI.Label(new Rect(0, 0, Screen.width, 30), "Customization Prototype");
        
        GUI.skin.button.fontStyle = FontStyle.Normal;
        GUI.skin.button.fontSize = Screen.height / 72 * 3;
        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - (BTN_HEIGHT * 0.5f), BTN_WIDTH, BTN_HEIGHT), "Flat Cube\n3 Decals")) {
            if (Fish != null) {
                Destroy(Fish);
            }
            invokeCube();
            switchFace = true;
            currentTime = new float[] { 0, 0, 0 };
            transform.position = new Vector3(0, 0, -2);

        }
        if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f), BTN_WIDTH, BTN_HEIGHT), "Animated Fish\n3 Decals")) {
            if (staticCube != null) {
                Destroy(staticCube);
            }
            InvokeFish("redmariofish-onecolor");
            switchFace = true;
            currentTime = new float[] { 0, 0, 0 };
            transform.position = new Vector3(0, 8, -20);

        }


        GUI.skin.button.fontSize = 16;
        if (GUI.Button(new Rect(0, Screen.height - 40, 100, 40), "Main Menu")) {
            Debug.Log("GoTo: PrototypeMainMenu");
            Application.LoadLevel("PrototypeMainMenu");
        }
    }

    // Update is called once per frame
    void Update() {
        if (Fish != null || staticCube != null) {
            if (switchFace) {
                for (int i = 0; i < currentTime.Length; i++) {
                    currentTime[i] += Time.deltaTime;
                    if (currentTime[i] > switchTime[i]) {
                        if (i < 2)
                            indices[i] = randomizer.Next(maxTextures[i]);
                        else
                        {
                            indices[i]++;
                            indices[i] %= maxTextures[i];
                        }
                        for (int j = 0; j < faces.GetLength(0); j++) {
                            FaceSections faceSection = System.Enum.GetValues(typeof(FaceSections)).GetValue(i) is FaceSections ? (FaceSections)System.Enum.GetValues(typeof(FaceSections)).GetValue(i) : FaceSections.EYES;
                            applyTexture(faces[j, i], faceSection);
                            currentTime[i] = 0;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel("PrototypeMainMenu");
        }

        if(Input.GetKeyDown(KeyCode.Menu))
        {
            Application.Quit(); 
        }
    }

    private void applyTexture(GameObject face, FaceSections faceSection) {
        Texture2D appliedTexture = null;
        switch (faceSection) {
            case FaceSections.EYES:
                appliedTexture = eyeTextures[indices[0]];
                break;
            case FaceSections.MOUTH:
                appliedTexture = mouthTextures[indices[1]];
                break;
            case FaceSections.DECORATION:
                appliedTexture = decorationTextures[indices[2]];
                break;
        }
        if (appliedTexture != null) {
            face.renderer.material.SetTexture("_MainTex", appliedTexture);
            face.renderer.material.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
        }
    }

    private void invokeCube() {
        if (staticCube != null) {
            Destroy(staticCube);
        }

        staticCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        staticCube.transform.localScale = new Vector3(1f, 1f, 0.1f);
        staticCube.name = "CubicFish";

        string materialName = "BasicRed";
        cubeMaterial = (Material)Resources.Load("Materials/" + materialName, typeof(Material));
        staticCube.renderer.material = cubeMaterial;
        staticCube.AddComponent("ViewDrag");
        initDecals(staticCube);
    }

    private void InvokeFish(string xmlName) {
        if (Fish != null) {
            Destroy(Fish);
        }

        Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("XML/BaseFish/" + xmlName, typeof(TextAsset)) as TextAsset).text, "Fish");
        Fish.AddComponent("ViewDrag");
        Fish.transform.rotation = Quaternion.AngleAxis(0, Fish.transform.forward);
        GenericUtil.CombineChildMeshesToFilter(Fish);
        initDecals(Fish);
        Fish.AddComponent("ProceduralSwimAnimation");
    }

    private void initDecals(GameObject parent) {

        MeshRenderer meshRenderer = parent.GetComponent<MeshRenderer>();

        Bounds bounds = meshRenderer.bounds;
        print((parent.Equals(Fish) ? "Fish:" : "Cube:") + bounds + "|| Min:" + bounds.min + "|| MAX: " + bounds.max);


        eyeTextures = new Texture2D[maxTextures[0]];
        mouthTextures = new Texture2D[maxTextures[1]];
        decorationTextures = new Texture2D[maxTextures[2]];
        for (int i = 0; i < eyeTextures.Length; i++) {
            string textureString = "eyes_" + ((i < 9) ? "0" : "") + (i + 1);
            eyeTextures[i] = (Texture2D)Resources.Load("Textures/" + textureString, typeof(Texture2D));
            textureString = "mouth_" + ((i < 9) ? "0" : "") + (i + 1);
            mouthTextures[i] = (Texture2D)Resources.Load("Textures/" + textureString, typeof(Texture2D));
            if (i < decorationTextures.Length) {
                textureString = "decoration_" + ((i < 9) ? "0" : "") + (i + 1);
                decorationTextures[i] = (Texture2D)Resources.Load("Textures/" + textureString, typeof(Texture2D));
            }
        }



        faceMaterials = new Material[System.Enum.GetNames(typeof(FaceSections)).Length];
        for (int i = 0; i < faceMaterials.Length; i++) {
            FaceSections faceSection = System.Enum.GetValues(typeof(FaceSections)).GetValue(i) is FaceSections ? (FaceSections)System.Enum.GetValues(typeof(FaceSections)).GetValue(i) : FaceSections.EYES;
            string materialName = faceSection.ToString().ToLower() + "Material";
            faceMaterials[i] = (Material)Resources.Load("Materials/" + materialName, typeof(Material));
        }




        faces = new GameObject[2, System.Enum.GetNames(typeof(FaceSections)).Length];
        for (int i = 0; i < faces.GetLength(0); i++) {
            for (int j = 0; j < faces.GetLength(1); j++) {
                planeSize = parent.Equals(Fish) ? 2.5f : 0.25f;

                FaceSections faceSection = System.Enum.GetValues(typeof(FaceSections)).GetValue(j) is FaceSections
                                               ? (FaceSections)System.Enum.GetValues(typeof(FaceSections)).GetValue(j)
                                               : FaceSections.EYES;

                faces[i, j] = createSimplePlane(i % 2 == 0);
                faces[i, j].name = (i % 2 == 0 ? "Back_" : "Front_") + faceSection.ToString();


                //float zPos = bounds.min
                float xPos = 0, yPos = 0;


                bool fish = parent.Equals(Fish);
                switch (faceSection) {
                    case FaceSections.MOUTH:
                        xPos = fish ? -1.75f : -0.25f;
                        yPos = fish ? 4f : -0.25f;
                        break;
                    case FaceSections.EYES:
                        xPos = fish ? -1.75f : -0.25f;
                        yPos = fish ? 10f : 0.25f;
                        break;
                    case FaceSections.DECORATION:
                        xPos = fish ? 2f : 0.25f;
                        yPos = fish ? 11f : 0.33f;

                        break;
                }

                faces[i, j].transform.parent = parent.transform;
                faces[i, j].transform.position = new Vector3(xPos, yPos, (fish ? 1.8f : 1.01f) * (i % 2 == 0 ? bounds.max.z : bounds.min.z));
                faces[i, j].transform.Rotate(new Vector3(0, (i % 2 == 0 ? 0 : 1) * 180f, 90));


                faces[i, j].renderer.material = faceMaterials[j];


                applyTexture(faces[i, j], faceSection);
            }
        }
    }

    private GameObject createSimplePlane(bool back) {

        Mesh m = new Mesh();
        m.name = "decal";

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
        GameObject obj = new GameObject();
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshFilter>();
        obj.AddComponent<MeshCollider>();
        MeshFilter filter = (MeshFilter)obj.GetComponent(typeof(MeshFilter));
        filter.mesh = m;

        return obj;
    }
}

