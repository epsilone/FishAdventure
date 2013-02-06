using UnityEngine;
using System.Collections.Generic;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class Model:MonoBehaviour
    {

        public Interpolate.Function explodeEase; // easing of a particular EaseType
        public Interpolate.Function implodeEase; // easing of a particular EaseType

        private Dictionary<PartGroup, Texture2D> groupSnapshots = new Dictionary<PartGroup, Texture2D>();
        public Dictionary<PartGroup, Texture2D> GroupSnapshots
        {
            get
            {
                return groupSnapshots;
            }
            set
            {
                groupSnapshots = value;
            }
        }

        private Texture2D wholeModelSnapshot;
        // setters for the snapshots here....


        public Texture2D WholeModelSnapshot
        {
            get
            {
                return wholeModelSnapshot;
            }
            set
            {
                wholeModelSnapshot = value;
            }
        }

        public class PartComparer : IComparer<Part>
        {
            public int Compare(Part p1, Part p2)
            {
                return (p1.GetPartBounds().min.y > p2.GetPartBounds().min.y) ? 0 : -1;
            }
        }

        private Bounds fullModelBounds = new Bounds();
        public Bounds FullModelBounds
        {
            get
            {
                return fullModelBounds;
            }
        }


        private List<PartGroup> partGroups;
        private List<Part> sceneParts = new List<Part>();
        //private List<GameObject> flexContainer;
        private float elapsedTime = 0.0f;
        private float duration = 0.0f;

        public int partsInFlightCount = 0;
        public int partsInMaterialLerpCount = 0;
        public bool IsQuiesent()
        {
            return (partsInFlightCount <= 0 && partsInMaterialLerpCount <= 0);
        }

        private List<Part> partsNotInThisModel;

        private Dictionary<Part, ModelPartDataItem> modelPartData = new Dictionary<Part, ModelPartDataItem>();
        private Dictionary<Part, ModelPartDataItem> notInModelPartData = new Dictionary<Part, ModelPartDataItem>();

        void Awake()
        {
            //assembledModelData = new Dictionary<Part, ModelPartDataItem>();
            //sceneParts = new List<Part>();
            //flexContainer = new List<GameObject>();
        }
        void Start()
        {
        }

        //	public Texture2D GetGroupSnapshotTextureByName(string name)
        //	{
        //		if(groupSnapshots.ContainsKey(name))
        //		{
        //			return groupSnapshots[name];
        //		}
        //		return null;
        //	}

        public Texture2D GetGroupSnapshotTexture(PartGroup pg)
        {
            if (groupSnapshots.ContainsKey(pg))
            {
                return groupSnapshots[pg];
            }
            return null;
        }

        public Texture2D GetWholeModelSnapshotTexture()
        {
            return wholeModelSnapshot;
        }


        public List<PartGroup> PartGroups
        {
            get
            {
                return partGroups;
            }
            set
            {
                partGroups = value;
            }
        }

        public List<Part> ListOfUnusedParts
        {
            get
            {
                return partsNotInThisModel;
            }
        }

        public void SetLayerRecursively(Transform nextT)
        {
            foreach (Transform child in nextT)
            {
                child.gameObject.layer = 8;
                SetLayerRecursively(child);
            }
        }

        public void CreateListOfUnusedParts()
        {	// NB. to be called after all models have been loaded
            List<Part> allParts = PartDatabase.Singleton.GetAllParts();
            foreach (Part p in sceneParts)
            {
                allParts.Remove(p);
            }
            partsNotInThisModel = allParts;
            // shame we can't do this... partsNotInThisModel = allParts - sceneParts; without lots of "effort"

            foreach (Part p in partsNotInThisModel)
            {
                p.usedInModel = false;
                p.gameObject.transform.parent = transform;
                ModelPartDataItem mda = new ModelPartDataItem(p.gameObject.transform.position, p.gameObject.transform.rotation);
                notInModelPartData[p] = mda;
            }
        }

        public Bounds GetFullModelBounds()
        {
            return fullModelBounds;
        }

        public float Duration
        {
            set
            {
                duration = value;
            }
            get
            {
                return duration;
            }
        }

        public bool IsAnimating()
        {
            return (partsInFlightCount > 0);
        }

        public float CurrentTime
        {
            get
            {
                return elapsedTime;
            }
            set
            {
                elapsedTime = Mathf.Clamp(value, 0, duration);
            }
        }

        public void StartFocusTransparency(PartGroup focusGroup, bool transparent)
        {
            partsInMaterialLerpCount = 0; // part itself updates this if it decides it should start a material lerp
            foreach (Part p in sceneParts)
            {
                p.TransparencyRoutine(focusGroup != null && focusGroup.ContainsPart(p) ? !transparent : transparent);
            }
        }

        public void SetNonFocusGroupsHidden(PartGroup focusGroup, bool hidden)
        {
            foreach (Part p in sceneParts)
            {
                if (!focusGroup.ContainsPart(p))
                {
                    p.Hidden(hidden);
                }
            }
        }

        //	public void AddFlexContainer(GameObject flex)
        //	{
        //		flexContainer.Add(flex);
        //	}

        public void AddPart(Part p)
        {
            p.gameObject.transform.parent = transform;
            sceneParts.Add(p);
            ModelPartDataItem mda = new ModelPartDataItem(p.gameObject.transform.position, p.gameObject.transform.rotation);
            modelPartData[p] = mda;
        }

        public void PostLoadRoutine()
        {
            sceneParts.Sort(new PartComparer());
            fullModelBounds = sceneParts[0].GetPartBounds();

            for (int i = 1; i < sceneParts.Count; ++i)
            {
                fullModelBounds.Encapsulate(sceneParts[i].GetPartBounds());
            }

            foreach (PartGroup pg in partGroups)
            {
                pg.ComputeBounding();
            }

            duration = 1.0f;
            elapsedTime = 0.0f;
        }

        void Update()
        {
            if (IsAnimating())
            {
                elapsedTime += Time.deltaTime;
                SetTime(elapsedTime);
            }
        }

        public void SetTime(float time)
        {
            if (sceneParts.Count > 0)
            {
                this.elapsedTime = time;
                foreach (Part p in sceneParts)
                {
                    p.UpdatePart(time);
                }
                foreach (Part p in partsNotInThisModel)
                {
                    p.UpdatePart(time);
                }
            }
        }

        public void Clear()
        {
            //		foreach(Part p in sceneParts)
            //		{
            //			UnityEngine.Object.Destroy(p.GetGameObject());
            //		}
            //		
            //		foreach(GameObject flex in flexContainer)
            //		{
            //			UnityEngine.Object.Destroy(flex);
            //		}


            sceneParts.Clear();

            if (partGroups != null)
            {
                partGroups.Clear();
            }
        }

        public void AssembleImmediate()
        {
            partsInFlightCount = 0;
            foreach (Part p in sceneParts)
            {
                p.SetModelData(this, modelPartData[p]);
                p.AssembleImmediate();
            }
            if (partsNotInThisModel != null)
            {
                foreach (Part p in partsNotInThisModel)
                {
                    p.SetModelDataUnused(this);//, notInModelPartData[p]);
                    p.SetParkedImmediate();
                }
            }
        }

        public void ComputeExplodeTrajectories()
        {
            foreach (Part p in sceneParts)
            {
                modelPartData[p].ComputeExplodeTrajectory(p.ExplodedPosition);
                modelPartData[p].ComputeImplodeTrajectory(p.ExplodedPosition);
            }
        }

        public void Explode(float pDuration)
        {
            partsInFlightCount = 0;
            foreach (Part p in sceneParts)
            {
                p.Hidden(false);
                p.usedInModel = true;
                p.explodeEase = explodeEase; // this is done here instead of in ModelViewerMain::Start so we can experiment in real time
                p.SetOpaque();
                p.Trajectory = modelPartData[p].ExplodeTrajectory;
                p.Explode(0.0f, pDuration);
                partsInFlightCount++;
            }
            foreach (Part p in partsNotInThisModel)
            {   // this is the "fly-in" of the parts not in this model from the cloud just above scene
                // the parts fly from above to arrive in the cloud at the same time as the exploded parts
                // which are in the model
                p.explodeEase = explodeEase; // this is done here instead of in ModelViewerMain::Start so we can experiment in real time
                p.Hidden(false);
                p.usedInModel = false;
                p.Explode(0.0f, pDuration);
                partsInFlightCount++;
                p.SetOpaque();
            }
            elapsedTime = 0.0f;
        }

        public void Implode(float pDuration)
        {	// set assembled positions and rotation from the model to be assembled
            partsInFlightCount = 0;
            foreach (Part p in sceneParts)
            {
                p.implodeEase = implodeEase; // this is done here instead of in ModelViewerMain::Start so we can experiment in real time			
                p.usedInModel = true;
                p.SetModelData(this, modelPartData[p]);
                p.Trajectory = modelPartData[p].ImplodeTrajectory;
                p.Implode(0.0f, pDuration); // + flightStart offset time if we want to do that
                p.Hidden(false);
                partsInFlightCount++;
            }
            foreach (Part p in partsNotInThisModel)
            {
                p.implodeEase = implodeEase; // this is done here instead of in ModelViewerMain::Start so we can experiment in real time			
                p.usedInModel = false;
                partsInFlightCount++;
                p.SetModelDataUnused(this);
                p.Implode(0.0f, pDuration); // + flightStart offset time if we want to do that
                p.Hidden(false);
            }
            Debug.Log("IMPLODE parts in flight =" + partsInFlightCount.ToString());
            elapsedTime = 0.0f;
        }

        public void TakeGroupSnapshots(SnapshotCamera sc, CameraEngine ca, float w, float h)
        {	// must be assembled and all non used parts hidden
            sc.TakeGroupSnapshots(this, ca, w, h);
        }
        public void TakeWholeModelSnapshot(SnapshotCamera sc, CameraEngine ca, float w, float h)
        {
            // must be assembled and all non used parts hidden
            sc.TakeWholeModelSnapshot(this, ca, w, h);
        }
    }
}