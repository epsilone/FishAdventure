using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * Liging Entity Follows another with the intention of hitting it
 * */
class ChaseBehaviour : BaseSocialBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(ChaseBehaviour));


//    float followDistance = 0;
    private BaseLivingEntity chased;
    public ChaseBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntities)
        : base(type, weight, triggerEntities)
    {
    }

    public override void Start()
    {
        iTween.ColorFrom(LivingEntity.gameObject, Color.black, .3f);
        if (TargetEntity is BaseLivingEntity) {
            chased = (BaseLivingEntity)TargetEntity;
        }
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
        }
    }

    public override void Update()
    {
        base.Update();
        iTween.ColorFrom(LivingEntity.gameObject, Color.black, .3f);
       // iTween.MoveUpdate(LivingEntity.gameObject, new Vector3(chased.transform.position.x, chased.transform.position.y, chased.transform.position.z), .3f);
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " update");
        }
    }

    public override void Stop()
    {
        base.Stop();
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " stop");
        }
    }
}
