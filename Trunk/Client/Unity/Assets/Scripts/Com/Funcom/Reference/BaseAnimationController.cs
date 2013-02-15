using UnityEngine;
using System.Collections.Generic;

public class BaseAnimationController:MonoBehaviour 
{
	/************************************************************************************************/
	/*	Const var																					*/
	/************************************************************************************************/
	private List<AnimationObject> AnimationList;
	private bool IsAnimationPlaying;
	
	/************************************************************************************************/
	/*	Member var																					*/
	/************************************************************************************************/
	
	/************************************************************************************************/
	/*	Constructor / Unity Reflective Method														*/
	/************************************************************************************************/
	protected virtual void Start() 
	{
		AnimationList = new List<AnimationObject>();
	}
	
	protected virtual void Update()
	{
		if(IsAnimationPlaying)
		{
			if(!animation.isPlaying && animation.wrapMode != WrapMode.Loop)
			{
				OnAnimationEnd();
			}
		}
	}
	
	/************************************************************************************************/
	/*	Public																						*/
	/************************************************************************************************/
	
	/************************************************************************************************/
	/*	Private	/ Protected																			*/
	/************************************************************************************************/
	protected void PlayAnimation(AnimationObject AnimationObj, bool ForceAnimation)
	{
		if(ForceAnimation)
		{
			OnAnimationEnd();
			AnimationList.Insert(0, AnimationObj);
		}
		else
		{
			AnimationList.Add(AnimationObj);
		}
		
		Debug.Log("AnimationController - Animation " + AnimationObj.AnimationName + " added to queue");
		
		CheckAnimationStatus();
	}
	
	protected void CheckAnimationStatus()
	{
		if(!IsAnimationPlaying)
		{
			if(AnimationList.Count > 0)
			{
				animation.wrapMode = AnimationList[0].AnimationWrapMode;
				animation.Play(AnimationList[0].AnimationName);
				IsAnimationPlaying = true;
				Debug.Log("AnimationController - Animation " + AnimationList[0].AnimationName + " Started");
			}
			else
			{
				Debug.Log("AnimationController - No Animation Pending");
			}
		}
	}
	
	protected void ClearAnimationList()
	{
		removeCurrentAnimation();
		AnimationList.Clear();
		Debug.Log("AnimationController - Animation list cleared");
	}
	
	private bool removeCurrentAnimation()
	{
		if(!IsAnimationPlaying || AnimationList.Count <= 0)
		{
			return false;
		}
		
		AnimationList.RemoveAt(0);
		IsAnimationPlaying = false;
		
		return true;
	}
	
	/************************************************************************************************/
	/*	Handler																						*/
	/************************************************************************************************/
	protected virtual void OnAnimationEnd()
	{
		if(removeCurrentAnimation())
		{
			Debug.Log("AnimationController - Animation " + AnimationList[0].AnimationName + " Completed");
			CheckAnimationStatus();
		}
	}
	
	/************************************************************************************************/
	/*	Accessor																					*/
	/************************************************************************************************/
	
}
