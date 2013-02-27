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

namespace MeshSkinner
{ 
	public class MeshSkinner:MonoBehaviour 
	{
		#region Member Variables
		public Mesh baseMesh;
		public Transform skeleton;
		#endregion

		void Start () 
		{
            //gameObject.AddComponent();
			//Add empty Component?
			
            SkinnedMeshRenderer renderer = gameObject.AddComponent<SkinnedMeshRenderer>();
			
            /*MESH*/
            Mesh sourceMesh = Instantiate(baseMesh) as Mesh;
            sourceMesh.RecalculateNormals();
            BoneWeight[] weights = new BoneWeight[sourceMesh.vertexCount];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i].boneIndex0 = 0;
                weights[i].weight0 = 1;
            }
            sourceMesh.boneWeights = weights;
			
            /*SKELETON*/
            Transform[] bones;
            Transform sourceSkeleton = Instantiate(skeleton) as Transform;
            sourceSkeleton.name = "skeleton";
            sourceSkeleton.parent = gameObject.transform;
            bones = sourceSkeleton.GetComponentsInChildren<Transform>();
			
            /*BIND POSES*/
            Matrix4x4[] bindPoses = new Matrix4x4[bones.Length];
            for (int i = 0; i < bindPoses.Length; i++)
            {
                bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
            }
			
            /*MATERIAL*/
			//What is that?
            Material[] materials = new Material[3];
            materials[0] = materials[1] = materials[2] = new Material(Shader.Find(" Diffuse"));
            renderer.materials = materials;
			
            /*END*/
            sourceMesh.bindposes = bindPoses;
            renderer.bones = bones;
            renderer.sharedMesh = sourceMesh;
		}
	}
}
