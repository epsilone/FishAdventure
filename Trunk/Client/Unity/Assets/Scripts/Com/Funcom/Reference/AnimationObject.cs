using UnityEngine;
using System.Collections;

public class AnimationObject 
{
	private string _animationName;
	private WrapMode _wrapMode;
	
	public AnimationObject(string aName, WrapMode aWrapMode)
    {
        _animationName = aName;
		_wrapMode = aWrapMode;
    }
	
	public string AnimationName
	{
	    get {return _animationName;}
	    set {_animationName = value;}
	}
	
	public WrapMode AnimationWrapMode
	{
	    get {return _wrapMode;}
	    set {_wrapMode = value;}
	}
}
