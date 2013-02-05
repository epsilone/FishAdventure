using UnityEngine;
using System.Collections;

public class CameraManager  : MonoBehaviour{
	public Camera camera;
	private Vector3 startPosition;

	private float timer;
	private float duration;

	private Vector3 target;
	private GameObject targetEntity;

	public enum CAMERA_MODE{
		STATIC,
		FOLLOW,
		SCROLL,
		PAN // rotation only
	}

	public CAMERA_MODE cameraMode;

	public CameraManager(Camera camera){
		if(camera){
			this.camera = camera;
			startPosition = camera.transform.position;
		}
		this.cameraMode = CAMERA_MODE.STATIC;
	}

	public void StartScroll(Vector3 target, float speed){
		this.target = target;
		timer = 0.00f;
		cameraMode = CAMERA_MODE.SCROLL;
		startPosition = camera.transform.position;
		duration = Vector3.Distance(startPosition, target) / speed;
	}

	public void FollowEntity(GameObject entity){
		targetEntity = entity;
		cameraMode = CAMERA_MODE.FOLLOW;
	}

	public void LateUpdate(){
		//Debug.Log("Camera update");
		switch(cameraMode) {
			case CAMERA_MODE.STATIC:
				break;

			case CAMERA_MODE.FOLLOW:
				camera.transform.position = new Vector3(targetEntity.transform.position.x, targetEntity.transform.position.y, camera.transform.position.z);
				//camera.transform.rotation = Quaternion.LookRotation(targetEntity.EntityGameObject.transform.position - camera.transform.position);
				break;

			case CAMERA_MODE.SCROLL:
				//Debug.Log("Duration = " + duration);
				camera.transform.position = Vector3.Lerp(startPosition, target, Mathf.Clamp(timer/duration, 0f, 1f));
				timer+=Time.deltaTime;

				//Debug.Log("Camera Mode Scroll. " + "startposition = " + startPosition + "; cameraposition = " + camera.transform.position + "; target = " + target + "; timer = " + timer);

				if(camera.transform.position == target){
					cameraMode = CAMERA_MODE.STATIC;
					Debug.Log("Set to static");
				}

				break;
			}
	}
	
	public Camera getCamera(){
		return camera;
	}
}
