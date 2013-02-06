using UnityEngine;

namespace Com.Funcom.FishAdventure.Factory.Animation.Type
{
    public class BaseAnimation : IAnimation
    {
        protected GameObject target;
        protected IAnimationParameter parameter;

        //AnimationStart Event
        public delegate void AnimationStart();
        public event AnimationStart OnAnimationStart;

        //AnimationStop Event
        public delegate void AnimationStop();
        public event AnimationStop OnAnimationStop;

        public BaseAnimation()
        {
            Init();
        }

        protected virtual void Init()
        {

        }

        public virtual void Destroy()
        {

        }

        public virtual void Update()
        {
            
        }

        public virtual void Start(IAnimationParameter parameter)
        {
            this.parameter = parameter;

            OnAnimationStart();
        }

        public virtual void Stop()
        {
            OnAnimationStop(); // TODO (Kaiv): This is asynch, an animationc an take a little bit of time to stop
        }

        public virtual void SetTarget(GameObject target)
        {
            this.target = target;
        }
    }
}
