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
using System.Collections.Generic;
#endregion

public static class GenericUtil 
{
	#region Member Variables
	
	#endregion

	public static void CombineChildMeshesToSkin(GameObject obj)
	{
		// Find all mesh filter submeshes and separate them by their cooresponding materials
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
		
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();

        foreach( MeshFilter meshFilter in meshFilters )
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            // Handle bad input
            if(!meshRenderer) { 
                Debug.LogError("MeshFilter does not have a coresponding MeshRenderer."); 
                continue; 
            }
            if(meshRenderer.materials.Length != meshFilter.sharedMesh.subMeshCount) { 
                Debug.LogError("Mismatch between material count and submesh count. Is this the correct MeshRenderer?"); 
                continue; 
            }

            for(int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = 0;
                for(materialArrayIndex = 0; materialArrayIndex < materials.Count; materialArrayIndex++)
                {
                    if(materials[materialArrayIndex] == meshRenderer.sharedMaterials[s])
                        break;
                }

                if(materialArrayIndex == materials.Count)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    combineInstanceArrays.Add(new ArrayList());
                }

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add( combineInstance );
				
				GameObject childGameObject = meshFilter.gameObject as GameObject;
				childGameObject.SetActive(false);
            }
        }
 
        // For MeshFilter
        {
            // Get / Create mesh filter
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            if(!skinnedMeshRenderer)
                skinnedMeshRenderer = obj.AddComponent<SkinnedMeshRenderer>();
 
            // Combine by material index into per-material meshes
            // also, Create CombineInstance array for next step
            Mesh[] meshes = new Mesh[materials.Count];
            CombineInstance[] combineInstances = new CombineInstance[materials.Count];
 
            for( int m = 0; m < materials.Count; m++ )
            {
                CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
                meshes[m] = new Mesh();
                meshes[m].CombineMeshes( combineInstanceArray, true, true );
 
                combineInstances[m] = new CombineInstance();
                combineInstances[m].mesh = meshes[m];
                combineInstances[m].subMeshIndex = 0;
            }
 
            // Combine into one
            skinnedMeshRenderer.sharedMesh = new Mesh();
            skinnedMeshRenderer.sharedMesh.CombineMeshes( combineInstances, false, false );
 
            // Destroy other meshes
            /*foreach( Mesh mesh in meshes )
            {
                mesh.Clear();
                DestroyImmediate(mesh);
            }*/
			
			// Assign materials
            Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
            skinnedMeshRenderer.materials = materialsArray;
        }
            
	}
	
	public static void CombineChildMeshesToFilter(GameObject obj)
	{
		// Find all mesh filter submeshes and separate them by their cooresponding materials
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
		
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();

        foreach( MeshFilter meshFilter in meshFilters )
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            // Handle bad input
            if(!meshRenderer) { 
                Debug.LogError("MeshFilter does not have a coresponding MeshRenderer."); 
                continue; 
            }
            if(meshRenderer.materials.Length != meshFilter.sharedMesh.subMeshCount) { 
                Debug.LogError("Mismatch between material count and submesh count. Is this the correct MeshRenderer?"); 
                continue; 
            }

            for(int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = 0;
                for(materialArrayIndex = 0; materialArrayIndex < materials.Count; materialArrayIndex++)
                {
                    if(materials[materialArrayIndex] == meshRenderer.sharedMaterials[s])
                        break;
                }

                if(materialArrayIndex == materials.Count)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    combineInstanceArrays.Add(new ArrayList());
                }

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add( combineInstance );
				
				GameObject childGameObject = meshFilter.gameObject as GameObject;
				childGameObject.SetActive(false);
            }
        }
 
        // For MeshFilter
        {
            // Get / Create mesh filter
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if(!meshFilter)
                meshFilter = obj.AddComponent<MeshFilter>();
			
			MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if(!meshRenderer)
                meshRenderer = obj.AddComponent<MeshRenderer>();
 
            // Combine by material index into per-material meshes
            // also, Create CombineInstance array for next step
            Mesh[] meshes = new Mesh[materials.Count];
            CombineInstance[] combineInstances = new CombineInstance[materials.Count];
 
            for( int m = 0; m < materials.Count; m++ )
            {
                CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
                meshes[m] = new Mesh();
                meshes[m].CombineMeshes( combineInstanceArray, true, true );
 
                combineInstances[m] = new CombineInstance();
                combineInstances[m].mesh = meshes[m];
                combineInstances[m].subMeshIndex = 0;
            }
 
            // Combine into one
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.CombineMeshes( combineInstances, false, false );
 
            // Destroy other meshes
            /*foreach( Mesh mesh in meshes )
            {
                mesh.Clear();
                DestroyImmediate(mesh);
            }*/
			
			// Assign materials
            Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
            meshRenderer.materials = materialsArray;
        }
            
	}
	
	public static void CreateBones(GameObject obj)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
        if(!skinnedMeshRenderer)
		{
        	Debug.Log("CreateBones() - There is no mesh to apply bones on!");
        	return;
		}
		
		Vector3 size = skinnedMeshRenderer.sharedMesh.bounds.size;
		
		//BONES CREATION
	    Transform[] bones = new Transform[4];
	    Matrix4x4[] bindPoses = new Matrix4x4[4];
	
	    bones[0] = new GameObject ("Bone01").transform;
	    bones[0].parent = obj.transform;
	    //bones[0].localRotation = Quaternion.identity;
	    //bones[0].localPosition = new Vector3 (0, 0, size.z * 0.5f);
	    //bindPoses[0] = bones[0].worldToLocalMatrix * obj.transform.localToWorldMatrix;
		bindPoses[0] = obj.transform.localToWorldMatrix;
		
		bones[1] = new GameObject ("Bone02").transform;
	    bones[1].parent = bones[0].transform;
	   	//bones[1].localRotation = Quaternion.identity;
	    //bones[1].localPosition = new Vector3 (size.x * 0.33f, 0, 0);
	    //bindPoses[1] = bones[1].worldToLocalMatrix * bones[0].transform.localToWorldMatrix;
		bindPoses[1] = bones[0].transform.localToWorldMatrix;
		
		bones[2] = new GameObject ("Bone03").transform;
	    bones[2].parent = bones[1].transform;
	    //bones[2].localRotation = Quaternion.identity;
	    //bones[2].localPosition = new Vector3 (size.x * 0.33f, 0, 0);
	    //bindPoses[2] = bones[2].worldToLocalMatrix * bones[1].transform.localToWorldMatrix;
		bindPoses[2] = bones[1].transform.localToWorldMatrix;
		
		bones[3] = new GameObject ("Bone4").transform;
	    bones[3].parent = bones[2].transform;
	   	//bones[3].localRotation = Quaternion.identity;
	    //bones[3].localPosition = new Vector3 (size.x * 0.33f, 0, 0);
	    //bindPoses[3] = bones[3].worldToLocalMatrix * bones[2].transform.localToWorldMatrix;
		bindPoses[3] = bones[2].transform.localToWorldMatrix;
	
	    skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
	    skinnedMeshRenderer.bones = bones;
		skinnedMeshRenderer.rootBone = bones[0];
	}
	
	public static void SkinIt (GameObject obj, float minimalBoneWeight)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
	    if(!skinnedMeshRenderer)
		{
	    	skinnedMeshRenderer = obj.AddComponent<SkinnedMeshRenderer>();
		}
		
		Transform[] boneList = skinnedMeshRenderer.bones;
		Vector3[] verticeList = skinnedMeshRenderer.sharedMesh.vertices;
		BoneWeight[] boneWeightList = new BoneWeight[verticeList.Length];
		Transform boneBuffer;
		Vector3 verticeBuffer;
		float distanceBuffer;
		float weightBuffer;
		float distanceCumulative;
		
		for(int verticeIndex = 0; verticeIndex < verticeList.Length; verticeIndex++)
		{
			verticeBuffer = verticeList[verticeIndex];
			
			List<Vector2> boneWeightByVertice = new List<Vector2>();
			distanceCumulative = 0;
			
			//Calculate distance for each bone
			for(int boneIndex = 0; boneIndex < boneList.Length; boneIndex++)
			{
				boneBuffer = boneList[boneIndex];
				distanceBuffer = Vector3.Distance(verticeBuffer, boneBuffer.position);
				boneWeightByVertice.Add(new Vector2(boneIndex, distanceBuffer));
				distanceCumulative += distanceBuffer;
			}
			
			//Calculate weight for each bone
			for(int boneWeightIndex = 0; boneWeightIndex < boneWeightByVertice.Count; boneWeightIndex++)
			{
				/*if((verticeIndex % 40) == 0)
				{
					Debug.Log("[ DEBUG ][Distance] - VertexId=" + verticeIndex +  " / BoneId=" + boneWeightByVertice[boneWeightIndex].x + " / Distance=" + boneWeightByVertice[boneWeightIndex].y);
				}*/
				
				weightBuffer =  (distanceCumulative - boneWeightByVertice[boneWeightIndex].y);
				weightBuffer = (weightBuffer / (boneWeightByVertice.Count - 1));
				weightBuffer = (weightBuffer / distanceCumulative);
				boneWeightByVertice[boneWeightIndex] = new Vector2(boneWeightByVertice[boneWeightIndex].x, weightBuffer);
			}
			
			boneWeightList[verticeIndex] = GetBoneWeightByVerticeInfo(boneWeightByVertice, minimalBoneWeight);
			
			/*if((verticeIndex % 40) == 0)
			{
				foreach(Vector2 vec in boneWeightByVertice)
				{
					Debug.Log("[ DEBUG ][Weight] VertexId=" + verticeIndex +  " / BoneId=" + vec.x + " / Weight=" + vec.y);
				}
			}*/
		}
		
		skinnedMeshRenderer.sharedMesh.boneWeights = boneWeightList;
	}
	
	public static void Center (GameObject obj)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
		if(!skinnedMeshRenderer)
		{
	    	return;
		}
		
		Vector3 size = skinnedMeshRenderer.sharedMesh.bounds.size;
		Vector3 position = skinnedMeshRenderer.transform.position;
		Debug.Log("size: " + size);
		Debug.Log("position: " + position);
		Vector3[] verticeList = skinnedMeshRenderer.sharedMesh.vertices;
		Vector3[] newVerticeList = new Vector3[verticeList.Length];
		
		for(int vIndex = 0; vIndex < verticeList.Length; vIndex++)
		{
			Vector3 verticeBuffer = new Vector3();
			
			verticeBuffer.x = verticeList[vIndex].x + (size.x * 0.5f);
			verticeBuffer.y = verticeList[vIndex].y - (size.y * 0.5f);
			verticeBuffer.z = verticeList[vIndex].z - 2.85f;//(size.z * 0.5f);
			
			newVerticeList[vIndex] = verticeBuffer;
		}
		
		skinnedMeshRenderer.sharedMesh.vertices = newVerticeList;
	}
	
	private static BoneWeight GetBoneWeightByVerticeInfo(List<Vector2> boneWeightByVertice, float minimalBoneWeight)
	{
		BoneWeight boneWeight = new BoneWeight();
		float newMaxWeight = 0.0f;
		
		//Sort
		boneWeightByVertice.Sort((x, y) => x.y.CompareTo(y.y));
		boneWeightByVertice.RemoveRange(0,2);
		
		//Remove weight under the minimum limit
		//boneWeightByVertice.RemoveAll(x => x.y < minimalBoneWeight);
		
		//Recalibrate weight
		for(int i = 0; i < boneWeightByVertice.Count; i++)
		{
			newMaxWeight += boneWeightByVertice[i].y;
		}
		
		for(int i = 0; i < boneWeightByVertice.Count; i++)
		{
			boneWeightByVertice[i] = new Vector2(boneWeightByVertice[i].x, boneWeightByVertice[i].y / newMaxWeight);
		}
		
		//Populate boneweight object
		for(int i = 0; i < boneWeightByVertice.Count; i++)
		{
			switch(i)
			{
				case 0:
				{
					boneWeight.boneIndex0 = (int)boneWeightByVertice[0].x;
					boneWeight.weight0 = boneWeightByVertice[0].y;
					break;
				}
				case 1:
				{
					boneWeight.boneIndex1 = (int)boneWeightByVertice[1].x;
					boneWeight.weight1 = boneWeightByVertice[1].y;
					break;
				}
				case 2:
				{
					boneWeight.boneIndex2 = (int)boneWeightByVertice[2].x;
					boneWeight.weight2 = boneWeightByVertice[2].y;
					break;
				}
				case 3:
				{
					boneWeight.boneIndex3 = (int)boneWeightByVertice[3].x;
					boneWeight.weight3 = boneWeightByVertice[3].y;
					break;
				}
			}
		}
		
		return boneWeight;
	}
}
