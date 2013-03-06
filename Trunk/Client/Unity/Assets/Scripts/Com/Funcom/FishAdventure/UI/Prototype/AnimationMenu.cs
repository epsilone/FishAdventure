using com.funcom.legoxmlreader;
using UnityEngine;
using System;
using System.Collections;

public class AnimationMenu:MonoBehaviour 
{
	public GameObject rig;
	public AnimationClip swimAnimation;
	private GameObject Fish;
	private GameObject PreSkinnedMesh;
	private GameObject CustomMesh;
	private GameObject CustomMeshProcSkinning;
	private GameObject SkinningPrefab;
	private GameObject SkinningPrefabProcAnim;
	
	
	Transform[] bonesRef;
	
	private int currentState = -1;
	
	private const int NUMBER_OF_BUTTON = 6;
	private const float BTN_WIDTH = 250.0f;
	private const float BTN_HEIGHT = 60.0f;
	
	void Start()
	{
		PreSkinnedMesh = GameObject.Find("PreSkinned_Mesh");
		PreSkinnedMesh.AddComponent("ViewDrag");
		PreSkinnedMesh.SetActive(false);
		
		CustomMesh = GameObject.Find("Custom_Mesh");
		CustomMesh.AddComponent("ViewDrag");
		CustomMesh.SetActive(false);
		
		CustomMeshProcSkinning = GameObject.Find("Custom_Mesh_ProceduralSkinning");
		CustomMeshProcSkinning.AddComponent("ViewDrag");
		CustomMeshProcSkinning.SetActive(false);
		
		SkinningPrefab = GameObject.Find("SkinningPrefab");
		SkinningPrefab.AddComponent("ViewDrag");
		SkinningPrefab.SetActive(false);
		
		SkinningPrefabProcAnim = GameObject.Find("SkinningPrefabProcAnim");
		SkinningPrefabProcAnim.AddComponent("ViewDrag");
		SkinningPrefabProcAnim.SetActive(false);
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,0,Screen.width,30),"Animation Prototype");
		
		if (GUI.Button(new Rect(0, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Preskinned Mesh\n(The Goal)"))
		{
			if(ChangeState(1) == false) {return;}
			
			ClearPreviousInstance();
			PreSkinnedMesh.SetActive(true);
		}
		
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Procedural Animation]\nNo Skinning (Invoke)"))
		{
			if(ChangeState(2) == false) {return;}
			
			InvokeFish("redmariofish");
			GenericUtil.CombineChildMeshesToFilter(Fish);
			Fish.AddComponent("ProceduralSwimAnimation");
			Fish.AddComponent("ViewDrag");
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Legacy]\nProcedural Skinning (Invoke)"))
		{
			if(ChangeState(3) == false) {return;}
			
			InvokeFish("redmariofish");
			Fish.AddComponent("ViewDrag");
			
			GenericUtil.CombineChildMeshesToSkin(Fish);
			//GenericUtil.Center(Fish);
			GenericUtil.CreateBones(Fish);
			GenericUtil.SkinIt(Fish, 0.20f);
			
			//Animation
			Fish.AddComponent<Animation>();
			Fish.animation.AddClip(swimAnimation, "swim");
			Fish.animation.wrapMode = WrapMode.Loop;
		    Fish.animation.Play("swim");
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Mecanim Generic]\n No Skinning (GameObject)"))
		{
			if(ChangeState(4) == false) {return;}
			
			InvokeFish("redmariofish");
			
			Transform[] allChildren = Fish.GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren) 
			{
				if(child.parent == Fish.transform)
				{
					child.parent = CustomMesh.transform;
				}
			}
			
			CustomMesh.SetActive(true);
			Destroy(Fish);
			
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Mecanim Generic]\n Procedural Skinning (Invoke)"))
		{
			if(ChangeState(5) == false) {return;}
			
			InvokeFish("redmariofish");
			
			CustomMeshProcSkinning.SetActive(true);
			GenericUtil.CombineChildMeshesToSkin(Fish);
			SkinnedMeshRenderer skinnedMeshRenderer = Fish.GetComponent<SkinnedMeshRenderer>();
			
			//BONES CREATION
		    Transform[] bones = new Transform[4];
		    Matrix4x4[] bindPoses = new Matrix4x4[4];
			
		    bones[0] = CustomMeshProcSkinning.transform.Find("Bone01").transform;
			bindPoses[0] = CustomMeshProcSkinning.transform.worldToLocalMatrix;
			bones[1] = bones[0].FindChild("Bone02").transform;
			bindPoses[1] = bones[0].transform.worldToLocalMatrix;
			bones[2] = bones[1].FindChild("Bone03").transform;
			bindPoses[2] = bones[1].transform.worldToLocalMatrix;
			bones[3] = bones[2].FindChild("Bone04").transform;
			bindPoses[3] = bones[2].transform.worldToLocalMatrix;
		
		    skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
		    skinnedMeshRenderer.bones = bones;
			skinnedMeshRenderer.rootBone = bones[0];
			
			GenericUtil.SkinIt(Fish, 0.20f);
			
			Fish.transform.parent = CustomMeshProcSkinning.transform;
			
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 5) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Legacy]\nProcedural Skinning (GameObject)"))
		{
			if(ChangeState(6) == false) {return;}
			
			InvokeFish("redmariofish");
			
			SkinningPrefab.SetActive(true);
			
			//Set var
			SkinnedMeshRenderer skinnedMeshRenderer;
			Matrix4x4 matrixBuffer;
			Vector3 size;
		    Transform[] bonesRefList = new Transform[4];
		    Matrix4x4[] bindPoses = new Matrix4x4[4];
			
			//Move fish object to prefab
			Transform[] allChildren = Fish.GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren) 
			{
				if(child.parent == Fish.transform)
				{
					child.parent = SkinningPrefab.transform;
				}
			}
			
			//Destroy fish
			Destroy(Fish);
			
			//Combine children mesh to parent (Skinning mode)
			GenericUtil.CombineChildMeshesToSkin(SkinningPrefab);
			skinnedMeshRenderer = SkinningPrefab.GetComponent<SkinnedMeshRenderer>();
			
			//Get size
			size = skinnedMeshRenderer.sharedMesh.bounds.size;
			
			//Get bones
			bonesRefList[0] = SkinningPrefab.transform.Find("Bone01").transform;
			bonesRefList[1] = bonesRefList[0].FindChild("Bone02").transform;
			bonesRefList[2] = bonesRefList[1].FindChild("Bone03").transform;
			bonesRefList[3] = bonesRefList[2].FindChild("Bone04").transform;
			
			//Move bones
			bonesRefList[0].position = new Vector3((size.x * 0.5f), size.y * 0.5f, 0);
			bonesRefList[1].position = bonesRefList[0].position - new Vector3(3.6f,0,0);
			bonesRefList[2].position = bonesRefList[1].position - new Vector3(3.6f,0,0);
			bonesRefList[3].position = bonesRefList[2].position - new Vector3(3.6f,0,0);
			
			/*bonesRefList[0].position = new Vector3(-(size.x * 0.5f), size.y * 0.5f, 0);
			bonesRefList[1].position = bonesRefList[0].position + new Vector3(3.6f,0,0);
			bonesRefList[2].position = bonesRefList[1].position + new Vector3(3.6f,0,0);
			bonesRefList[3].position = bonesRefList[2].position + new Vector3(3.6f,0,0);*/
			
			//Bones rotation (HACK)
			bonesRefList[0].localEulerAngles = new Vector3(-90f,0f,0f);
			
			//test 3
			bindPoses[0] = bonesRefList[0].worldToLocalMatrix;
			bindPoses[1] = bonesRefList[1].worldToLocalMatrix;
			bindPoses[2] = bonesRefList[2].worldToLocalMatrix;
			bindPoses[3] = bonesRefList[3].worldToLocalMatrix;
			
			//Populate bones info
			skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
		    skinnedMeshRenderer.bones = bonesRefList;
			skinnedMeshRenderer.rootBone = bonesRefList[0];
			
			//Skinning
			GenericUtil.SkinIt(SkinningPrefab, 0f);
			
			//Animation
			SkinningPrefab.animation.wrapMode = WrapMode.Loop;
			SkinningPrefab.animation.Play("swim");
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 6) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Procedural Animation]\nProcedural Skinning (GameObject)"))
		{
			if(ChangeState(7) == false) {return;}
			
			InvokeFish("redmariofish");
			
			SkinningPrefabProcAnim.SetActive(true);
			
			//Set var
			SkinnedMeshRenderer skinnedMeshRenderer;
			Vector3 size;
		    bonesRef = new Transform[4];
		    Matrix4x4[] bindPoses = new Matrix4x4[4];
			
			//Move fish object to prefab
			Transform[] allChildren = Fish.GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren) 
			{
				if(child.parent == Fish.transform)
				{
					child.parent = SkinningPrefabProcAnim.transform;
				}
			}
			
			//Destroy fish
			Destroy(Fish);
			
			//Combine children mesh to parent (Skinning mode)
			GenericUtil.CombineChildMeshesToSkin(SkinningPrefabProcAnim);
			skinnedMeshRenderer = SkinningPrefabProcAnim.GetComponent<SkinnedMeshRenderer>();
			
			//Get size
			size = skinnedMeshRenderer.sharedMesh.bounds.size;
			
			//Get bones
			bonesRef[0] = SkinningPrefabProcAnim.transform.Find("Bone01").transform;
			bonesRef[1] = bonesRef[0].FindChild("Bone02").transform;
			bonesRef[2] = bonesRef[1].FindChild("Bone03").transform;
			bonesRef[3] = bonesRef[2].FindChild("Bone04").transform;
			
			//Move bones
			bonesRef[0].position = new Vector3(-(size.x / 2f) + ((size.x / 3f) * 0), size.y / 2f, 0f);
			bonesRef[1].position = new Vector3(-(size.x / 2f) + ((size.x / 3f) * 1), size.y / 2f, 0f);
			bonesRef[2].position = new Vector3(-(size.x / 2f) + ((size.x / 3f) * 2), size.y / 2f, 0f);
			bonesRef[3].position = new Vector3(-(size.x / 2f) + ((size.x / 3f) * 3), size.y / 2f, 0f);
			
			//BindPoses
			bindPoses[0] = bonesRef[0].worldToLocalMatrix;
			bindPoses[1] = bonesRef[1].worldToLocalMatrix;
			bindPoses[2] = bonesRef[2].worldToLocalMatrix;
			bindPoses[3] = bonesRef[3].worldToLocalMatrix;
			
			//Populate bones info
			skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
		    skinnedMeshRenderer.bones = bonesRef;
			skinnedMeshRenderer.rootBone = bonesRef[0];
			
			//Skinning
			GenericUtil.SkinIt(SkinningPrefabProcAnim, 0f);
			
			//Procedural animation
			Debug.Log("Start");
			Hashtable hastTable = new Hashtable();
			hastTable.Add("amount", new Vector3(0f,0.03f,0f));
			hastTable.Add("time", 1f);
			hastTable.Add("easetype", EaseType.easeInBack);
			hastTable.Add("delay", 1f);
			
			iTween.RotateBy(bonesRef[0].gameObject, hastTable);
			iTween.RotateBy(bonesRef[1].gameObject, hastTable);
			iTween.RotateBy(bonesRef[2].gameObject, hastTable);
			iTween.RotateBy(bonesRef[3].gameObject, hastTable);
			
			
			hastTable = new Hashtable();
			hastTable.Add("amount", new Vector3(0f,-0.06f,0f));
			hastTable.Add("time", 2f);
			hastTable.Add("easetype", EaseType.easeInBack);
			hastTable.Add("delay", 2f);
			
			iTween.RotateBy(bonesRef[0].gameObject, hastTable);
			iTween.RotateBy(bonesRef[1].gameObject, hastTable);
			iTween.RotateBy(bonesRef[2].gameObject, hastTable);
			iTween.RotateBy(bonesRef[3].gameObject, hastTable);
			
			
			hastTable = new Hashtable();
			hastTable.Add("amount", new Vector3(0f,0.06f,0f));
			hastTable.Add("time", 2f);
			hastTable.Add("easetype", EaseType.easeInBack);
			hastTable.Add("delay", 4f);
			
			iTween.RotateBy(bonesRef[0].gameObject, hastTable);
			iTween.RotateBy(bonesRef[1].gameObject, hastTable);
			iTween.RotateBy(bonesRef[2].gameObject, hastTable);
			iTween.RotateBy(bonesRef[3].gameObject, hastTable);
		}
		
		
		if (GUI.Button(new Rect(0,Screen.height - 40 , 100, 40),"Main Menu"))
		{
			Debug.Log("GoTo: PrototypeMainMenu");
			Application.LoadLevel("PrototypeMainMenu");
		}
	}
	
	public void segmentOneCompleted()
	{
		Hashtable hastTable = new Hashtable();
		hastTable.Add("amount", new Vector3(0f,-0.06f,0f));
		hastTable.Add("time", 10f);
		hastTable.Add("easetype", EaseType.linear);
		hastTable.Add("oncomplete", "segmentTwoCompleted");
		
		iTween.RotateBy(bonesRef[0].gameObject, hastTable);
		iTween.RotateBy(bonesRef[1].gameObject, hastTable);
		iTween.RotateBy(bonesRef[2].gameObject, hastTable);
		iTween.RotateBy(bonesRef[3].gameObject, hastTable);
	}
	
	public void segmentTwoCompleted()
	{
		Hashtable hastTable = new Hashtable();
		hastTable.Add("amount", new Vector3(0f,0.06f,0f));
		hastTable.Add("time", 10f);
		hastTable.Add("easetype", EaseType.linear);
		hastTable.Add("oncomplete", "segmentOneCompleted");
		
		iTween.RotateBy(bonesRef[0].gameObject, hastTable);
		iTween.RotateBy(bonesRef[1].gameObject, hastTable);
		iTween.RotateBy(bonesRef[2].gameObject, hastTable);
		iTween.RotateBy(bonesRef[3].gameObject, hastTable);
	}
	
	private bool ChangeState(int stateId)
	{
		if(currentState == stateId)
		{
			return false;
		}
		else
		{
			currentState = stateId;
			return true;
		}
	}
	
	private void InvokeFish(string xmlName)
	{
		ClearPreviousInstance();
		
        Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("XML/BaseFish/" + xmlName, typeof(TextAsset)) as TextAsset).text, "Fish");
	}
	
	private void ClearPreviousInstance()
	{
		if(Fish != null)
		{
			Destroy(Fish);
		}
		PreSkinnedMesh.SetActive(false);
		
		
		Transform[] allChildren = CustomMesh.GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren) 
		{
			if(child.parent == CustomMesh.transform && child.name != "Bone01")
			{
				Debug.Log("Destroyed");
				Destroy(child.gameObject);
			}
		}
		CustomMesh.SetActive(false);
		
		CustomMeshProcSkinning.SetActive(false);
		
		Transform[] allChildren2 = SkinningPrefab.GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren2) 
		{
			if(child.parent == SkinningPrefab.transform && child.name != "Bone01")
			{
				Debug.Log("Destroyed");
				Destroy(child.gameObject);
			}
		}
		
		Destroy(SkinningPrefab.GetComponent<SkinnedMeshRenderer>());
		SkinningPrefab.SetActive(false);
		
		Transform[] allChildren3 = SkinningPrefabProcAnim.GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren3) 
		{
			if(child.parent == SkinningPrefabProcAnim.transform && child.name != "Bone01")
			{
				Debug.Log("Destroyed");
				Destroy(child.gameObject);
			}
		}
		
		Destroy(SkinningPrefabProcAnim.GetComponent<SkinnedMeshRenderer>());
		SkinningPrefabProcAnim.SetActive(false);
	}	
	
	void Update()
	{
		if(SkinningPrefab != null)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = SkinningPrefab.GetComponent<SkinnedMeshRenderer>();
			if(skinnedMeshRenderer != null)
			{
				//Debug.Log(skinnedMeshRenderer.sharedMesh.bindposes[0]);
				//Debug.Log(skinnedMeshRenderer.bones[0].localToWorldMatrix);
			}
		}
		
	}
}

