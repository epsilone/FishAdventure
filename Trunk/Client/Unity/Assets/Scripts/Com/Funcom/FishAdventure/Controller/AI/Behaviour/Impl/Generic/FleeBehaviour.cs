using System.Collections.Generic;
using UnityEngine;

/**
 * LivingEntity  Attempts to move away from other living entities
 * **/

internal class FleeBehaviour : BaseSocialBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(FleeBehaviour));

    private static float FLEE_DISTANCE = 50;
    private static float ANIMATION_TIME = 0.01f;

    public FleeBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntities)
        : base(type, weight, triggerEntities)
    {
    }

    private BaseLivingEntity fleedEntity = null;

    public override void Start()
    {
        if (TargetEntity is BaseLivingEntity)
        {
            fleedEntity = (BaseLivingEntity)TargetEntity;
        }
        iTween.ColorUpdate(LivingEntity.gameObject, Color.blue, .01f);
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
        }
    }

    public override void Update()
    {
        base.Update();
        iTween.ColorUpdate(LivingEntity.gameObject, Color.blue, .01f);
        //        float deltaX = LivingEntity.transform.position.x - fleedEntity.transform.position.x;
        //        float deltaY = LivingEntity.transform.position.y - fleedEntity.transform.position.y;
        //        logger.Log("DeltaX  " + deltaX + " DeltaY " + deltaY);
        //        if (fleedEntity != null && (Math.Abs(deltaX) < FLEE_DISTANCE && Math.Abs(deltaY) < FLEE_DISTANCE))
        //        {
        //            Vector3 direction = LivingEntity.transform.position - fleedEntity.transform.position;
        //            logger.Log("Direction not normalized " + direction);
        //            direction.Normalize();
        //            logger.Log("Direction normalized " + direction);
        //            direction.z = fleedEntity.transform.position.z;
        //            logger.Log("MoveUpdate");
        //            logger.Log("Direction normalized multiplied " + direction * 10);
        //            LivingEntity.gameObject.transform.Translate(direction);
        //
        //            //iTween.MoveBy(LivingEntity.gameObject, direction * 10, ANIMATION_TIME);
        //        }

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