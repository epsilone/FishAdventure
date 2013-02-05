using UnityEngine;
using System.Collections;

public class Swipe : MonoBehaviour {
	float startTime;
	Vector2 startPosition;
	bool couldBeSwipe;
	
	public float comfortZone = 150f;
	public float minSwipeDistance = 1;
	public float maxSwipeTime = 5;

	public float deltaDivider = 30.0f;
	public float scrollSpeed = 1.0f;
	
	public bool swap = true;
	
	public GameObject planeParent;
	public GameObject cameraBounds;
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.touchCount > 0){
			Touch touch = Input.GetTouch(0);

			switch(touch.phase){
				case(TouchPhase.Began):
					couldBeSwipe = true;
					startPosition = touch.position;
					startTime = Time.time;
					
					//Debug.Log("Phase : Began; startPosition: " + startPosition + "; startTime = " + startTime);
				break;

				case(TouchPhase.Moved):
					float swipeY = Mathf.Abs(touch.position.y - startPosition.y); 
				    float swipeX = Mathf.Abs(touch.position.x - startPosition.x);
				    
					if((swipeY > comfortZone) || (swipeX > comfortZone)){
					   //Debug.Log("Out of comfortZone = " + comfortZone);
					  // Debug.Log("CouldBeSwipe = FALSE");
						couldBeSwipe = false;						
					}else{
						Vector2 deltaSwipe;
						if(swap){
							deltaSwipe = new Vector2(startPosition.x - touch.position.x, startPosition.y - touch.position.y);
						}else{
							deltaSwipe = new Vector2(touch.position.x - startPosition.x, touch.position.y - startPosition.y);
						}
						
						Vector3 position = Camera.main.transform.position;
						//Vector3 newPosition = new Vector3(position.x + deltaSwipe.x/5, position.y + deltaSwipe.y/5, position.z);
						Vector3 newPosition = new Vector3(position.x + deltaSwipe.x  / deltaDivider, position.y + deltaSwipe.y / deltaDivider, position.z);
						//Bounds scaledBounds = new Bounds(cameraBounds.center, new Vector3(cameraBounds.size.x, cameraBounds.size.y, 100));
						//(newPosition.x > -90 && newPosition.x < 90 && 
						//newPosition.y < 105 && newPosition.y > -110){
					
						float x = Mathf.Clamp(newPosition.x, cameraBounds.collider.bounds.min.x, cameraBounds.collider.bounds.max.x);
						float y = Mathf.Clamp(newPosition.y, cameraBounds.collider.bounds.min.y, cameraBounds.collider.bounds.max.y);
						
						//iTween.MoveTo(Camera.main.gameObject, new Vector3(x, y, newPosition.z), scrollSpeed);
						//Camera.main.transform.position = Vector3.Lerp(position, newPosition, Mathf.SmoothStep(0.0, 1.0, Mathf.SmoothStep(0.0, 1.0, t)));
						Camera.main.transform.position = new Vector3(x, y, newPosition.z);
					//	}
					}
					
					
					//Debug.Log("swipeY = " + swipeY + "; swipeX = " + swipeX);
					break;

				case(TouchPhase.Stationary):
					couldBeSwipe = false;
					//Debug.Log("Phase = stationary; CouldBeSwipe = " + couldBeSwipe);
				break;
				
				case(TouchPhase.Ended):
					float swipeTime = Time.time - startTime;
					float swipeDistance = (touch.position - startPosition).magnitude;
					
					//Debug.Log("Phase = ended; CouldBeSwipe = " +couldBeSwipe+ "; swipeTime =" + swipeTime + "; swipeDistance = " + swipeDistance );
					//Debug.Log("couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDistance > minSwipeDistance) = " + couldBeSwipe + "; " + (swipeTime < maxSwipeTime) + ";" + (swipeDistance > minSwipeDistance));
				
					if(couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDistance > minSwipeDistance)){
						float swipeDirection = Mathf.Sign(touch.position.y - startPosition.y);
					//	Debug.Log("Swipe to the left! swipeDirection = " + swipeDirection);
					}
					
					//iTween.Stop();
				break;
			}
			
			startPosition = touch.position;
		}
	}
	
	public static Bounds combineBounds(params Renderer[] renderers){
		Bounds result = renderers[0].bounds;
		if(renderers.Length > 1){
			for(int i = 1; i < renderers.Length; ++i){
				result.Encapsulate(renderers[i].bounds);
			}
		}
		return result;
	} 
	
	public static float DistanceToBounds(Bounds bounds)
	{                                                       
			return Mathf.Max(bounds.size.x, bounds.size.y) / Mathf.Sin(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f);
	}
}
