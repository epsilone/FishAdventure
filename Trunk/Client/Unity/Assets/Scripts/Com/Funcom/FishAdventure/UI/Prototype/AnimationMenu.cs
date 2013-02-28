using com.funcom.legoxmlreader;
using UnityEngine;

public class AnimationMenu:MonoBehaviour 
{
	public AnimationClip swimAnimation;
	private GameObject Fish;
	private GameObject PreSkinnedMesh;
	
	private const int NUMBER_OF_BUTTON = 5;
	private const float BTN_WIDTH = 200.0f;
	private const float BTN_HEIGHT = 60.0f;
	
	void Start()
	{
		PreSkinnedMesh = GameObject.Find("swim_4bones_with_mesh");
		PreSkinnedMesh.AddComponent("ViewDrag");
		PreSkinnedMesh.SetActive(false);
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,0,Screen.width,30),"Animation Prototype");
		
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Fully Procedural"))
		{
			InvokeFish("redmariofish");
			GenericUtil.CombineChildMeshesToFilter(Fish);
			Fish.AddComponent("ProceduralSwimAnimation");
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Pre-Skinned"))
		{
			if(Fish != null)
			{
				Destroy(Fish);
			}
			PreSkinnedMesh.SetActive(true);
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Procedural Skinning"))
		{
			InvokeFish("redmariofish");
			
			GenericUtil.CombineChildMeshesToFilter(Fish);
			//GenericUtil.CombineChildMeshesToSkin(Fish);
			//GenericUtil.Center(Fish);
			//GenericUtil.CreateBones(Fish);
			//GenericUtil.SkinIt(Fish, 0.0f);
			
			//Animation
			/*Fish.AddComponent<Animation>();
			Fish.animation.AddClip(swimAnimation, "swim");
			Fish.animation.wrapMode = WrapMode.Loop;
		    Fish.animation.Play("swim");*/
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Mecanim Generic"))
		{
			
		}
		if (GUI.Button(new Rect(Screen.width - BTN_WIDTH, (((float)Screen.height / (NUMBER_OF_BUTTON + 1)) * 5) - (BTN_HEIGHT * 0.5f),BTN_WIDTH,BTN_HEIGHT),"Mecanim Humanoide"))
		{
			
		}
		
		if (GUI.Button(new Rect(0,Screen.height - 40 , 100, 40),"Main Menu"))
		{
			Debug.Log("GoTo: PrototypeMainMenu");
			Application.LoadLevel("PrototypeMainMenu");
		}
	}
	
	private void InvokeFish(string xmlName)
	{
		if(Fish != null)
		{
			Destroy(Fish);
		}
		PreSkinnedMesh.SetActive(false);
		
        Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("XML/BaseFish/" + xmlName, typeof(TextAsset)) as TextAsset).text, "Fish");
		Fish.AddComponent("ViewDrag");
	}
}

