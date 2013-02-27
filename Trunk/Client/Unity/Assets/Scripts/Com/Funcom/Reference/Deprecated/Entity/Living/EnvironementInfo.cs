#region Copyright
/***************************************************
Author: Keven Poulin
Company: Funcom
Project: FishAdventure (LEGO)
****************************************************/
#endregion

#region Usings

#endregion

namespace Com.Funcom.FishAdventure.Component.Entity.Living
{
    public class EnvironementInfo
    {
        #region Member Variables

        public bool IsRequestAttention { get; private set; }
        public bool IsShaking { get; private set; }
        public bool IsWindy { get; private set; }
        public bool NearbyEntityList { get; private set; }
        #endregion

        public EnvironementInfo()
        {

        }

        //SETTER
        public void SetRequestAttention(bool requestAttention)
        {
            IsRequestAttention = requestAttention;
        }

        public void SetShaking(bool shaking)
        {
            IsShaking = shaking;
        }

        public void SetWindy(bool windy)
        {
            IsWindy = windy;
        }

        public void SetNearbyEntityList(bool nearbyEntityList)
        {
            NearbyEntityList = nearbyEntityList;
        }
    }
}
