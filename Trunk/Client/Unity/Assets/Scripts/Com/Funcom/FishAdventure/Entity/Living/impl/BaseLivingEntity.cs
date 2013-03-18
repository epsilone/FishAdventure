using System;
using System.Collections.Generic;
using UnityEngine;

internal abstract class BaseLivingEntity : MonoBehaviour, ILivingEntity
{
    public BehaviourType assignedBehaviour;
    public String entityName;
    public int id;
    public String informationBoxBehaviour;
    public String informationBoxNeed;
    public Boolean isAiManagerEnabled;
    public float layer;
    protected AIController aiController;
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(FleeBehaviour));
    private IBehaviour currentBehaviour;
    private Need currentNeed;



    public int rotation;
    public Vector3 vector;

    private List<IBehaviour> supportedBehaviours;

    private List<Need> supportedNeeds;

    internal IBehaviour CurrentBehaviour
    {
        get { return currentBehaviour; }
    }

    internal Need CurrentNeed
    {
        get { return currentNeed; }
    }

    internal List<IBehaviour> SupportedBehaviours
    {
        get { return supportedBehaviours; }
        set { supportedBehaviours = value; }
    }

    internal List<Need> SupportedNeeds
    {
        get { return supportedNeeds; }
        set { supportedNeeds = value; }
    }

    public abstract Aura getAura();

    public INeed GetCurrentNeed()
    {
        return currentNeed;
    }

    public abstract EntityType GetEntityType();

    public virtual int GetId()
    {
        return id;
    }

    public virtual string GetName()
    {
        if (entityName == null || (entityName != null && entityName.Length == 0))
        {
            return GetEntityType().ToString() + ":" + GetId();
        }
        else
        {
            return entityName;
        }
    }

    IBehaviour ILivingEntity.GetCurrentBehaviour()
    {
        return currentBehaviour;
    }

    public void SetCurrentBehaviour(IBehaviour behaviour)
    {
        this.currentBehaviour = behaviour;
        gameObject.name = GetName();
        informationBoxBehaviour = behaviour.ToString();

        //if (listeners != null)
        //{
        //    foreach (ILivingEntityListener listener in listeners)
        //    {
        //        listener.BehaviourChange(this);
        //    }

        //}
    }

    /* Debug ONLY */
    /* END OF Debug ONLY */

    public void SetCurrentNeed(Need need)
    {
        this.currentNeed = need;

        informationBoxNeed = need.GetNeedType().ToString();

        //if (listeners != null) {
        //    foreach (ILivingEntityListener listener in listeners){
        //        listener.NeedChanged(this);
        //    }

        //}
    }

    //internal List<ILivingEntityListener> listeners;
    //internal List<ILivingEntityListener> Listeners
    //{
    //    get { return listeners; }
    //}
    public virtual void Start()
    {
        SupportedBehaviours = FishTankFactory.getInstance().createSupportedBehaviours(this);
        SupportedNeeds = FishTankFactory.getInstance().createSupportedNeeds(this);
        AttachCollider();
        aiController = new AIController(this);
        aiController.Start();

        //
        //Factory
        // getEquipedItems()
        CalibrateWeights();
    }

    //public void AddListener(ILivingEntityListener listener) {
    //    if (listeners == null) {
    //        listeners = new List<ILivingEntityListener>();
    //    }
    //    Debug.Log("Adding listener " + listener + " LIsteners " + listeners);
    //    listeners.Add(listener);
    //}

    public void tweenUpdate()
    {
        //  currentBehaviour.TweenUpdate();
    }

    public void LateUpdate()
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.LateUpdate();
        }
    }

    public virtual void Update()
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.Update();
        }



        // end of debug only
    }

    private void AttachCollider()
    {
        Rigidbody body = (Rigidbody)gameObject.AddComponent(typeof(Rigidbody));
        body.useGravity = false;
        body.isKinematic = true;

        //body.constraints = RigidbodyConstraints.FreezeAll;
        SphereCollider collider = (SphereCollider)gameObject.AddComponent("SphereCollider");
        collider.isTrigger = true;
        logger.Log("Collider " + collider);

        collider.radius = getAura().Radius;
    }

    private void CalibrateWeights()
    {
        logger.Log("Calibrating weights");

        logger.Log("Calibrating weights Not implemented");

        logger.Log("Calibrating weights END, 0 changes");
    }

    private void generateBehaviour()
    {
        aiController.GenerateBehaviour();
    }

    private void generateNeed()
    {
        aiController.GenerateNeed();
    }

    private void OnMouseDown()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        transform.position = curPosition;
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject o = collision.gameObject;

        Component livEntityComp = o.GetComponent(typeof(BaseLivingEntity));
        if (livEntityComp != null)
        {
            BaseLivingEntity e = (BaseLivingEntity)livEntityComp;
            aiController.AddNeighbour(e);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        GameObject o = collision.gameObject;

        Component livEntityComp = o.GetComponent(typeof(BaseLivingEntity));
        if (livEntityComp != null)
        {
            BaseLivingEntity e = (BaseLivingEntity)livEntityComp;
            aiController.RemoveNeighbour(e);
        }
    }


    public void GenericTweenUpdate(object args)
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.GenericTweenUpdate(args);
        }

    }
    void OnDrawGizmos()
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.OnDrawGizmos();
        }
    }
}