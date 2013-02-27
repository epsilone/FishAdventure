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
    public class FeelingInfo
    {
        #region Member Variables

        public bool Appetite { get; private set; }
        public bool Energy { get; private set; }
        public bool Heatlth { get; private set; }
        public bool SelfConfidence { get; private set; }
        #endregion

        public FeelingInfo()
        {

        }

        //SETTER
        public void SetAppetite(bool appetite)
        {
            Appetite = appetite;
        }

        public void SetEnergy(bool energy)
        {
            Energy = energy;
        }

        public void SetHeatlth(bool heatlth)
        {
            Heatlth = heatlth;
        }

        public void SetSelfConfidence(bool selfConfidence)
        {
            SelfConfidence = selfConfidence;
        }
    }
}
