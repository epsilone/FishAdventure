using Com.Funcom.FishAdventure.Factory.Animation.Type;
namespace Com.Funcom.FishAdventure.Factory.Animation
{
    public static class AnimationFactory
    {
        public static IAnimation GetAnimation(int animationTypeId, int fishTypeId)
        {
            return (new object()) as IAnimation;
        }
    }
}