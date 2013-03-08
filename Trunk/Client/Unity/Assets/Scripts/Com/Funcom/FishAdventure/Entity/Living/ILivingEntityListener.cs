//intinterface ILivingEntityListener
internal interface ILivingEntityListener
{
    void BehaviourChange(ILivingEntity entity);

    void NeedChanged(ILivingEntity entity);

    void DirectionChanged(ILivingEntity entity);
}