using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneUtility
{
	public const float walkStepLength = 0.8f;
	public const float brickHeight = 0.96f;
	public static Vector3[] bounds = new Vector3[4];

	public static bool ApproximateVectors(Vector3 vectorOne, Vector3 vectorTwo, float percentageDifferenceAllowed = 0.01f)
	{
		return (vectorOne - vectorTwo).sqrMagnitude <= (vectorOne * percentageDifferenceAllowed).sqrMagnitude;	
	}
	
	public static bool CanMove(Vector3 direction, Bounds bounds, float checkDistance)
	{
		int passes = Mathf.FloorToInt(bounds.size.y/0.4f);
		for(int i=1; i<=passes; ++i)
		{
			Vector3 nextPos = new Vector3(bounds.center.x, bounds.min.y+i*0.4f, bounds.center.z);
			if(Physics.Raycast(nextPos, direction, checkDistance))
			{
				return false;
			}
		}
		return true;
	}
	
	public static bool TestStickerCollision (Bounds stickerBounds, out float distance, out GameObject hitObject, float offset = 0.2f)
	{
		float maxZ = stickerBounds.max.z;
		bounds [0] = new Vector3 (stickerBounds.min.x+offset, stickerBounds.max.y-offset, maxZ);
		bounds [1] = new Vector3 (stickerBounds.max.x-offset, stickerBounds.max.y-offset, maxZ);
		bounds [2] = new Vector3 (stickerBounds.min.x+offset, stickerBounds.min.y+offset, maxZ);
		bounds [3] = new Vector3 (stickerBounds.max.x-offset, stickerBounds.min.y+offset, maxZ);

		float tmpDistance = 100.0f;
		GameObject tmpHitObject = null;
		foreach (Vector3 v in bounds) {
			RaycastHit hit;
			Ray ray = new Ray (v, Vector3.forward);
			if (!Physics.Raycast (ray, out hit) ) {
				Debug.Log ("TestStickerCollision : Did not hit Something with ray");
				distance = 0f;
				hitObject = null;
				return 	false;
			}else{
				tmpHitObject = hit.transform.gameObject;
				tmpDistance = Mathf.Min(tmpDistance, hit.distance);
			}
		}
		
		hitObject = tmpHitObject;
		distance = tmpDistance;
		return true;
	}

	public static void Update(){
		foreach (Vector3 v in bounds) {
			Debug.DrawLine(v, v+Vector3.forward);
		}
	}
}