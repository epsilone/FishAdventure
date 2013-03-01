using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

abstract class BaseSocialBehaviour : BaseBehaviour, ISocialBehaviour
{
    protected BaseSocialBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntitiesTypes)
        : base(type, weight)
    {
        this.triggerEntitiesTypes = triggerEntitiesTypes;
    }


    private IEntity targetEntity;

    internal IEntity TargetEntity
    {
        get { return targetEntity; }
    }


    private List<EntityType> triggerEntitiesTypes;

    public List<EntityType> TriggerEntitiesTypes
    {
        set { triggerEntitiesTypes = value; }
    }

    public List<EntityType> getTriggeringTypes()
    {
        return triggerEntitiesTypes;
    }


    public void SetTargetEntity(IEntity entity)
    {
        targetEntity = entity;
    }

    public override void Update()
    {
        if (targetEntity != null && targetEntity is BaseLivingEntity)
        {
            BaseLivingEntity targetLivingEntity = (BaseLivingEntity)targetEntity;
            LineRenderer lr = (LineRenderer)LivingEntity.gameObject.GetComponent(typeof(LineRenderer));
            if (lr == null)
            {
                lr = (LineRenderer)LivingEntity.gameObject.AddComponent(typeof(LineRenderer));
            }
            lr.SetWidth(0.1f, 1f);
            lr.SetPosition(0, LivingEntity.transform.position);
            lr.SetPosition(1, targetLivingEntity.transform.position);
        }
    }


    public override void Stop()
    {
        if (targetEntity != null && targetEntity is BaseLivingEntity)
        {

            LineRenderer lr = (LineRenderer)LivingEntity.gameObject.GetComponent(typeof(LineRenderer));
            if (lr != null)
            {
                lr.enabled = false;
                BaseLivingEntity.Destroy(lr);
            }

        }
    }
}
