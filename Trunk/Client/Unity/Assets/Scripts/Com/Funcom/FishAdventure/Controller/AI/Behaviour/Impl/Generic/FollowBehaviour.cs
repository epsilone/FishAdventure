using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class FollowBehaviour : BaseSocialBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(FollowBehaviour));


    float followDistance = 30;
    private BaseLivingEntity followee;
    public FollowBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntities)
        : base(type, weight, triggerEntities)
    {
    }

    public override void Start()
    {
        iTween.ColorFrom(LivingEntity.gameObject, Color.black, .3f);
        if (TargetEntity is BaseLivingEntity) {
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
        iTween.ColorFrom(LivingEntity.gameObject, Color.black, .3f);
     //       iTween.MoveUpdate(LivingEntity.gameObject, new Vector3(followee.transform.position.x, 0, -followDistance), .8f);
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
}
