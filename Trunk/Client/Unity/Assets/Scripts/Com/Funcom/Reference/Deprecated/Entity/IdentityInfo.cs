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
	public class IdentityInfo 
	{
		#region Member Variables
        public int Id { get; private set; }
        public int EntityTypeId { get; private set; }
        public string Name { get; private set; }
		#endregion

        public IdentityInfo(int id, int entityTypeId, string name)
        {
            SetId(id);
            SetEntityTypeId(entityTypeId);
            SetName(name);
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public void SetEntityTypeId(int entityTypeId)
        {
            EntityTypeId = entityTypeId;
        }

        public void SetName(string name)
        {
            Name = name;
        }
	}
}
