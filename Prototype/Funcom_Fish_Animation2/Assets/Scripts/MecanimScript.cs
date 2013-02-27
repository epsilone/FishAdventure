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
#endregion

namespace MecanimScript
{ 
	public class MecanimScript:MonoBehaviour 
	{
		#region Member Variables
		
		#endregion

		void Start () 
		{
			Animator animator = gameObject.AddComponent<Animator>();
			animator.speed = 2;
		}
		
		void Update () 
		{
		
		}
	}
}
