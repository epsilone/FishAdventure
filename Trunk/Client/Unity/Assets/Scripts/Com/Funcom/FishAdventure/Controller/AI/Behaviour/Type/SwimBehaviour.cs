using Com.Funcom.FishAdventure.Component.Entity.Living.Type;

namespace Com.Funcom.FishAdventure.Controller.AI.Behaviour.Type
{
    public class SwimBehaviour:BaseBehaviour
    {
        public SwimBehaviour()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
        }

        public override void Destroy()
        {
            base.Destroy();
        }

       /* public override void ComputeStatusData(Portfolio portfolio)
        {
            base.ComputeStatusData(portfolio);
            //Set the score in function of the portfolio
        }

        public override Personality GetImpact()
        {
            base.GetImpact();
            //Return the impact of the behaviour on the personality
            return new Personality();
        }*/

        public override int GetAnimationTypeId()
        {
            base.GetAnimationTypeId();
            //Return the current animation linked to the behaviour
            return 0;
        }

        public override IAnimationParameter GetAnimationParameter()
        {
            base.GetAnimationParameter();
            //Return the current animation linked to the behaviour
            return (new object()) as IAnimationParameter;
        }
    }
}
