using com.funcom.legoxmlreader;
using UnityEngine;
using System;
using System.Collections;

public class AnimationV2Menu:MonoBehaviour
{
	public GameObject rig;
	public AnimationClip swimAnimation;
	private GameObject Fish;
	private GameObject AnimatedFish;
	private Texture2D[] eyeTextures, mouthTextures, decorationTextures;
	private GameObject[,] faces; // front and back synced faces
	private Material[] faceMaterials;
	private Material cubeMaterial;
	float[] currentTime = { 0, 0, 0 }, switchTime = { 1.2f, 1.5f, 0.5f };
	float planeSize = 1.0f;
	private int[] maxTextures = { 10, 10, 2 }, indices = { 0, 0, 0 };
	private System.Random randomizer;
	private bool switchFace = false;
	
	void Start ()
	{
		randomizer = new System.Random ();

		AnimatedFish = GameObject.Find ("AnimatedFish");
		AnimatedFish.AddComponent ("ViewDrag");
		
		InvokeFish ("hero_fish");
		
		//Set var
		SkinnedMeshRenderer skinnedMeshRenderer;
		Matrix4x4 matrixBuffer;
		Vector3 size;
		Transform[] bonesRefList = new Transform[4];
		Matrix4x4[] bindPoses = new Matrix4x4[4];
		
		//Move fish object to prefab
		Transform[] allChildren = Fish.GetComponentsInChildren<Transform> ();
		foreach (Transform child in allChildren) {
			if (child.parent == Fish.transform) {
				child.parent = AnimatedFish.transform;
			}
		}
		
		//Destroy fish
		Destroy (Fish);
		
		// Decal it
		initDecals (AnimatedFish);
		
		//Combine children mesh to parent (Skinning mode)
		GenericUtil.CombineChildMeshesToSkin (AnimatedFish);
		skinnedMeshRenderer = AnimatedFish.GetComponent<SkinnedMeshRenderer> ();
		
		//Get size
		size = skinnedMeshRenderer.sharedMesh.bounds.size;
		
		//Get bones
		bonesRefList [0] = AnimatedFish.transform.Find ("Bone005").transform;
		bonesRefList [1] = bonesRefList [0].FindChild ("Bone006").transform;
		bonesRefList [2] = bonesRefList [1].FindChild ("Bone007").transform;
		bonesRefList [3] = bonesRefList [2].FindChild ("Bone008").transform;
		
		//Move bones
		bonesRefList [0].position = new Vector3 (-(size.x * 0.5f), size.y * 0.5f, 0);
		bonesRefList [1].position = bonesRefList [0].position + new Vector3 (2f, 0, 0);
		bonesRefList [2].position = bonesRefList [1].position + new Vector3 (2f, 0, 0);
		bonesRefList [3].position = bonesRefList [2].position + new Vector3 (2f, 0, 0);
		
		/*bonesRefList [0].position = new Vector3 ((size.x * 0.5f), size.y * 0.5f, 0);
		bonesRefList [1].position = bonesRefList [0].position - new Vector3 (2f, 0, 0);
		bonesRefList [2].position = bonesRefList [1].position - new Vector3 (2f, 0, 0);
		bonesRefList [3].position = bonesRefList [2].position - new Vector3 (2f, 0, 0);*/
		
		//Bones rotation (HACK)
		bonesRefList [0].localEulerAngles = new Vector3 (-90f, 0f, 0f);
		//bonesRefList [0].localEulerAngles = new Vector3 (180f, 0f, 0f);
		
		//Bind poses
		bindPoses [0] = bonesRefList [0].worldToLocalMatrix;
		bindPoses [1] = bonesRefList [1].worldToLocalMatrix;
		bindPoses [2] = bonesRefList [2].worldToLocalMatrix;
		bindPoses [3] = bonesRefList [3].worldToLocalMatrix;
		
		//Populate bones info
		skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
		skinnedMeshRenderer.bones = bonesRefList;
		skinnedMeshRenderer.rootBone = bonesRefList [0];
		
		//Skinning
		GenericUtil.SkinIt (AnimatedFish, 0f);
		
		//Animation
		AnimatedFish.AddComponent<Animation> ();
		AnimatedFish.animation.AddClip (swimAnimation, "idle");
		AnimatedFish.animation.wrapMode = WrapMode.Loop;
		AnimatedFish.animation.Play ("idle");
		

		switchFace = true;
	}
	
