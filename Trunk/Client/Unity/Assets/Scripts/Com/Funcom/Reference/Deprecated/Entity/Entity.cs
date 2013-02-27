#region Copyright
/***************************************************
Author: Keven Poulin
Company: Funcom
Project: FishAdventure (LEGO)
****************************************************/
#endregion

#region Usings
using Com.Funcom.FishAdventure.Controller.AI;
using UnityEngine;

#endregion

namespace Com.Funcom.FishAdventure.Component.Entity
{
    public abstract class Entity:MonoBehaviour
    {
        #region Member Variables
        public IdentityInfo IdentityInfoRef { get; private set; }
        public CharacteristicInfo CharacteristicInfoRef { get; private set; }
        protected Aura AuraRef;
        #endregion

        protected virtual void Awake()
        {
            AuraRef = new Aura(gameObject);
            AuraRef.SetRadius(8.0f);//TODO (Kaiv) - Temp stuff, this should be evaluate base on fish!
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            
        }

        public void SetIdentityInfo(IdentityInfo identityInfo)
        {
            IdentityInfoRef = identityInfo;
        }

        public void SetCharacteristicInfo(CharacteristicInfo characteristicInfo)
        {
            CharacteristicInfoRef = characteristicInfo;
        }
    }
}
