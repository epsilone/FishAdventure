using UnityEngine;
using System.Collections;
using System;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class CameraEngine:MonoBehaviour
    {
        //TESTING	
        public Interpolate.EaseType easeType; // set using Unity's property inspector
        private Interpolate.Function ease; // easing of a particular EaseType
        public float[] bTimes = new float[20];

        public float horizontalObliqueness = -0.05f;
        public float verticalObliqueness = -0.15f;
        public Rect rotateControlRelativeRect;
        public GUIStyle rotateControlStyle;
        public GUIStyle rotateUpControlStyle;
        public GUIStyle rotateDownControlStyle;
        public GUIStyle rotateLeftControlStyle;
        public GUIStyle rotateRightControlStyle;

        public GUIStyle rotateLeftButtonStyle;
        public Rect rotateLeftButtonRelativeRect;

        public GUIStyle rotateRightButtonStyle;
        public Rect rotateRightButtonRelativeRect;

        public GUIStyle rotateUpButtonStyle;
        public Rect rotateUpButtonRelativeRect;

        public GUIStyle rotateDownButtonStyle;
        public Rect rotateDownButtonRelativeRect;

        public GUIStyle zoomInButtonStyle;
        public Rect zoomInButtonRelativeRect;

        public GUIStyle zoomOutButtonStyle;
        public Rect zoomOutButtonRelativeRect;

        public GUIStyle fullScreenToggleButtonStyle;
        public GUIStyle windowToggleButtonStyle;
        public Rect fullScreenButtonRelativeRect;

        private Vector3 focusPoint = Vector3.zero;
        private Vector3 viewingPosition;
        private Vector3 startPosition;
        public float angleXZ = 60;
        public float angleXY = 35;
        private float distance = 0;
        private bool cameraLerp = false;
        public Camera mainCamera;
        private float duration;
        private float minDistance = 0.0f;
        private bool changed = false;

        private Vector2 mouseLeftButtonDownPosition;
        private Vector2 mouseRightButtonDownPosition;
        private bool leftMouseDown = false;
        private bool rightMouseDown = false;

        private float wTolerance = 0.005f; // tolerance for mouse down/up movement registering in screen fractions
        private float hTolerance = 0.005f; //
        private float degreesPerScreenWidth = 360.0f;  // screen territory in percentage per degree movement
        private float degreesPerScreenHeight = 180.0f;

        void Start()
        {
            ease = Interpolate.Ease(easeType);
            SetObliqueness();
        }

        private void SetObliqueness()
        {
            Matrix4x4 mat = mainCamera.projectionMatrix;
            mat[0, 2] = horizontalObliqueness;
            mat[1, 2] = verticalObliqueness;
            mainCamera.projectionMatrix = mat;
        }


        public float CurrentAngle
        {
            get
            {
                return angleXZ;
            }
        }

        public void UpdateCamera(Model model)
        {
            // only needed for positioning - SetObliqueness();
            if (!cameraLerp)
            {
                if (!model.IsAnimating())
                {
                    mainCamera.transform.position = ComputeCameraPosition(distance, focusPoint);
                    mainCamera.transform.LookAt(focusPoint);
                }
                else
                { // THIS NEVER GETS CALLED - CULL TIME...
                    float clampedTime = Mathf.Clamp(model.CurrentTime, 0, duration);

                    Bounds modelBounds = model.GetFullModelBounds();

                    Vector3 newPosition = ComputeCameraPosition(distance, focusPoint);
                    if (newPosition != viewingPosition || changed)
                    {
                        changed = false;
                        ComputeStartEndPositions(modelBounds);
                    }
                    try
                    {
                        if (duration == 0)
                        {
                            Debug.Log("duration = 0");

                        }
                        mainCamera.transform.position = Vector3.Slerp(startPosition, viewingPosition, clampedTime / duration);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                    mainCamera.transform.LookAt(focusPoint);
                }
            }
        }

        public Vector3 ComputeCameraPosition(float newDistance, Vector3 pFocusPoint)
        {
            //"A quaternion is just a special type of vector. Usually, you can get the angle between two unit vectors by using the dot product which would give you the cosine of the angle. If we look at the rotations that are represented by the quaternions, then we see that the unit quaternions form a double cover over the rotation space (so q = -q). Therefore, we need to do some extra work to get the correct angle between two rotations represented by quaternions, but the dot product still works. The difference in rotation (angle) between two quaternions can be given by:
            //  angle = 2 * acos(|q1.q2|)
            // If you don't want to use acos, you can just use the absolute dot product itself (which is close to one for small angles)."

            Vector3 cp = Vector3.forward;			// unit vector down the z
            Quaternion q = Quaternion.Euler(-angleXY, // rotation around the x axis 
                                            angleXZ, // rotation around the Y
                                            0.0f);   //rotation around the Z -  order is rotate z,x,y
            cp = q * cp;					// rotate the unit vector in the appropriate direction
            cp = cp * newDistance;
            cp = cp + pFocusPoint;

            //		Vector2 angleVector = new Vector2(Mathf.Cos(angleXY * Mathf.Deg2Rad)*newDistance, Mathf.Sin(angleXY * Mathf.Deg2Rad)*newDistance);
            //		Vector3 cp = new Vector3(Mathf.Sin(angleXZ*Mathf.Deg2Rad)*angleVector.x, angleVector.y, Mathf.Cos(angleXZ*Mathf.Deg2Rad)*angleVector.x) + pFocusPoint;	
            return cp;
        }

        public void SetFocusOnBounds(Bounds specBounds)
        {
            distance = ComputeBoundsDistance(specBounds);
            minDistance = distance * 0.80f;
            StopCoroutine("Focus");
            StartCoroutine("Focus", specBounds);
        }

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }

        public bool IsLerping()
        {
            return cameraLerp;
        }

        public void ComputeStartEndPositions(Bounds wholeModelBounds)
        {
            focusPoint = wholeModelBounds.center;
            distance = ComputeBoundsDistance(wholeModelBounds);
            minDistance = distance * 0.80f;
            startPosition = ComputeCameraPosition(distance, focusPoint);
            Bounds scaledBounds = new Bounds(wholeModelBounds.center, Vector3.Scale(wholeModelBounds.size, new Vector3(2.5f, 0.2f, 2.5f)));
            focusPoint = scaledBounds.center;
            viewingPosition = ComputeCameraPosition(ComputeBoundsDistance(scaledBounds), focusPoint);
        }

        private IEnumerator Focus(Bounds bounds)
        {
            focusPoint = bounds.center;
            viewingPosition = ComputeCameraPosition(ComputeBoundsDistance(bounds), focusPoint);
            startPosition = mainCamera.transform.position;
            cameraLerp = true;
            changed = true;

            while (!ApproximatePositions(mainCamera.transform.position, viewingPosition,0.01f))
            {
                mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, viewingPosition, Time.deltaTime * 3);
                mainCamera.transform.LookAt(focusPoint);
                yield return null;
            }
            cameraLerp = false;
        }

        private bool ApproximatePositions(Vector3 vectorOne, Vector3 vectorTwo, float percentageDifferenceAllowed)
        {
            return (vectorOne - vectorTwo).sqrMagnitude <= (vectorOne * percentageDifferenceAllowed).sqrMagnitude;
        }

        public float ComputeBoundsDistance(Bounds groupBounds)
        {
            float temp = Mathf.Max(groupBounds.size.x, groupBounds.size.y);
            float biggestSide = Mathf.Max(temp, groupBounds.size.z);
            return (biggestSide * 0.5f) / Mathf.Tan((mainCamera.fieldOfView * Mathf.Deg2Rad) * 0.5f);
        }

        void OnGUI()
        {
            bool buttonActioned = false;

            //back drop image for rotate controls

            if (!cameraLerp)
            {
                GUI.Label(ScreenRelative.rect(rotateControlRelativeRect), "rotate", rotateControlStyle);
                if (GUI.RepeatButton(ScreenRelative.rect(rotateUpButtonRelativeRect), "Up", rotateUpButtonStyle))
                {
                    GUI.Label(ScreenRelative.rect(rotateControlRelativeRect), "rotateUp", rotateUpControlStyle);
                    angleXY += 100 * Time.deltaTime;
                    buttonActioned = true;
                }
                if (GUI.RepeatButton(ScreenRelative.rect(rotateDownButtonRelativeRect), "Down", rotateDownButtonStyle))
                {
                    GUI.Label(ScreenRelative.rect(rotateControlRelativeRect), "rotateDown", rotateDownControlStyle);
                    angleXY -= 100 * Time.deltaTime;
                    buttonActioned = true;
                }

                if (GUI.RepeatButton(ScreenRelative.rect(rotateLeftButtonRelativeRect), "Left", rotateLeftButtonStyle))
                {
                    GUI.Label(ScreenRelative.rect(rotateControlRelativeRect), "rotateLeft", rotateLeftControlStyle);
                    angleXZ += 100 * Time.deltaTime;
                    buttonActioned = true;
                }

                if (GUI.RepeatButton(ScreenRelative.rect(rotateRightButtonRelativeRect), "Right", rotateRightButtonStyle))
                {
                    GUI.Label(ScreenRelative.rect(rotateControlRelativeRect), "rotateRight", rotateRightControlStyle);
                    angleXZ -= 100 * Time.deltaTime;
                    buttonActioned = true;
                }
                if (GUI.RepeatButton(ScreenRelative.rect(zoomInButtonRelativeRect), "In", zoomInButtonStyle))
                {
                    distance = Mathf.Clamp(distance - 30 * Time.deltaTime, minDistance, minDistance * 3);
                    buttonActioned = true;
                }

                if (GUI.RepeatButton(ScreenRelative.rect(zoomOutButtonRelativeRect), "Out", zoomOutButtonStyle))
                {
                    distance = Mathf.Clamp(distance + 30 * Time.deltaTime, minDistance, minDistance * 3);
                    buttonActioned = true;
                }

                if (GUI.Button(ScreenRelative.rect(fullScreenButtonRelativeRect), "fullscreen", (Screen.fullScreen) ? windowToggleButtonStyle : fullScreenToggleButtonStyle))
                {
                    ease = Interpolate.Ease(easeType);
                    for (int i = 0; i < bTimes.Length; i++)
                    {
                        bTimes[i] = ease(0, bTimes.Length, i, bTimes.Length);
                    }
                    Screen.fullScreen = !Screen.fullScreen;
                    buttonActioned = true;
                }

                if (!buttonActioned)
                {
                    Event e = Event.current;
                    if (e.isMouse)
                    {	// could maybe of used e.delta - but gave problems with button down - maybe used wrong approach
                        if (e.type == EventType.MouseDown)
                        {
                            switch (e.button)
                            {
                                case 0: leftMouseDown = true; mouseLeftButtonDownPosition = e.mousePosition; break;
                                case 1: rightMouseDown = true; mouseRightButtonDownPosition = e.mousePosition; break;
                            }
                        }
                        else if (e.type == EventType.MouseUp)
                        {
                            switch (e.button)
                            {
                                case 0: leftMouseDown = false; break;
                                case 1: rightMouseDown = false; break;
                            }
                        }
                        else if (e.type == EventType.ScrollWheel)
                        {
                        }

                        if (leftMouseDown)
                        {
                            Vector2 delta = e.mousePosition - mouseLeftButtonDownPosition;
                            if (Mathf.Abs(delta.x) > Screen.width * wTolerance)
                            {
                                angleXZ += (delta.x / Screen.width) * degreesPerScreenWidth;
                            }
                            if (Mathf.Abs(delta.y) > Screen.height * hTolerance)
                            {
                                angleXY += (delta.y / Screen.height) * degreesPerScreenHeight;
                            }
                            mouseLeftButtonDownPosition = e.mousePosition;
                        }
                        else if (rightMouseDown)
                        {
                            Vector2 delta = e.mousePosition - mouseRightButtonDownPosition;
                            if (Mathf.Abs(delta.x) > Screen.width * wTolerance)
                            {
                                angleXZ += (delta.x / Screen.width) * degreesPerScreenWidth * 4;
                            }
                            if (Mathf.Abs(delta.y) > Screen.height * hTolerance)
                            {
                                angleXY += (delta.y / Screen.height) * degreesPerScreenHeight * 4;
                            }
                            mouseRightButtonDownPosition = e.mousePosition;
                        }
                    }
                }
                angleXZ %= 360;
                angleXY %= 360;
                angleXY = Mathf.Clamp(angleXY, -89.9f, 89.9f);
                //if(angleXY > 89.9f)  { angleXY =  89.9f; }
                //if(angleXY < -89.9f) { angleXY = -89.9f; }
            }
        }

        void LateUpdate()
        {
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 30, minDistance, minDistance * 3);
        }
    }
}