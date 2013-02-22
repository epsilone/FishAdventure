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
			if(Fish != null)
			{
				Destroy(Fish);
			}
			
            Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("redmariofish", typeof(TextAsset)) as TextAsset).text, "Fish");
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
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / 5) * 2) - 15,100,30),"Block"))
		{
			if(Fish != null)
			{
				Destroy(Fish);
			}
			
            Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("fish", typeof(TextAsset)) as TextAsset).text, "Fish");
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
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / 5) * 3) - 15,100,30),"Bus"))
		{
			if(Fish != null)
			{
				Destroy(Fish);
			}
			
            Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("thebus", typeof(TextAsset)) as TextAsset).text, "Fish");
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
		
		if (GUI.Button(new Rect(Screen.width-100,((Screen.height / 5) * 4) - 15,100,30),"Octopod"))
		{
			if(Fish != null)
			{
				Destroy(Fish);
			}
			
            Fish = LEGOXMLReader.GetLegoObjectByLxfml((Resources.Load("octopod", typeof(TextAsset)) as TextAsset).text, "Fish");
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
