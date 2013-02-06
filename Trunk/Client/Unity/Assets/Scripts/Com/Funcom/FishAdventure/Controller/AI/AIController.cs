using Com.Funcom.FishAdventure.Component.Fish.Profile;
using Com.Funcom.FishAdventure.Controller.AI.Behaviour;
using UnityEngine;
using System.Collections.Generic;

namespace Com.Funcom.FishAdventure.Controller.AI
{
    public class AIController
    {
        //Configuration
        private int suitableBehaviourCount = 3;
        private float cycleDelay = 1; //sec

        private Portfolio targetPortfolio;
        

        private List<IBehaviour> behaviourList;
        private IBehaviour currentBehaviour;

        private float delay;
        
        //OnBehaviourStart
        public delegate void BehaviourStart(int animationTypeId, IAnimationParameter animationParameter);
        public event BehaviourStart OnBehaviourStart;

        //OnBehaviourStop
        public delegate void BehaviourStop();
        public event BehaviourStop OnBehaviourStop;

        public AIController()
        {
            Init();
        }

        private void Init()
        {
            behaviourList = new List<IBehaviour>();
            behaviourList.Add(new SwimBehaviour());//TODO (kaiv): remove this
        }

        public void Destroy()
        {

        }

        public void Update()
        {
            if (targetPortfolio != null)
            {
                delay += Time.deltaTime;
                if (delay >= cycleDelay)
                {
                    delay -= cycleDelay;
                    ComputeBehaviour();
                    AnalyzeBehaviour();
                }
            }
        }

        private void AnalyzeBehaviour()
        {
            List<IBehaviour> suitableBehaviourList = new List<IBehaviour>();
            behaviourList.Sort();

            for (int i = behaviourList.Count - 1; ;i--)
            {

            }


            Debug.Log("AnalyzeBehaviour: " + behaviourList[behaviourList.Count - 1]);
        }

        private void ComputeBehaviour()
        {
            foreach (IBehaviour behaviour in behaviourList)
            {
                behaviour.ComputeStatusData(targetPortfolio);
            }
        }

        private void ApplyImpact()
        {

        }

        public Portfolio GetTargetPortfolio()
        {
            return targetPortfolio;
        }

        public void SetTargetPortfolio(Portfolio portfolio)
        {
            targetPortfolio = portfolio;
        }
    }
}
