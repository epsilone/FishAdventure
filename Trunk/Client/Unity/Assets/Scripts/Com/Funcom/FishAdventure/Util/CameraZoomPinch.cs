using UnityEngine;
using System.Collections;

public class CameraZoomPinch : MonoBehaviour {
    public int speed = 4;
    public float MINSCALE = -50.0F;
    public float MAXSCALE = -10.0F;
    public float minPinchSpeed = 5.0F;
    public float varianceInDistances = 5.0F;
    private float touchDelta = 0.0F;
    private Vector2 prevDist = new Vector2(0, 0);
    private Vector2 curDist = new Vector2(0, 0);
    private float speedTouch0 = 0.0F;
    private float speedTouch1 = 0.0F;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        

         if (Input.GetAxis("Mouse ScrollWheel") != 0.0F) {
             Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
             Vector3 cameraPosition = gameObject.transform.position;

             gameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y,
                 Mathf.Clamp(cameraPosition.z + (Input.GetAxis("Mouse ScrollWheel") * speed), MINSCALE, MAXSCALE));
             Debug.Log(gameObject.transform.position.z);
         }



        else if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) {

            curDist = Input.GetTouch(0).position - Input.GetTouch(1).position; //current distance between finger touches
            prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition)); //difference in previous locations using delta positions
            touchDelta = curDist.magnitude - prevDist.magnitude;
            speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
            speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;


            Vector3 cameraPosition = gameObject.transform.position;
            if ((touchDelta + varianceInDistances <= 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed)) {
                gameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, Mathf.Clamp(cameraPosition.z - (1 * 0.5f*speed), MINSCALE, MAXSCALE));
            }

            if ((touchDelta + varianceInDistances > 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed)) {
                gameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, Mathf.Clamp(cameraPosition.z + (1 * 0.5f * speed), MINSCALE, MAXSCALE));
            }

        }
    }

}