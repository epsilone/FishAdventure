using UnityEngine;
using System.Collections;

public class DecalUpdate : MonoBehaviour {

    private Texture2D[] faceTextures;
    private GameObject[] faces; // front and back synced faces

    private Material faceMaterial;

    public float scale = 1.0f;

    float currentTime, switchTime = 1.5f;
    int faceIndex = 0;

    // Use this for initialization
    void Start() {
        this.gameObject.AddComponent("ViewDrag");

        faceTextures = new Texture2D[3];
        for (int i = 0; i < faceTextures.Length; i++) {
            string textureString = "face_0" + (i + 1) + ".png";
            faceTextures[i] = (Texture2D)Resources.LoadAssetAtPath("Assets/Resources/Textures/" + textureString, typeof(Texture2D));
        }

        string materialName = "faceMaterial.mat";
        faceMaterial = (Material)Resources.LoadAssetAtPath("Assets/Resources/Materials/"+materialName, typeof(Material));

        faces = new GameObject[2];
        for (int i = 0; i < faces.Length; i++) {
            faces[i] = createSimplePlane(i%2==0);
            //faces[i] = GameObject.CreatePrimitive(PrimitiveType.Plane);
            faces[i].name = (i % 2 == 0 ? "Back" : "Front") + "_Face";

            float zPos = transform.localScale.z * 0.501f; // 50.1%
            faces[i].transform.position = new Vector3(0f, 0f, (i % 2 == 0 ? 1 : -1) * zPos);
            faces[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            faces[i].transform.Rotate(new Vector3(0, (i % 2 == 0 ? 0 : 1) * 180f, 90));
            //            faces[i].n
            faces[i].renderer.material = faceMaterial;

            faces[i].transform.parent = transform;
        }

        faceMaterial.SetTexture("_MainTex", faceTextures[faceIndex]);
        faceMaterial.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));


    }

    private GameObject createSimplePlane(bool back) {
        float size = 0.5f;
        Mesh m = new Mesh();
        m.name = "Scripted_Plane_New_Mesh";

        Vector3[] vertices = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3((i % 3 == 0 ? -1 : 1) * size, (i < 2 ? -1 : 1) * size, 0.01f); // (-1, -1, 0), (1,-1,0), (1, 1, 0), (-1, 1, 0)

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

    private void initMaterial() {
        //faceMaterial = new Material();
    }

    // Update is called once per frame
    void Update() {
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
