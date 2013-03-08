using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Liging Entity Follows another with the intention of hitting it
 * */

internal class ChaseBehaviour : BaseSocialBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(ChaseBehaviour));

    private static float FOLLOW_DISTANCE = 0.3f;

    private static float CHASE_DISTANCE = 0.2f;

    private static float ANIMATION_TIME = 0.2f;

    //    float followDistance = 0;
    private BaseLivingEntity chased;

    public ChaseBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntities)
        : base(type, weight, triggerEntities)
    {
    }

    private Boolean wasMoving = true;

    public override void Start()
    {
        iTween.ColorUpdate(LivingEntity.gameObject, Color.magenta, .01f);

        if (TargetEntity is BaseLivingEntity)
        {
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
        iTween.ColorUpdate(LivingEntity.gameObject, Color.magenta, .01f);

        //        if (chased.transform.position.x - LivingEntity.transform.position.x < CHASE_DISTANCE)
        //        {
        //            if (!wasMoving)
        //            {
        //                iTween.Stop(LivingEntity.gameObject);
        //            }
        //            wasMoving = true;
        //
        //            logger.Log("Move Update");
        //            iTween.MoveUpdate(LivingEntity.gameObject, new Vector3(chased.transform.position.x - FOLLOW_DISTANCE, chased.transform.position.y - FOLLOW_DISTANCE, chased.transform.position.z), ANIMATION_TIME * 2);
        //        }
        //        else
        //        {
        //            if (wasMoving)
        //            {
        //                iTween.Stop(LivingEntity.gameObject);
        //            }
        //            iTween.Stop(LivingEntity.gameObject);
        //            logger.Log("Move By");
        //            iTween.MoveBy(LivingEntity.gameObject, new Vector3(chased.transform.position.x - LivingEntity.transform.position.x, chased.transform.position.y - LivingEntity.transform.position.y, 0), ANIMATION_TIME);
        //        }
        //        if (logger.IsExtreme())
        //        {
        //            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " update");
        //        }
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