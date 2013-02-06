using Com.Funcom.FishAdventure.Component.Fish.Profile;

namespace Com.Funcom.FishAdventure.Controller.AI.Behaviour
{
    public interface IBehaviour
    {
        int Score { get; set; }
        void Destroy();
        void ComputeStatusData(Portfolio portfolio);
        Personality GetImpact();
        int GetAnimationTypeId();
        IAnimationParameter GetAnimationParameter();
    }
}
