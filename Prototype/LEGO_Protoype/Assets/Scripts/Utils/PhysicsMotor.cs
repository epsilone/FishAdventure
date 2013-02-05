using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsMotor
{
	private Rigidbody body;
	private float forceDecayPerSec;
	private float rotationTimer = 0.0f;
	private float rotationDuration = 0.3f;
	private float currentRotationDuration;
	private Vector3 currentForce;
	private GameObject entityGo;
	private bool rotate = false;
	private Quaternion startRotation;
	private Quaternion endRotation;
	
	public PhysicsMotor(GameObject entityGo, float forceDecay, float downwardsDuration, float maxDownwardsForce, float rotationDuration)
	{		
		this.entityGo = entityGo;
		this.forceDecayPerSec = forceDecay;
		this.rotationDuration = rotationDuration;
		body = entityGo.AddComponent<Rigidbody>() as Rigidbody;	
		currentRotationDuration = rotationDuration;		
	}
	
	public void SetUpRigidbody(float mass, float drag)
	{
		body.mass = mass;
		body.drag = drag;
		body.freezeRotation = true;
	}
	
	public void AddForce(Vector3 force)
	{
		currentForce=force;
	}
	
	public float GetCurrentForce()
	{
		return currentForce.magnitude;
	}
	
	public float GetRotationDuration()
	{
		return rotationDuration;
	}
	
	public bool IsRotating()
	{
		return rotate;		
	}
	
	public void PhysicsUpdate()
	{
		if(currentForce!=Vector3.zero)
		{
			body.AddForce(currentForce, ForceMode.Force);
		}
	}
	
	public void StopMovement()
	{
		currentForce = Vector3.zero;
		Vector3 velocity = body.velocity;
		velocity.x = 0;
		body.velocity = velocity;
	}
	
	public void EnableFall(bool state)
	{
		if(state)
		{
			
			body.constraints = RigidbodyConstraints.FreezePositionY;
		}
		else
		{
			body.freezeRotation = true;
			body.constraints = RigidbodyConstraints.None;
		}
	}
	
	public void ChangeDirection(Vector3 newDirection)
	{
		Quaternion tempRotation = Quaternion.LookRotation(newDirection);
		if(entityGo.transform.rotation!=tempRotation)
		{
			if(!rotate)
			{
				rotate = true;
			}
			startRotation = entityGo.transform.rotation;
			rotationTimer = 0.0f;
			endRotation = tempRotation;
			currentRotationDuration = rotationDuration*(Quaternion.Angle(startRotation, endRotation)/180);
		}
	}
	
	public void UpdateVelocities()
	{		
		
		if(rotate)
		{
			if(rotationTimer/rotationDuration<=1)
			{
				rotationTimer+=Time.deltaTime;
				entityGo.transform.rotation = Quaternion.Slerp(startRotation, endRotation, rotationTimer/currentRotationDuration);
			}
			else
			{
				rotate = false;				
			}
		}
		
		
		float minX = Mathf.Sign(currentForce.x)==-1 ? -500 : 0;
		float maxX = Mathf.Sign(currentForce.x)==-1 ? 0 : currentForce.x;
		float minY = Mathf.Sign(currentForce.y)==-1 ? -500 : 0;
		float maxY = Mathf.Sign(currentForce.y)==-1 ? 0 : currentForce.y;
		float minZ = Mathf.Sign(currentForce.z)==-1 ? -500 : 0;
		float maxZ = Mathf.Sign(currentForce.z)==-1 ? 0 : currentForce.z;
		float decay = forceDecayPerSec*Time.deltaTime;
		float nextX = Mathf.Sign(currentForce.x)==-1 ? currentForce.x+decay : currentForce.x-decay; 
		float nextY = Mathf.Sign(currentForce.y)==-1 ? currentForce.y+decay : currentForce.y-decay;
		float nextZ = Mathf.Sign(currentForce.z)==-1 ? currentForce.z+decay : currentForce.z-decay;
		float x = Mathf.Clamp(nextX, minX, maxX);
		float y = Mathf.Clamp(nextY, minY, maxY);
		float z = Mathf.Clamp(nextZ, minZ, maxZ);
		
		currentForce = new Vector3(x, y, z);
	}
}