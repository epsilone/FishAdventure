#region Copyright
/***************************************************
Author: Keven Poulin
Company: Funcom
Project: FishAdventure (LEGO)
****************************************************/
#endregion

#region Usings
using System.Collections;
using UnityEngine;
#endregion

namespace Com.Funcom.FishAdventure.Component.Entity
{ 
	public class Aura 
	{
		#region Member Variables
        public float Radius { get; private set; }
        private GameObject target;
        private SphereCollider collider;
        private bool isActive;
		#endregion

        public Aura(GameObject target)
        {
            this.target = target;

            Init();
        }

        private void Init()
        {
            collider = target.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = Radius;
        }

        public void SetRadius(float radius)
        {
            Radius = radius;
            if (collider != null)
            {
                collider.radius = radius;
            }
        }
	}
}
