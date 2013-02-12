using System;

namespace Com.Funcom.FishAdventure.Controller.AI.Behaviour
{
    public class BaseBehaviour : IBehaviour, IComparable 
    {
        public int Score { get; set; }

        protected int animationTypeId; //TODO (kaiv): Make sure it still needed
        protected IAnimationParameter animationParameter; //TODO (kaiv): Make sure it still needed

        public BaseBehaviour()
        {
            Init();
        }

        protected virtual void Init()
        {
            Score = 100;
        }

        public virtual void Destroy()
        {

        }

        /*public virtual void ComputeStatusData(Portfolio portfolio)
        {
            //Set the score in function of the portfolio
        }

        public virtual Personality GetImpact()
        {
            //Return the impact of the behaviour on the personality
            return new Personality();
        }*/

        public virtual int GetAnimationTypeId()
        {
            //Return the current animation linked to the behaviour
            return 0;
        }

        public virtual IAnimationParameter GetAnimationParameter()
        {
            //Return the current animation linked to the behaviour
            return (new object()) as IAnimationParameter;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) { return 1; }

            IBehaviour behaviour = obj as IBehaviour;
            if (behaviour != null)
            {
                return this.Score.CompareTo(behaviour.Score);
            }
            else
            {
                throw new ArgumentException("Object is not a behaviour");
            }
        }
    }
}
