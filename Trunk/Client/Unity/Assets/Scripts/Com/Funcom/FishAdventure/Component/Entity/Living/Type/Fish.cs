using System.Collections.Generic;
using Com.Funcom.FishAdventure.Controller.AI.Behaviour;
using Com.Funcom.FishAdventure.Controller.AI.Behaviour.Type;

namespace Com.Funcom.FishAdventure.Component.Entity.Living.Type
{
    public class Fish:LivingEntity
    {


        // Use this for initialization
        void Start()
        {
            base.Start();
        }

        void Update()
        {
            base.Update();
        }

        /*public Portfolio GetPortfolio()
        {
            return portfolio;
        }

        public void SetPortfolio(Portfolio portfolio)
        {
            this.portfolio = portfolio;
            aiController.SetTargetPortfolio(this.portfolio);
        }*/

        protected override List<IBehaviour> GetBehaviourList()
        {
            List<IBehaviour> behaviourList = new List<IBehaviour>();
            behaviourList.Add(new SwimBehaviour());

            return behaviourList;
        }
    }
}