	void OnGUI ()
	{
		GUI.Label (new Rect (0, 0, Screen.width, 30), "Animation Prototype V2");
		GUI.skin.button.fontStyle = FontStyle.Normal;
	    GUI.skin.button.fontSize = 16;
		if (GUI.Button (new Rect (0, Screen.height - 40, 100, 40), "Main Menu")) {
			Debug.Log ("GoTo: PrototypeMainMenu");
			Application.LoadLevel ("PrototypeMainMenu");
		}
	}
	
	private void InvokeFish (string xmlName)
	{
		Fish = LEGOXMLReader.GetLegoObjectByLxfml ((Resources.Load ("XML/BaseFish/" + xmlName, typeof(TextAsset)) as TextAsset).text, "Fish");
	}
	
	private void applyTexture (GameObject face, FaceSections faceSection)
	{
		Texture2D appliedTexture = null;
		switch (faceSection) {
		case FaceSections.EYES:
			appliedTexture = eyeTextures [indices [0]];
			break;
		case FaceSections.MOUTH:
			appliedTexture = mouthTextures [indices [1]];
			break;
		case FaceSections.DECORATION:
			appliedTexture = decorationTextures [indices [2]];
			break;
		}
		if (appliedTexture != null) {
			face.renderer.material.SetTexture ("_MainTex", appliedTexture);
			face.renderer.material.SetTextureScale ("_MainTex", new Vector2 (1.0f, 1.0f));
		}
	}
	
	private void initDecals (GameObject parent)
	{

	//	SkinnedMeshRenderer meshRenderer = parent.GetComponent<SkinnedMeshRenderer> ();

	//	Bounds bounds = meshRenderer.sharedMesh.bounds;
	//	print ((parent.Equals (AnimatedFish) ? "Fish:" : "Cube:") + bounds + "|| Min:" + bounds.min + "|| MAX: " + bounds.max);


		eyeTextures = new Texture2D[maxTextures [0]];
		mouthTextures = new Texture2D[maxTextures [1]];
		decorationTextures = new Texture2D[maxTextures [2]];
		for (int i = 0; i < eyeTextures.Length; i++) {
			string textureString = "eyes_" + ((i < 9) ? "0" : "") + (i + 1);
			eyeTextures [i] = (Texture2D)Resources.Load ("Textures/" + textureString, typeof(Texture2D));
			textureString = "mouth_" + ((i < 9) ? "0" : "") + (i + 1);
			mouthTextures [i] = (Texture2D)Resources.Load ("Textures/" + textureString, typeof(Texture2D));
			if (i < decorationTextures.Length) {
				textureString = "no_decoration";
				decorationTextures [i] = (Texture2D)Resources.Load ("Textures/" + textureString, typeof(Texture2D));
			}
		}



		faceMaterials = new Material[System.Enum.GetNames (typeof(FaceSections)).Length];
		for (int i = 0; i < faceMaterials.Length; i++) {
			FaceSections faceSection = System.Enum.GetValues (typeof(FaceSections)).GetValue (i) is FaceSections ? (FaceSections)System.Enum.GetValues (typeof(FaceSections)).GetValue (i) : FaceSections.EYES;
			string materialName = faceSection.ToString ().ToLower () + "Material";
			faceMaterials [i] = (Material)Resources.Load ("Materials/" + materialName, typeof(Material));
		}




		faces = new GameObject[2, System.Enum.GetNames (typeof(FaceSections)).Length];
		for (int i = 0; i < faces.GetLength(0); i++) {
			for (int j = 0; j < faces.GetLength(1); j++) {
				bool fish = parent.Equals (AnimatedFish);

				planeSize = fish ? 1.25f : 0.25f;

				FaceSections faceSection = System.Enum.GetValues (typeof(FaceSections)).GetValue (j) is FaceSections
                                               ? (FaceSections)System.Enum.GetValues (typeof(FaceSections)).GetValue (j)
                                               : FaceSections.EYES;

				faces [i, j] = createSimplePlane (i % 2 == 0);
				faces [i, j].name = (i % 2 == 0 ? "Back_" : "Front_") + faceSection.ToString ();


				//float zPos = bounds.min
				float xPos = 0, yPos = 0;


				switch (faceSection) {
				case FaceSections.MOUTH:
					xPos = fish ? -2.55f : -0.25f;
					yPos = fish ? 1.9f : -0.25f;
					break;
				case FaceSections.EYES:
					xPos = fish ? -2.75f : -0.25f;
					yPos = fish ? 3.0f : 0.25f;
					break;
				case FaceSections.DECORATION:
					xPos = fish ? -1f : 0.25f;
					yPos = fish ? 3.25f : 0.33f;

					break;
				}
				
				float zPos = 0.6f;

				faces [i, j].transform.parent = parent.transform;
				faces [i, j].transform.position = new Vector3 (xPos, yPos, (fish ? 1.8f : 1.01f) * (i % 2 == 0 ? 1:-0.25f )*zPos);
				faces [i, j].transform.Rotate (new Vector3 (0, (i % 2 == 0 ? 0 : 1) * 180f, 90));


				faces [i, j].renderer.material = faceMaterials [j];


				applyTexture (faces [i, j], faceSection);
			}
		}
	}

