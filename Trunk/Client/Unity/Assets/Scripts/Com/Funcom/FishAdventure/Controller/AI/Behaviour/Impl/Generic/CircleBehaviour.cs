using System.Collections.Generic;
using UnityEngine;

// Not applicable if both are circling
internal class CircleBehaviour : BaseSocialBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(CircleBehaviour));
    private BaseLivingEntity circleCenter;

    public CircleBehaviour(BehaviourType type, int weight, List<EntityType> triggerEntities)
        : base(type, weight, triggerEntities)
    {
    }

    public override void Start()
    {
        iTween.ColorUpdate(LivingEntity.gameObject, Color.green, .01f);

        if (TargetEntity is BaseLivingEntity)
        {
            circleCenter = (BaseLivingEntity)TargetEntity;
            //     LivingEntity.transform.parent = circleCenter.transform;
        }
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
        }
    }

    public override void Update()
    {
        base.Update();
        iTween.ColorUpdate(LivingEntity.gameObject, Color.green, .01f);

        // ellipse
        // LivingEntity.transform.localPosition = new Vector3(2 * Mathf.Cos(Time.time), 0, 3 * Mathf.Sin(Time.time));
        // circle

        //  LivingEntity.transform.localPosition = new Vector3(3 * Mathf.Cos(2 * Time.time), 3 * Mathf.Sin(2 * Time.time), 0);
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " update");
        }
    }

    public override void Stop()
    {
        base.Stop();
        LivingEntity.transform.parent = null;
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " stop");
        }
    }
}