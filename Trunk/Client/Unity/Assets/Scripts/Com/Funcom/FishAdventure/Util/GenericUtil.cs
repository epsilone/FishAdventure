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

	public static void CombineChildMeshes(GameObject obj)
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
	
	public static void CreateBones(GameObject obj)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
        if(!skinnedMeshRenderer)
		{
        	Debug.Log("CreateBones() - There is no mesh to apply bones on!");
        	return;
		}
		
		//BONES CREATION
	    Transform[] bones = new Transform[4];
	    Matrix4x4[] bindPoses = new Matrix4x4[4];
	
	    bones[0] = new GameObject ("Bone1").transform;
	    bones[0].parent = obj.transform;
	    bones[0].localRotation = Quaternion.identity;
	    bones[0].localPosition = new Vector3 (0, 0, 2.2f);
	    bindPoses[0] = bones[0].worldToLocalMatrix * obj.transform.localToWorldMatrix;
		
		bones[1] = new GameObject ("Bone2").transform;
	    bones[1].parent = bones[0].transform;
	    bones[1].localRotation = Quaternion.identity;
	    bones[1].localPosition = new Vector3 (-3, 0, 0);
	    bindPoses[1] = bones[1].worldToLocalMatrix * bones[0].transform.localToWorldMatrix;
		
		bones[2] = new GameObject ("Bone3").transform;
	    bones[2].parent = bones[1].transform;
	    bones[2].localRotation = Quaternion.identity;
	    bones[2].localPosition = new Vector3 (-3, 0, 0);
	    bindPoses[2] = bones[2].worldToLocalMatrix * bones[1].transform.localToWorldMatrix;
		
		bones[3] = new GameObject ("Bone4").transform;
	    bones[3].parent = bones[2].transform;
	    bones[3].localRotation = Quaternion.identity;
	    bones[3].localPosition = new Vector3 (-3, 0, 0);
	    bindPoses[3] = bones[3].worldToLocalMatrix * bones[2].transform.localToWorldMatrix;
	
	    skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
	    skinnedMeshRenderer.bones = bones;
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
					Debug.Log("VertexId=" + verticeIndex +  " / BoneId=" + vec.x + " / Weight=" + vec.y);
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
		
		Vector3 size = skinnedMeshRenderer.bounds.size;
		Debug.Log(size);
		float Xpos = -(size.x * 0.5f);
		float Ypos = (size.y * 0.5f);
		float Zpos = (size.z * 0.5f);
		Vector3 verticeBuffer;
		
		for(int verticeIndex = 0; verticeIndex < skinnedMeshRenderer.sharedMesh.vertices.Length; verticeIndex++)
		{
			verticeBuffer = skinnedMeshRenderer.sharedMesh.vertices[verticeIndex];
			verticeBuffer.x = 1;
			verticeBuffer.y = 1;
			verticeBuffer.z = 1;
			
		}
	}
	
	private static BoneWeight GetBoneWeightByVerticeInfo(List<Vector2> boneWeightByVertice, float minimalBoneWeight)
	{
		BoneWeight boneWeight = new BoneWeight();
		float weightDiff = 0.0f;
		
		//Remove weight under the minimum limit
		boneWeightByVertice.RemoveAll(x => x.y < minimalBoneWeight);
		
		//Recalibrate weight
		for(int i = 0; i < boneWeightByVertice.Count; i++)
		{
			float weight = (boneWeightByVertice[i].y / (1.0f - weightDiff));
			boneWeightByVertice[i] = new Vector2(boneWeightByVertice[i].x, weight);
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
