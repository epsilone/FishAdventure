using UnityEngine;

internal class IdleBehaviour : BaseBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(IdleBehaviour));

    public IdleBehaviour(BehaviourType type, int weight)
        : base(type, weight)
    {
    }

    public override void Start()
    {
        iTween.ColorUpdate(LivingEntity.gameObject, Color.white, .01f);
        //  LivingEntity.transform.Rotate(Vector3.forward * Time.deltaTime * 100);

        logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
    }

    public override void Update()
    {
        iTween.ColorUpdate(LivingEntity.gameObject, Color.white, .01f);
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " update");
        }
    }

    public override void Stop()
    {

        logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " stop");
    }
}