	private GameObject createSimplePlane (bool back)
	{

		Mesh m = new Mesh ();
		m.name = "decal";

		Vector3[] vertices = new Vector3[4];
		Vector2[] uvs = new Vector2[4];
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = new Vector3 ((i % 3 == 0 ? -1 : 1) * planeSize, (i < 2 ? -1 : 1) * planeSize, 0.01f); // (-1, -1, 0), (1,-1,0), (1, 1, 0), (-1, 1, 0)

			int uvIndex = back ? vertices.Length - i - 1 : i;
			uvs [uvIndex] = new Vector2 ((i < 2 ? 0 : 1), i % 3 == 0 ? 0 : 1);  // (0, 0), (0,1), (1,1), (1,0)
		}


		m.vertices = vertices;
		m.uv = uvs;
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
		m.RecalculateNormals ();
		GameObject obj = new GameObject ();
		obj.AddComponent<MeshRenderer> ();
		obj.AddComponent<MeshFilter> ();
		obj.AddComponent<MeshCollider> ();
		MeshFilter filter = (MeshFilter)obj.GetComponent (typeof(MeshFilter));
		filter.mesh = m;

		return obj;
	}
	
	void Update ()
	{
		if (AnimatedFish != null && switchFace) {
			for (int i = 0; i < currentTime.Length; i++) {
				currentTime [i] += Time.deltaTime;
				if (currentTime [i] > switchTime [i]) {
					if (i < 2)
						indices [i] = randomizer.Next (maxTextures [i]);
					else {
						indices [i]++;
						indices [i] %= maxTextures [i];
					}
					for (int j = 0; j < faces.GetLength(0); j++) {
						FaceSections faceSection = System.Enum.GetValues (typeof(FaceSections)).GetValue (i) is FaceSections ? (FaceSections)System.Enum.GetValues (typeof(FaceSections)).GetValue (i) : FaceSections.EYES;
						applyTexture (faces [j, i], faceSection);
						currentTime [i] = 0;
					}
				}
			}
		}

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel("PrototypeMainMenu");
        }

        if (Input.GetKeyDown(KeyCode.Menu)) {
            Application.Quit();
        }
	}
	
}

