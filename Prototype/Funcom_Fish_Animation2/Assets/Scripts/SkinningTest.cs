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

namespace SkinningTest
{ 
	public class SkinningTest:MonoBehaviour 
	{
		#region Member Variables
		public Mesh basemesh;
		public AnimationClip animationBuffer;
		#endregion

		void Start () 
		{
			gameObject.AddComponent<Animation>();
		    gameObject.AddComponent<SkinnedMeshRenderer>();
		    SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>() as SkinnedMeshRenderer;
		
		    //CREATE MESH
		    Mesh mesh = new Mesh ();
		    /*mesh.vertices = new Vector3[] {new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, 5, 0),new Vector3(1, 5, 0)};
		    mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)};
		    mesh.triangles = new int[] {0, 1, 2, 1, 3, 2};*/
			
			mesh.vertices = basemesh.vertices;
			mesh.uv = basemesh.uv;
			mesh.triangles = basemesh.triangles;
			
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
			
			//SKINNING (BoneWeight BY vertex... mesh.boneWeights.Length == mesh.vertices.Length)
		   	BoneWeight[] weights = new BoneWeight[mesh.vertices.Length];
			for(int i=0;i<mesh.vertices.Length;i++)
			{
				if(i < (mesh.vertices.Length * 0.5))
				{
					weights[i].boneIndex0 = 0;
		    		weights[i].weight0 = 1;
				}
				else
				{
					weights[i].boneIndex0 = 3;
		    		weights[i].weight0 = 1;
				}
			}
		    mesh.boneWeights = weights;
			
			//ANIMATION
			animation.AddClip(animationBuffer, "idle");
			animation.wrapMode = WrapMode.Loop;
		    animation.Play("idle");
		}
		
		void Update () 
		{
		
		}
	}
}
