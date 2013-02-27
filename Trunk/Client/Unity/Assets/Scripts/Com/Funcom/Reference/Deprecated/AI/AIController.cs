using System.Collections.Generic;
using Com.Funcom.FishAdventure.Controller.AI.Behaviour;
using UnityEngine;

namespace Com.Funcom.FishAdventure.Controller.AI
{
    public class AIController
    {
        //Configuration
        private int suitableBehaviourCount = 3;
        private float cycleDelay = 1; //sec

        //private Portfolio targetPortfolio;
        

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
        }

        public void Destroy()
        {

        }

        public void registerBehaviourByList(List<IBehaviour> behaviourList)
        {
            foreach (IBehaviour behaviourBuffer in behaviourList)
            {
                registerBehaviour(behaviourBuffer);
            }
        }

        public void registerBehaviour(IBehaviour behaviour)
        {
            foreach (IBehaviour behaviourBuffer in behaviourList)
            {
                if (behaviourBuffer.GetType() == behaviour.GetType())
                {
                    Debug.Log("AIController.cs - registerBehaviour() - Behaviour " + behaviour + " is already registered.");
                    return;
                }
            }

            behaviourList.Add(behaviour);
        }

        public void Update()
        {
            //if (targetPortfolio != null)
            //{
                delay += Time.deltaTime;
                if (delay >= cycleDelay)
                {
                    delay -= cycleDelay;
                    ComputeBehaviour();
                    AnalyzeBehaviour();
                }
            //}
        }

        private void AnalyzeBehaviour()
        {
            List<IBehaviour> suitableBehaviourList = new List<IBehaviour>();
            behaviourList.Sort();

            /*for (int i = behaviourList.Count - 1; ;i--)
            {

            }*/


            Debug.Log("AnalyzeBehaviour: " + behaviourList[behaviourList.Count - 1]);
        }

        private void ComputeBehaviour()
        {
            foreach (IBehaviour behaviour in behaviourList)
            {
                //behaviour.ComputeStatusData(targetPortfolio);
            }
        }

        private void ApplyImpact()
        {

        }

        /*public Portfolio GetTargetPortfolio()
        {
            return targetPortfolio;
        }

        public void SetTargetPortfolio(Portfolio portfolio)
        {
            targetPortfolio = portfolio;
        }*/
    }
}
