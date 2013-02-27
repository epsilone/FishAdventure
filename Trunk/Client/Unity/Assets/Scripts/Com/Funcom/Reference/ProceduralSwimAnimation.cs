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

public class ProceduralSwimAnimation:MonoBehaviour 
{
	#region Member Variables
	public float scale = 0.3f;
	public float speed = 5.0f;
	private Vector3[] baseHeight;
	#endregion
	
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
		}
	}
}
