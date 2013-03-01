using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * LivingEntity  Attempts to move away from other living entities
 * **/
class FleeBehaviour : BaseSocialBehaviour
{

    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(FleeBehaviour));

    public FleeBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntities)
        : base(type, weight, triggerEntities)
    {

    }


    public override void Start()
    {

        iTween.ColorFrom(LivingEntity.gameObject, Color.blue, .3f);
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
        }

    }

    public override void Update()
    {
        base.Update();
        iTween.ColorFrom(LivingEntity.gameObject, Color.blue, .3f);
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
