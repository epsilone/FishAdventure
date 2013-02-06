using UnityEngine;
using System.Collections;
using Com.Funcom.FishAdventure.Component.Fish.Profile;
using Com.Funcom.FishAdventure.Controller.AI;

namespace Com.Funcom.FishAdventure.Component.Fish
{
    public class Fish:MonoBehaviour
    {
        private Portfolio portfolio;
        private AIController aiController;

        void Awake()
        {
            aiController = new AIController();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            aiController.Update();
        }

        public Portfolio GetPortfolio()
        {
            return portfolio;
        }

        public void SetPortfolio(Portfolio portfolio)
        {
            this.portfolio = portfolio;
            aiController.SetTargetPortfolio(this.portfolio);
        }
    }
}
