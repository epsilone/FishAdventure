using System.Collections.Generic;
using UnityEngine;

internal class FollowBehaviour : BaseSocialBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(FollowBehaviour));

    private static float FOLLOW_DISTANCE = 2;

    private static float ANIMATION_TIME = 2;

    private BaseLivingEntity followee;

    public FollowBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntities)
        : base(type, weight, triggerEntities)
    {
    }

    public override void Start()
    {
        Debug.Log("Follow starting " + TargetEntity);
        iTween.ColorUpdate(LivingEntity.gameObject, Color.black, .01f);

        if (TargetEntity is BaseLivingEntity)
        {
            followee = (BaseLivingEntity)TargetEntity;
        }
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
        }
    }

    public override void Update()
    {
        base.Update();
        iTween.ColorUpdate(LivingEntity.gameObject, Color.black, .01f);
        //  iTween.MoveUpdate(LivingEntity.gameObject, new Vector3(followee.transform.position.x - FOLLOW_DISTANCE, followee.transform.position.y - FOLLOW_DISTANCE, LivingEntity.transform.position.z), ANIMATION_TIME);
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " update");
        }
    }

    public override void Stop()
    {
        base.Stop();
        logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " stop");
    }

    public override string ToString()
    {
        return base.ToString();
    }
}