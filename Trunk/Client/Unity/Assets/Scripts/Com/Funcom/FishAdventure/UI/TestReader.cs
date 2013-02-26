using com.funcom.legoxmlreader;
using Com.Funcom.FishAdventure.Component.Entity.Living.Type;
using UnityEngine;

public class TestReader:MonoBehaviour 
{
	public AnimationClip animationBuffer;
	private GameObject Fish;
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / 5) * 1) - 15,100,30),"Mario"))
		{
			InvokeFish("redmariofish");
		}
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / 5) * 2) - 15,100,30),"Block"))
		{
			InvokeFish("fish");
		}
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / 5) * 3) - 15,100,30),"Bus"))
		{
			InvokeFish("thebus");
		}
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / 5) * 4) - 15,100,30),"Octopod"))
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
	}
}
