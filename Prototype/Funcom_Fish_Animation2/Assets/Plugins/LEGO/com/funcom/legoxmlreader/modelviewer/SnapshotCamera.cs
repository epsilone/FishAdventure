using System.Collections.Generic;
using UnityEngine;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class SnapshotCamera : MonoBehaviour
    {
        public Camera snapCamera;

        void Awake()
        {
        }
        void Start()
        {
        }

        public void TakeGroupSnapshots(Model model, CameraEngine ce, float w, float h)
        {
            if (model.PartGroups != null)
            {
                model.GroupSnapshots.Clear();

                List<PartGroup> groups = model.PartGroups;
                CreateRenderingContext(w, h);

                foreach (PartGroup bg in groups)
                {
                    model.GroupSnapshots[bg] = GetGroupSnapshot(bg, model, ce);
                }
                ReleaseContext();
            }
        }

        public void TakeWholeModelSnapshot(Model model, CameraEngine ce, float w, float h)
        {
            CreateRenderingContext(w, h);
            //		GameObject go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Plane) as GameObject;
            //		go.transform.localScale = new Vector3(20,20,20);
            //		go.transform.rotation = Quaternion.AngleAxis(275, Vector3.forward);
            //		Vector3 worldCameraForwardDirection = snapCamera.transform.TransformDirection(Vector3.forward);
            //		go.transform.position = snapCamera.transform.position + worldCameraForwardDirection*80;
            //		Material planeMat = new Material(Shader.Find("Diffuse"));
            //		planeMat.color = new Color(1, 0, 0, 125);
            //		(go.renderer as MeshRenderer).material = planeMat;
            //		Debug.Log ("here");
            model.AssembleImmediate();
            Bounds fullModelBounds = model.GetFullModelBounds();
            snapCamera.transform.position = ce.ComputeCameraPosition((ce.ComputeBoundsDistance(fullModelBounds) * 1.2f), fullModelBounds.center);
            snapCamera.transform.transform.LookAt(fullModelBounds.center);
            model.WholeModelSnapshot = new Texture2D(snapCamera.targetTexture.width, snapCamera.targetTexture.height, TextureFormat.ARGB32, false);
            //		Debug.Break();
            snapCamera.Render();
            model.WholeModelSnapshot.ReadPixels(new Rect(0, 0, snapCamera.targetTexture.width, snapCamera.targetTexture.height), 0, 0, false);
            model.WholeModelSnapshot.Apply();
            ReleaseContext();
            //		UnityEngine.Object.Destroy(go);

        }

        private void CreateRenderingContext(float w, float h)
        {
            RenderTexture rt = new RenderTexture((int)Mathf.Floor(w), (int)Mathf.Floor(h), 24);
            snapCamera.targetTexture = rt;
            RenderTexture.active = rt;
        }

        private void ReleaseContext()
        {
            snapCamera.targetTexture = null;
            RenderTexture.active = null;
        }

        private Texture2D GetGroupSnapshot(PartGroup bg, Model model, CameraEngine ce)
        {
            Bounds groupBounds = bg.GetBounding();
            model.SetNonFocusGroupsHidden(bg, true);
            snapCamera.transform.position = ce.ComputeCameraPosition((ce.ComputeBoundsDistance(groupBounds) * 1.4f), groupBounds.center);
            snapCamera.transform.transform.LookAt(groupBounds.center);

            Texture2D buttonImage = new Texture2D(snapCamera.targetTexture.width, snapCamera.targetTexture.height, TextureFormat.ARGB32, false);
            snapCamera.Render();
            model.SetNonFocusGroupsHidden(bg, false);
            buttonImage.ReadPixels(new Rect(0, 0, snapCamera.targetTexture.width, snapCamera.targetTexture.height), 0, 0, false);
            buttonImage.Apply();

            return buttonImage;
        }
    }
}
