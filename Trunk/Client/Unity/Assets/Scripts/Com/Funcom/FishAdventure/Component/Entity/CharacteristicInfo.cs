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
    public class CharacteristicInfo
    {
        #region Member Variables
        public int Speed { get; private set; }
        public int Courage { get; private set; }
        public int Greedy { get; private set; }
        public int Social { get; private set; }
        public int Beauty { get; private set; }
        #endregion

        public CharacteristicInfo()
        {

        }

        public void applyCharacteristic(CharacteristicInfo characteristicInfo)
        {
            SetSpeed(Speed + characteristicInfo.Speed);
            SetSpeed(Courage + characteristicInfo.Courage);
            SetSpeed(Greedy + characteristicInfo.Greedy);
            SetSpeed(Social + characteristicInfo.Social);
            SetSpeed(Beauty + characteristicInfo.Beauty);
        }

        //SETTER
        public void SetSpeed(int speed)
        {
            Speed = speed;
        }

        public void SetCourage(int courage)
        {
            Courage = courage;
        }

        public void SetGreedy(int greedy)
        {
            Greedy = greedy;
        }

        public void SetSocial(int social)
        {
            Social = social;
        }

        public void SetBeauty(int beauty)
        {
            Beauty = beauty;
        }
    }
}
