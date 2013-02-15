using UnityEngine;
using System.Collections;

namespace com.funcom.component.camera
{
	public class CameraControllerXY:MonoBehaviour 
	{
		//Configuration
		public float _sensitivity = 0.2f;
		
		//Managment
		private Vector3 _clickPosition;
		private Vector3 _hitPosition;
		private Vector3 _currentPosition;
		
		
		void Start () 
		{
			_clickPosition = Vector3.zero;
			_hitPosition = Vector3.zero;
			_currentPosition = Vector3.zero;
		}
		
		void FixedUpdate()
		{
		    if(Input.GetMouseButtonDown(0))
			{
				StartDrag();
		    }
			
		    if(Input.GetMouseButton(0))
			{
		        _currentPosition = Input.mousePosition;
		        OnDrag();        
		    }
		}
		
		private void StartDrag()
		{
			_clickPosition = Input.mousePosition;
		    _hitPosition = _clickPosition;
			_currentPosition = _hitPosition;
		}
		
		private void OnDrag()
		{
		    Vector3 positionDiff = _hitPosition - _currentPosition;
			positionDiff.z = 0;
			
			positionDiff *= _sensitivity;
			transform.Translate(positionDiff);
			_hitPosition = _currentPosition;
		}
	}
}