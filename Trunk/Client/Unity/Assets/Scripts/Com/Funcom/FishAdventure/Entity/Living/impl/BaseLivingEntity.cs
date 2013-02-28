using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


abstract class BaseLivingEntity : MonoBehaviour, ILivingEntity
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(FleeBehaviour));

    public String informationBoxBehaviour;
    public String informationBoxNeed;
    Need currentNeed;

    internal Need CurrentNeed
    {
        get { return currentNeed; }
 
    }
    List<Need> supportedNeeds;

    internal List<Need> SupportedNeeds
    {
        get { return supportedNeeds; }
        set { supportedNeeds = value; }
    }
    List<IBehaviour> supportedBehaviours;

    internal List<IBehaviour> SupportedBehaviours
    {
        get { return supportedBehaviours; }
        set { supportedBehaviours = value; }
    }

    IBehaviour currentBehaviour;

    internal IBehaviour CurrentBehaviour
    {
        get { return currentBehaviour; }
    }

    public void SetCurrentBehaviour(IBehaviour behaviour)
    {
        this.currentBehaviour = behaviour;
        informationBoxBehaviour = behaviour.ToString();
        //if (listeners != null)
        //{
        //    foreach (ILivingEntityListener listener in listeners)
        //    {
        //        listener.BehaviourChange(this);
        //    }

        //}
    }


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

    private AIController aiController;

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
        currentBehaviour.TweenUpdate();
        //Debug.Log("Update method itween" + Time.timeSinceLevelLoad);
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

    public virtual void Update()
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.Update();
        }
    }


    void OnTriggerEnter(Collider collision)
    {
        GameObject o = collision.gameObject;

        Component livEntityComp = o.GetComponent(typeof(BaseLivingEntity));
        if (livEntityComp != null)
        {
          
            BaseLivingEntity e = (BaseLivingEntity)livEntityComp;
            aiController.AddNeighbour(e);
        }
    }

    void OnTriggerExit(Collider collision)
    {
        GameObject o = collision.gameObject;

        Component livEntityComp = o.GetComponent(typeof(BaseLivingEntity));
        if (livEntityComp != null)
        {
  
            BaseLivingEntity e = (BaseLivingEntity)livEntityComp;
            aiController.RemoveNeighbour(e);
        }

    }

    private void CalibrateWeights()
    {
        logger.Log("Calibrating weights");

        logger.Log("Calibrating weights Not implemented");

        logger.Log("Calibrating weights END, 0 changes");

    }

    private void generateNeed()
    {
        aiController.GenerateNeed();
    }

    private void generateBehaviour()
    {
        aiController.GenerateBehaviour();
    }

    public int id;
    public String entityName;

    public float layer;

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

    

    public virtual int GetId()
    {
        return id;
    }


    public abstract EntityType GetEntityType();

    public abstract Aura getAura();

    IBehaviour ILivingEntity.GetCurrentBehaviour()
    {
        return currentBehaviour;
    }


    public INeed GetCurrentNeed()
    {
        return currentNeed;
    }
}
