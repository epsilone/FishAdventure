using com.funcom.legoxmlreader;
using Com.Funcom.FishAdventure.Component.Entity.Living.Type;
using UnityEngine;

public class TestReader:MonoBehaviour 
{
	public AnimationClip animationBuffer;
	private GameObject Fish;
	
	private const int NUMBER_OF_BUTTON = 5;
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 1) - 15,100,30),"Mario"))
		{
			InvokeFish("redmariofish");
		}
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 2) - 15,100,30),"Block"))
		{
			InvokeFish("fish");
		}
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 3) - 15,100,30),"Bus"))
		{
			InvokeFish("thebus");
		}
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 4) - 15,100,30),"Octopod"))
		{
			InvokeFish("octopod");
		}
	}
	
	private void InvokeFish(string xmlName)
	{
		if(Fish != null)
		{
			Destroy(Fish);
		}
		
        Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load(xmlName, typeof(TextAsset)) as TextAsset).text, "Fish");
		Fish.AddComponent("ViewDrag");
		GenericUtil.CombineChildMeshes(Fish);
		//GenericUtil.Center(Fish);
		GenericUtil.CreateBones(Fish);
		GenericUtil.SkinIt(Fish, 0.0f);
		
		//Animation
		Fish.AddComponent<Animation>();
		Fish.animation.AddClip(animationBuffer, "idle");
		Fish.animation.wrapMode = WrapMode.Loop;
	    Fish.animation.Play("idle");
	}
	
	void LateUpdate()
	{
		if(Fish != null)
		{
			Fish.GetComponent<SkinnedMeshRenderer>().bones[0].transform.Rotate(90, 0, 0);
		}
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / (NUMBER_OF_BUTTON + 1)) * 5) - 15,100,30),"horizontal"))
		{
			if(Fish != null)
			{
				Destroy(Fish);
			}
			
            Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("horizontalfish", typeof(TextAsset)) as TextAsset).text, "Fish");
			Fish.AddComponent("ViewDrag");
			//GenericUtil.CombineChildMeshes(Fish);
			//GenericUtil.CreateBones(Fish);
			//GenericUtil.SkinIt(Fish, 0.10f);
			//GenericUtil.Center(Fish);
			
			//Animation
			/*fishGameObject.AddComponent<Animation>();
			fishGameObject.animation.AddClip(animationBuffer, "idle");
			fishGameObject.animation.wrapMode = WrapMode.Loop;
		    fishGameObject.animation.Play("idle");*/
		}
	}
}
