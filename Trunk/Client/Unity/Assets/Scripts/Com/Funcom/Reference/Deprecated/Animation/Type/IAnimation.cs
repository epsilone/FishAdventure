using UnityEngine;

namespace Com.Funcom.FishAdventure.Factory.Animation.Type
{
    public interface IAnimation
    {
        void Destroy();
        void Update();
        void Start(IAnimationParameter parameter);
        void Stop();
        void SetTarget(GameObject target);
    }
}
