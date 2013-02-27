#region Copyright
/***************************************************
Author: Keven Poulin
Company: Funcom
Project: FishAdventure (LEGO)
****************************************************/
#endregion

#region Usings
using UnityEngine;
using System.Collections;
using Com.Funcom.FishAdventure.Controller.AI;
using System.Collections.Generic;
using Com.Funcom.FishAdventure.Controller.AI.Behaviour;
#endregion

namespace Com.Funcom.FishAdventure.Component.Entity.Living
{ 
	public abstract class LivingEntity:Entity 
	{
		#region Member Variables

        //Controller
        protected AIController aiController;
        public EnvironementInfo EnvironementInfoRef { get; private set; }
        public FeelingInfo FeelingInfoRef { get; private set; }
		#endregion

        protected override void Awake()
        {
            base.Awake();
            aiController = new AIController();
        }

		protected override void Start () 
		{
            base.Start();
            aiController.registerBehaviourByList(GetBehaviourList());
		}

        protected override void Update() 
		{
            base.Update();
            aiController.Update();
		}

        protected virtual List<IBehaviour> GetBehaviourList()
        {
            return new List<IBehaviour>();
        }
	}
}
