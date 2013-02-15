using UnityEngine;
using System.Collections.Generic;

public class HelicoController:MonoBehaviour 
{
	/************************************************************************************************/
	/*	Const var																					*/
	/************************************************************************************************/
	public float _maximumEngineForce = 0.0f;
	public float _currentEngineForce = 0.0f;
	public Rigidbody _missileObject;
	public float _missileforce;
	
	private Rigidbody _objRigidbody;
	private Vector3 _forceVector;
	
	/************************************************************************************************/
	/*	Member var																					*/
	/************************************************************************************************/
	
	/************************************************************************************************/
	/*	Constructor / Unity Reflective Method														*/
	/************************************************************************************************/
	protected virtual void Start() 
	{
		_forceVector = new Vector3(0,1,0);
		_objRigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
	}
	
	protected virtual void Update()
	{
		float axisValue;
		Vector3 fwd;
		
		if(Input.GetButton("Motor"))
		{
			axisValue = Input.GetAxis("Motor");
			if(axisValue > 0)
			{
				_currentEngineForce = _currentEngineForce + 0.1f;
			}
			else
			{
				_currentEngineForce = _currentEngineForce - 0.05f;
			}
			
			if(_currentEngineForce > _maximumEngineForce)
			{
				_currentEngineForce = _maximumEngineForce;
			}
			else if(_currentEngineForce < 0)
			{
				_currentEngineForce = 0;
			}
		}
		
		if(Input.GetButton("Vertical"))
		{
			axisValue = Input.GetAxis("Vertical");
			if(axisValue > 0)
			{
				transform.Rotate(new Vector3(0,0,0.1f));
			}
			else
			{
				transform.Rotate(new Vector3(0,0,-0.1f));
			}
		}
		
		if(Input.GetButton("Horizontal"))
		{
			axisValue = Input.GetAxis("Horizontal");
			if(axisValue > 0)
			{
				transform.Rotate(new Vector3(-0.1f,0,0));
			}
			else
			{
				transform.Rotate(new Vector3(0.1f,0,0));
			}
		}
		
		if(Input.GetButton("Fire"))
		{
			Vector3 position = transform.position;
			position.x += 14;
			Rigidbody instance = Instantiate(_missileObject, position, transform.rotation) as Rigidbody;
			fwd = transform.TransformDirection(new Vector3(-1,0,0));
			instance.AddForce(fwd * _missileforce);
		}
		
		
		
		fwd = transform.TransformDirection(_forceVector);
		_objRigidbody.AddForce(fwd * _currentEngineForce);
		
	}
	
	/************************************************************************************************/
	/*	Public																						*/
	/************************************************************************************************/
	
	/************************************************************************************************/
	/*	Private	/ Protected																			*/
	/************************************************************************************************/
	
	
	/************************************************************************************************/
	/*	Handler																						*/
	/************************************************************************************************/
	
	/************************************************************************************************/
	/*	Accessor																					*/
	/************************************************************************************************/
	
}
