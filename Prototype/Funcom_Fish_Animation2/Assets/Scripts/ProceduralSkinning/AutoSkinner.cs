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
using System.Collections.Generic;
#endregion

namespace AutoSkinner
{ 
	public class AutoSkinner:MonoBehaviour 
	{
		#region Member Variables
		public AnimationClip animationBuffer;
		#endregion

		void Start () 
		{
			SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>() as SkinnedMeshRenderer;
			Transform[] boneList = renderer.bones;
			Vector3[] verticeList = renderer.sharedMesh.vertices;
			BoneWeight[] boneWeightList = new BoneWeight[verticeList.Length];
			
			Transform boneBuffer;
			Vector3 verticeBuffer;
			float distanceBuffer;
			float weightBuffer;
			List<Vector2> boneWeightByVertice = new List<Vector2>();
			float distanceCumulative;
			
			for(int verticeIndex = 0; verticeIndex < verticeList.Length; verticeIndex++)
			{
				verticeBuffer = verticeList[verticeIndex];
				
				boneWeightByVertice.Clear();
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
				boneWeightList[verticeIndex].boneIndex0 = (int)boneWeightByVertice[0].x;
				boneWeightList[verticeIndex].weight0 = boneWeightByVertice[0].y;
				
				boneWeightList[verticeIndex].boneIndex1 = (int)boneWeightByVertice[1].x;
				boneWeightList[verticeIndex].weight1 = boneWeightByVertice[1].y;
				
				boneWeightList[verticeIndex].boneIndex2 = (int)boneWeightByVertice[2].x;
				boneWeightList[verticeIndex].weight2 = boneWeightByVertice[2].y;
				
				boneWeightList[verticeIndex].boneIndex3 = (int)boneWeightByVertice[3].x;
				boneWeightList[verticeIndex].weight3 = boneWeightByVertice[3].y;
				
				
				
				/*if((verticeIndex % 20) == 0)
				{
					foreach(Vector2 vec in boneWeightByVertice)
					{
						Debug.Log("VertexId=" + verticeIndex +  " / BoneId=" + vec.x + " / Weight=" + vec.y);
					}
				}*/
			}
			
			renderer.sharedMesh.boneWeights = boneWeightList;
			
			
			
			
			/////////////////
			animation.AddClip(animationBuffer, "idle");
			animation.wrapMode = WrapMode.Loop;
		    animation.Play("idle");
		}
		
		//Should be move into an Util Class
		private GameObject GetChildByName(string name)
		{
		   Component[] transforms = GetComponentsInChildren(typeof(Transform), true );
			
		   foreach(Transform transform in transforms)
		   {
		      if( transform.gameObject.name == name )
		      {
		         return transform.gameObject;
		      }
		   }
			
		   return null;
		}
	}
}
