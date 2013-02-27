#region Copyright
/***************************************************
Author: Keven Poulin
Company: Funcom
Project: FishAdventure (LEGO)
****************************************************/
#endregion

#region Usings
using UnityEngine;
using System.Collections;
#endregion

namespace SwimBehaviour
{ 
	public class SwimBehaviour:MonoBehaviour 
	{
		#region Member Variables
		Hashtable basetweenHT = new Hashtable();
		int xTranslation = 60;
		
		public float scale = 0.3f;
		public float speed = 5.0f;
		private Vector3[] baseHeight;
		#endregion
		
		void Awake () 
		{
			basetweenHT.Add("time",2);
			basetweenHT.Add("delay",0);
			basetweenHT.Add("onupdate","onSwim");
			basetweenHT.Add("oncomplete","onComplete");
			basetweenHT.Add("looptype",iTween.LoopType.none);
			basetweenHT.Add("easeType",iTween.EaseType.easeInOutCubic);
		}
		
		void Start () 
		{
			xTranslation = -xTranslation;
			Hashtable tween = basetweenHT.Clone() as Hashtable;
			tween.Add("x",xTranslation);
			//iTween.MoveTo(gameObject, tween);
		}
		
		void Update () 
		{
		
		}
		
		public void onSwim()
		{
			Debug.Log("onSwim");
		}
		
		public void onComplete()
		{
			Debug.Log("onComplete");
			
			xTranslation = -xTranslation;
			Hashtable tween = basetweenHT.Clone() as Hashtable;
			tween.Add("x",xTranslation);
			iTween.MoveTo(gameObject, tween);
		}
		
		void FixedUpdate() 
		{
			Component[] meshList = GetComponentsInChildren(typeof(MeshFilter));
			foreach(MeshFilter meshfilter in meshList)
			{
				Mesh mesh = meshfilter.mesh;
				//Mesh mesh = (meshList[0] as MeshFilter).mesh;
				
				if (baseHeight == null)
				{
			    	baseHeight = mesh.vertices;
				}
			    
				float time = Time.time;
			    Vector3[] vertices = new Vector3[baseHeight.Length];
			
			    for (var i=0;i<vertices.Length;i++)
			    {
			        Vector3 vertex = baseHeight[i];
			        vertex.z += Mathf.Sin(time * speed - baseHeight[i].x/* + baseHeight[i].y + baseHeight[i].z*/) * scale;
			        vertices[i] = vertex;
			    }
			
			    mesh.vertices = vertices;
			    mesh.RecalculateNormals();
				//(GetComponent("MeshCollider") as MeshCollider).sharedMesh = mesh;
			}
		}
	}
}
