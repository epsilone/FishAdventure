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

namespace CreateBones
{ 
	public class CreateFishBones:MonoBehaviour 
	{
		#region Member Variables
		public Mesh baseMesh;
		public AnimationClip animationBuffer;
		public float minimalBoneWeight = 0.0f;
		#endregion

		void Start () 
		{
			//ADD BASE COMPONENT
			gameObject.AddComponent<Animation>();
		    gameObject.AddComponent<SkinnedMeshRenderer>();
		    SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>() as SkinnedMeshRenderer;
			
			//CREATE MESH
			Mesh mesh = new Mesh ();
			mesh.vertices = baseMesh.vertices;
			mesh.uv = baseMesh.uv;
			mesh.triangles = baseMesh.triangles;
			mesh.RecalculateNormals();
			
			//ADD MATERIAL
		    renderer.material = new Material (Shader.Find("Diffuse"));
			
			//BONES CREATION
		    Transform[] bones = new Transform[4];
		    Matrix4x4[] bindPoses = new Matrix4x4[4];
		
		    bones[0] = new GameObject ("Bone1").transform;
		    bones[0].parent = transform;
		    bones[0].localRotation = Quaternion.identity;
		    bones[0].localPosition = new Vector3 (0, 0, -2.2f);
		    bindPoses[0] = bones[0].worldToLocalMatrix * transform.localToWorldMatrix;
			
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
		
		    mesh.bindposes = bindPoses;
		    renderer.bones = bones;
		    renderer.sharedMesh = mesh;
			
			//Add base skinning info
			/*int verticeLength = mesh.vertices.Length;
			BoneWeight[] weights = new BoneWeight[verticeLength];
			for(int i = 0; i<verticeLength; i++)
			{
				weights[i].boneIndex0 = 0;
				weights[i].weight0 = 1;
			}
		    mesh.boneWeights = weights;*/
			
			//////////
			SkinIt();
			
			animation.AddClip(animationBuffer, "idle");
			animation.wrapMode = WrapMode.Loop;
		    //animation.Play("idle");
			//////////
		}
		
		private void SkinIt()
		{
			SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>() as SkinnedMeshRenderer;
			Transform[] boneList = renderer.bones;
			Vector3[] verticeList = renderer.sharedMesh.vertices;
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
				
				/////////
				/*boneWeightList[verticeIndex].boneIndex0 = (int)boneWeightByVertice[0].x;
				boneWeightList[verticeIndex].weight0 = boneWeightByVertice[0].y;
				
				boneWeightList[verticeIndex].boneIndex1 = (int)boneWeightByVertice[1].x;
				boneWeightList[verticeIndex].weight1 = boneWeightByVertice[1].y;
				
				boneWeightList[verticeIndex].boneIndex2 = (int)boneWeightByVertice[2].x;
				boneWeightList[verticeIndex].weight2 = boneWeightByVertice[2].y;
				
				boneWeightList[verticeIndex].boneIndex3 = (int)boneWeightByVertice[3].x;
				boneWeightList[verticeIndex].weight3 = boneWeightByVertice[3].y;*/
				
				boneWeightList[verticeIndex] = GetBoneWeightByVerticeInfo(boneWeightByVertice);
				
				if((verticeIndex % 20) == 0)
				{
					foreach(Vector2 vec in boneWeightByVertice)
					{
						Debug.Log("VertexId=" + verticeIndex +  " / BoneId=" + vec.x + " / Weight=" + vec.y);
					}
				}
			}
			
			renderer.sharedMesh.boneWeights = boneWeightList;
		}
		
		private BoneWeight GetBoneWeightByVerticeInfo(List<Vector2> boneWeightByVertice)
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
		
		
		void LateUpdate () 
		{
			//renderer.bones[1].localPosition = renderer.bones[1].localPosition +  new Vector3(100,0,0);
		}
	}
}
