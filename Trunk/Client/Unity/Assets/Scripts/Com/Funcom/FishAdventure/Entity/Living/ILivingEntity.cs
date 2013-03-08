public interface ILivingEntity : IEntity
{
    IBehaviour GetCurrentBehaviour();

    INeed GetCurrentNeed();


    /**
     * Used as a generic method for itween callbacks
     * */
    void GenericTweenUpdate(object args);
}