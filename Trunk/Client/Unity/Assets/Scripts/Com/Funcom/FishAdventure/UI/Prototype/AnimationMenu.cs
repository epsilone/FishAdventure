using com.funcom.legoxmlreader;
using UnityEngine;

public class AnimationMenu:MonoBehaviour 
{
	public AnimationClip swimAnimation;
	private GameObject Fish;
	private GameObject PreSkinnedMesh;
	private GameObject CustomMesh;
	private GameObject CustomMeshProcSkinning;
	private GameObject SkinningPrefab;
	
	private int currentState = -1;
	
	private const int NUMBER_OF_BUTTON = 5;
	private const float BTN_WIDTH = 200.0f;
	private const float BTN_HEIGHT = 60.0f;
	
	void Start()
	{
		PreSkinnedMesh = GameObject.Find("PreSkinned_Mesh");
		PreSkinnedMesh.AddComponent("ViewDrag");
		//PreSkinnedMesh.SetActive(false);
		
		CustomMesh = GameObject.Find("Custom_Mesh");
		CustomMesh.AddComponent("ViewDrag");
		CustomMesh.SetActive(false);
		
		CustomMeshProcSkinning = GameObject.Find("Custom_Mesh_ProceduralSkinning");
		CustomMeshProcSkinning.AddComponent("ViewDrag");
		CustomMeshProcSkinning.SetActive(false);
		
		SkinningPrefab = GameObject.Find("SkinningPrefab");
		SkinningPrefab.AddComponent("ViewDrag");
		SkinningPrefab.SetActive(false);
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
		
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"100% Procedural animation\n(Not optimized)"))
		{
			if(ChangeState(2) == false) {return;}
			
			InvokeFish("redmariofish");
			GenericUtil.CombineChildMeshesToFilter(Fish);
			Fish.AddComponent("ProceduralSwimAnimation");
			Fish.AddComponent("ViewDrag");
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Legacy]\nProcedural Skinning"))
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
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Mecanim Generic]\n No Skinning"))
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
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Mecanim Generic]\n Procedural Skinning"))
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
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 5) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"[Mecanim Generic]\nProcedural Skinning on Prefab"))
		{
			if(ChangeState(6) == false) {return;}
			
			InvokeFish("redmariofish");
			
			SkinningPrefab.SetActive(true);
			
			Transform[] allChildren = Fish.GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren) 
			{
				if(child.parent == Fish.transform)
				{
					child.parent = SkinningPrefab.transform;
				}
			}
			
			Destroy(Fish);
			
			GenericUtil.CombineChildMeshesToSkin(SkinningPrefab);
			
			SkinnedMeshRenderer skinnedMeshRenderer = SkinningPrefab.GetComponent<SkinnedMeshRenderer>();
			
			//BONES CREATION
		    Transform[] bones = new Transform[4];
		    Matrix4x4[] bindPoses = new Matrix4x4[4];
			
		    bones[0] = SkinningPrefab.transform.Find("Bone01").transform;
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
			
			GenericUtil.SkinIt(SkinningPrefab, 0.15f);
			
			//Animation
			SkinningPrefab.animation.wrapMode = WrapMode.Loop;
			SkinningPrefab.animation.Play("swim");
		}
		
		
		
		if (GUI.Button(new Rect(0,Screen.height - 40 , 100, 40),"Main Menu"))
		{
			Debug.Log("GoTo: PrototypeMainMenu");
			Application.LoadLevel("PrototypeMainMenu");
		}
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
		//PreSkinnedMesh.SetActive(false);
		
		
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
	}
}

