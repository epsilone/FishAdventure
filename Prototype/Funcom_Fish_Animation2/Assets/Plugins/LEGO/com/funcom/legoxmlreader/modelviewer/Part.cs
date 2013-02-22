using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class Part : MonoBehaviour
    {
        public Interpolate.Function explodeEase; // easing of a particular EaseType
        public Interpolate.Function implodeEase; // easing of a particular EaseType

        private Model currentlyUsingModel;
        private float materialChangeInterval = 1.5f;
        private Vector3 assembledPosition;
        private Quaternion assembledRotation;

        private Vector3 explodedPosition;
        private Quaternion explodedRotation = Quaternion.identity;

        private Vector3 outOfScenePosition;
        private Quaternion outOfSceneRotation = Quaternion.identity;

        public bool usedInModel; // if used, explode/implode routines use the cloud animations, otherwise out/in scene routines

        public enum State { STOP, EXPLODE, IMPLODE };
        public State state;
        private float startTime;
        private float duration;
        private Vector3[] trajectory; // set up by model using modelPartDataItems
        public Vector3[] Trajectory
        {
            set
            {
                trajectory = value;
            }
        }
        //	private Vector3 [] nextModelTrajectory;
        //	public Vector3[] NextModelTrajectory
        //	{
        //		set
        //		{
        //			nextModelTrajectory = value;
        //		}
        //	}	
        //	
        private Material transparentMat;
        private Dictionary<string, Material> materialMap;

        private List<int> decorationIdList;
        private int materialId = 0;
        private int designId = 0;

        public Quaternion AssembledRotation
        {
            get
            {
                return assembledRotation;
            }
            set
            {
                assembledRotation = value;
            }
        }

        public Vector3 AssembledPosition
        {
            get
            {
                return assembledPosition;
            }
            set
            {
                assembledPosition = value;
            }
        }
        public Quaternion ExplodedRotation
        {
            get
            {
                return explodedRotation;
            }
            set
            {
                explodedRotation = value;
            }
        }

        public Vector3 ExplodedPosition
        {
            get
            {
                return explodedPosition;
            }
            set
            {
                explodedPosition = value;
            }
        }

        public Quaternion OutOfSceneRotation
        {
            get
            {
                return outOfSceneRotation;
            }
            set
            {
                outOfSceneRotation = value;
            }
        }

        public Vector3 OutOfScenePosition
        {
            get
            {
                return outOfScenePosition;
            }
            set
            {
                outOfScenePosition = value;
            }
        }

        public void SetData(int pDesignId, int pMaterialId, List<int> pDecorations)
        {
            designId = pDesignId;
            materialId = pMaterialId;
            decorationIdList = pDecorations;
        }

        private bool alreadyInUse = false; // means that during model/part database construction, this part is already in use - reset after loading
        public bool AlreadyInUse
        {
            get
            {
                return alreadyInUse;
            }
            set
            {
                alreadyInUse = value;
            }
        }


        public int DesignId
        {
            get
            {
                return designId;
            }
            set
            {
                designId = value;
            }
        }

        public int MaterialId
        {
            get
            {
                return materialId;
            }
            set
            {
                materialId = value;
            }
        }

        public List<int> Decorations
        {
            get
            {
                return decorationIdList;
            }
        }

        public void AddDecorationId(int pId)
        {
            decorationIdList.Add(pId);
            if (decorationIdList.Count > 1)
            {	// make sure it's sorted ready for comparison
                decorationIdList.Sort();
            }
        }

        void Awake()
        {
            currentlyUsingModel = null;
            state = State.STOP;
            decorationIdList = new List<int>();
            materialMap = new Dictionary<string, Material>();
        }

        //	public void SetupEndState(float duration)
        //	{
        //		this.duration = duration;
        //		
        //		endPosition = gameObject.transform.position;
        //		endRotation = gameObject.transform.rotation;
        //	}

        public void SetupStartState()
        {
            transparentMat = new Material(Shader.Find("Transparent/Specular"));
            Renderer r = gameObject.renderer;

            if (r != null)
            {
                materialMap[gameObject.name] = new Material(r.material);
            }
            foreach (Transform child in gameObject.transform)
            {
                GameObject cgo = child.gameObject;
                Renderer cr = cgo.renderer;
                if (cr != null)
                {
                    materialMap[cgo.name] = new Material(cr.material);
                }
            }
        }

        public void SetOpaque()
        {
            Renderer renderer = gameObject.renderer;
            if (renderer != null)
            {
                Material oldMat = renderer.material;
                if (oldMat.mainTexture != null)
                {
                    renderer.enabled = true;
                }
                else
                {
                    renderer.material = materialMap[gameObject.name];
                }
            }
            else
            {
                foreach (Transform child in gameObject.transform)
                {
                    bool skip = false;
                    GameObject cgo = child.gameObject;
                    Renderer childrenderer = cgo.renderer;
                    if (childrenderer != null)
                    {
                        if (childrenderer.material.mainTexture != null)
                        {
                            childrenderer.enabled = true;
                            skip = true;
                        }

                        if (!skip)
                        {
                            childrenderer.material = materialMap[cgo.name];
                        }
                    }
                }
            }
        }


        //~ void Update()
        //~ {
        //~ if(trajectory!=null)
        //~ {
        //~ foreach(Vector3 v in trajectory)
        //~ {
        //~ Debug.DrawLine(Vector3.zero, v, Color.red);
        //~ }
        //~ }
        //~ }

        public void Hidden(bool state)
        {
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = !state;
            }
            foreach (Transform child in gameObject.transform)
            {
                GameObject cgo = child.gameObject;
                MeshRenderer cmr = cgo.GetComponent<MeshRenderer>();
                if (cmr != null)
                {
                    cmr.enabled = !state;
                }
            }
        }

        public void TransparencyRoutine(bool transparent)
        {
            Renderer renderer = gameObject.renderer;
            if (renderer != null)
            {
                Material oldMat = renderer.material;
                if (oldMat.mainTexture != null)
                {
                    renderer.enabled = !transparent;
                }
                else
                {
                    if (transparent)
                    {
                        Color c = materialMap[gameObject.name].GetColor("_Color");
                        transparentMat.SetColor("_Color", c);
                        Material transparentCloneMat = new Material(transparentMat);
                        c.a = 0.1f;
                        transparentCloneMat.SetColor("_Color", c);
                        renderer.material = transparentMat;
                        StartCoroutine(LerpTransparentMaterial(renderer.material, transparentCloneMat));
                    }
                    else
                    {
                        StartCoroutine(LerpOpaqueMaterial(renderer.material, materialMap[gameObject.name], renderer));
                    }
                }
            }
            else
            {
                foreach (Transform child in gameObject.transform)
                {
                    bool skip = false;
                    GameObject cgo = child.gameObject;
                    Renderer childrenderer = cgo.renderer;
                    if (childrenderer != null)
                    {
                        if (childrenderer.material.mainTexture != null)
                        {
                            childrenderer.enabled = !transparent;
                            skip = true;
                        }

                        if (!skip)
                        {
                            if (transparent)
                            {
                                Color c = materialMap[cgo.name].GetColor("_Color");
                                transparentMat.SetColor("_Color", c);
                                Material transparentCloneMat = new Material(transparentMat);
                                c.a = 0.1f;
                                transparentCloneMat.SetColor("_Color", c);
                                childrenderer.material = transparentMat;
                                StartCoroutine(LerpTransparentMaterial(childrenderer.material, transparentCloneMat));
                            }
                            else
                            {
                                StartCoroutine(LerpOpaqueMaterial(childrenderer.material, materialMap[cgo.name], childrenderer));
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator LerpTransparentMaterial(Material startMat, Material endMat)
        {
            currentlyUsingModel.partsInMaterialLerpCount++;
            float elapsedTime = 0.0f;
            while (elapsedTime < materialChangeInterval)
            {
                elapsedTime += Time.deltaTime;
                startMat.Lerp(startMat, endMat, elapsedTime / materialChangeInterval);
                yield return null;
            }
            currentlyUsingModel.partsInMaterialLerpCount--;
        }

        private IEnumerator LerpOpaqueMaterial(Material startMat, Material endMat, Renderer r)
        {
            currentlyUsingModel.partsInMaterialLerpCount++;
            Material tempMat = new Material(startMat);
            Color c = tempMat.color;
            c.a = 1.0f;
            tempMat.SetColor("_Color", c);
            r.material = startMat;
            float elapsedTime = 0.0f;
            while (elapsedTime < materialChangeInterval)
            {
                elapsedTime += Time.deltaTime;
                startMat.Lerp(startMat, tempMat, elapsedTime / materialChangeInterval);
                yield return null;
            }
            r.material = endMat;
            currentlyUsingModel.partsInMaterialLerpCount--;
        }
        public void UpdatePart(float time)
        {
            switch (state)
            {
                default:
                case State.STOP:
                    break;

                case State.EXPLODE:
                    if (startTime <= time
                        && time < EndTime)
                    {
                        float elapsedTime = Mathf.Clamp(time - startTime, 0, duration);
                        if (usedInModel)
                        {	// part used in the model
                            float elapsedTimeFraction = elapsedTime / duration;
                            //slerped
                            int positionIndex = Mathf.Clamp(Mathf.RoundToInt(explodeEase(0, trajectory.Length, elapsedTime, duration)), 0, trajectory.Length - 1);
                            //gameObject.transform.position = trajectory[(int)Mathf.Clamp(Mathf.Abs(elapsedTimeFraction*trajectory.Length), 0, trajectory.Length-1)];
                            gameObject.transform.position = trajectory[positionIndex];
                            gameObject.transform.rotation = Quaternion.Slerp(assembledRotation, explodedRotation, elapsedTimeFraction);

                            // linear 
                            // gameObject.transform.position = Vector3.Lerp(assembledPosition, explodedPosition, elapsedTimeFraction);
                            // gameObject.transform.rotation = Quaternion.Lerp(assembledRotation, explodedRotation,elapsedTimeFraction);				
                        }
                        else
                        {	// part in parking above veiwable scene - bring down to cloud
                            // float start, float distance, float elapsedTime, float duration
                            float elapsedTimeFraction = explodeEase(0.0f, 1.0f, elapsedTime, duration);
                            gameObject.transform.position = Vector3.Lerp(outOfScenePosition, explodedPosition, elapsedTimeFraction);
                            gameObject.transform.rotation = Quaternion.Lerp(outOfSceneRotation, explodedRotation, elapsedTimeFraction);
                        }
                    }
                    else if (time >= EndTime)
                    {
                        gameObject.transform.position = explodedPosition;
                        gameObject.transform.rotation = explodedRotation;
                        //state = State.STOP;
                        state = State.IMPLODE;
                        currentlyUsingModel.partsInFlightCount--;
                        //				Debug.Log ("Part EXPLODE currentlyUsingModel.partsInFlightCount=" + currentlyUsingModel.partsInFlightCount.ToString());				
                    }
                    else if (time < startTime)
                    {
                        if (usedInModel)
                        {
                            gameObject.transform.position = assembledPosition;
                            gameObject.transform.rotation = assembledRotation;
                        }
                        else
                        {
                            gameObject.transform.position = outOfScenePosition;
                            gameObject.transform.rotation = outOfSceneRotation;
                        }
                    }
                    break;

                case State.IMPLODE:
                    if (startTime <= time
                        && time < EndTime)
                    {
                        float elapsedTime = Mathf.Clamp(time - startTime, 0, duration);

                        if (usedInModel)
                        {
                            //slerped
                            float elapsedTimeFraction = elapsedTime / duration;
                            int positionIndex = Mathf.Clamp(Mathf.RoundToInt(implodeEase(0, trajectory.Length, elapsedTime, duration)), 0, trajectory.Length - 1);
                            positionIndex = (trajectory.Length - 1) - positionIndex;
                            gameObject.transform.position = trajectory[positionIndex];
                            //gameObject.transform.position = trajectory[(int)Mathf.Clamp(Mathf.Abs(elapsedTimeFraction*trajectory.Length), 0, trajectory.Length-1)];
                            //gameObject.transform.position = trajectory[(int)Mathf.Clamp(Mathf.Abs(trajectory.Length - elapsedTimeFraction*trajectory.Length), 0, trajectory.Length-1)];
                            //gameObject.transform.position = Vector3.Slerp(explodedPosition, assembledPosition, elapsedTime);
                            gameObject.transform.rotation = Quaternion.Slerp(explodedRotation, assembledRotation, elapsedTimeFraction);
                        }
                        else
                        {
                            float elapsedTimeFraction = explodeEase(0.0f, 1.0f, elapsedTime, duration);
                            gameObject.transform.position = Vector3.Lerp(explodedPosition, outOfScenePosition, elapsedTimeFraction);
                            gameObject.transform.rotation = Quaternion.Lerp(explodedRotation, outOfSceneRotation, elapsedTimeFraction);
                        }
                    }
                    else if (time >= EndTime)
                    {
                        if (usedInModel)
                        {
                            gameObject.transform.position = assembledPosition;
                            gameObject.transform.rotation = assembledRotation;
                        }
                        else
                        {
                            gameObject.transform.position = outOfScenePosition;
                            gameObject.transform.rotation = outOfSceneRotation;
                            Hidden(true);
                        }
                        state = State.STOP;
                        currentlyUsingModel.partsInFlightCount--;
                    }
                    else if (time < startTime)
                    {
                        gameObject.transform.position = explodedPosition;
                        gameObject.transform.rotation = explodedRotation;
                    }
                    break;

            }
        }

        public Bounds GetPartBounds()
        {
            Bounds result = new Bounds(gameObject.transform.position, Vector3.zero);

            if (gameObject.renderer != null)
            {
                result.Encapsulate(gameObject.renderer.bounds);
            }
            foreach (Transform child in gameObject.transform)
            {
                Renderer r = child.gameObject.renderer;
                if (r != null)
                {
                    result.Encapsulate(r.bounds);
                }
            }
            return result;
        }

        //	public bool InFlight()
        //	{
        //		switch (state)
        //		{
        //		case	State.STOP:		return false; break;
        //		case	State.EXPLODE:	return (gameObject.transform.position != explodedPosition || gameObject.transform.rotation != explodedRotation); break;
        //		case	State.IMPLODE:	return (gameObject.transform.position != assembledPosition || gameObject.transform.rotation != assembledRotation); break;			
        //		default:				return false; break;
        //		}
        //	}

        public float EndTime
        {
            get
            {
                return startTime + duration;
            }
        }

        public float StartTime
        {
            get
            {
                return startTime;
            }
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetModelDataUnused(Model pModel/*, ModelPartDataItem pModelPartData*/)
        {
            currentlyUsingModel = pModel;
            usedInModel = false;
            //transform.position	= pModelPartData.ExplodedPosition;
            //transform.rotation	= pModelPartData.ExplodedRotation;
            //Hidden(true);
        }

        public void SetModelData(Model pModel, ModelPartDataItem pModelPartData)
        {
            currentlyUsingModel = pModel;
            usedInModel = true;
            assembledPosition = pModelPartData.AssembledPosition;
            assembledRotation = pModelPartData.AssembledRotation;
        }

        public void Explode(float pStartTime, float pDuration)
        {
            startTime = pStartTime;
            duration = pDuration;
            state = State.EXPLODE;
        }
        public void Implode(float pStartTime, float pDuration)
        {
            startTime = pStartTime;
            duration = pDuration;
            state = State.IMPLODE;
        }

        public void AssembleImmediate()
        {
            currentlyUsingModel.partsInFlightCount = 0;
            if (usedInModel)
            {
                transform.position = assembledPosition;
                transform.rotation = assembledRotation;
                Hidden(false);
            }
            else
            {
                transform.position = outOfScenePosition;
                transform.rotation = outOfSceneRotation;
                Hidden(true);
            }
            state = State.STOP;
            SetupStartState();
        }

        public void SetParkedImmediate()
        {
            transform.position = outOfScenePosition;
            transform.rotation = outOfSceneRotation;
        }
    }
}