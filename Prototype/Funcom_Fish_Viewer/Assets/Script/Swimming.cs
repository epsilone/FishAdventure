using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]

public class Swimming:MonoBehaviour 
{
	public float scale = 0.5f;
	public float speed = 5.0f;
	private Vector3[] baseHeight;
	
	void Awake()
	{
		if(!GetComponent(typeof(MeshCollider))) { gameObject.AddComponent(typeof(MeshCollider)); }
		if(!GetComponent(typeof(MeshRenderer))) { gameObject.AddComponent(typeof(MeshRenderer)); }
		if(!GetComponent(typeof(MeshFilter))) { gameObject.AddComponent(typeof(MeshFilter)); }
	}
	
	void Start() 
	{
		regroupSubmesh();
	}
	
	void regroupSubmesh()
	{
		Mesh groupedMesh;
		MeshFilter mainMeshFilter;
		
		//Create mesh list and deactivate child object containing those mesh
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
	    CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];
	    for (int i = 1; i < meshFilters.Length; i++)
		{
	        combine[i - 1].mesh = meshFilters[i].sharedMesh;
	        combine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;
	        meshFilters[i].gameObject.SetActive(false);
	    }
		
		//Create the new mesh
		groupedMesh = new Mesh();
		groupedMesh.name = gameObject.name;
		groupedMesh.CombineMeshes(combine);
		
		//Apply the new mesh
		mainMeshFilter = GetComponent("MeshFilter") as MeshFilter;
		mainMeshFilter.mesh = groupedMesh;
		
		//Active the main gameobject
	    transform.gameObject.SetActive(true);
		
		//Positionning correctly vertices
		Vector3[] currentVertexList = groupedMesh.vertices;
		Vector3[] newVertexList = new Vector3[currentVertexList.Length];
		Vector3 vertexBuffer;
		for (var i=0;i<currentVertexList.Length;i++)
	    {
	        vertexBuffer = currentVertexList[i];
			vertexBuffer = transform.InverseTransformPoint(vertexBuffer);
			newVertexList[i] = vertexBuffer;
	    }
		groupedMesh.vertices = newVertexList;
		
		//Mesh optimization
		groupedMesh.Optimize();
		string test = "test";
	}
	
	void Update() 
	{
		
	}
	
	void FixedUpdate() 
	{
		Mesh mesh = (GetComponent("MeshFilter") as MeshFilter).mesh;
		
	    if (baseHeight == null)
		{
	    	baseHeight = mesh.vertices;
		}
	    
		float time = Time.time;
	    Vector3[] vertices = new Vector3[baseHeight.Length];
	
	    for (var i=0;i<vertices.Length;i++)
	    {
	        Vector3 vertex = baseHeight[i];
	        vertex.z += Mathf.Sin(time * speed + baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
	        vertices[i] = vertex;
	    }
	
	    mesh.vertices = vertices;
	    mesh.RecalculateNormals();
		(GetComponent("MeshCollider") as MeshCollider).sharedMesh = mesh;
	}
}