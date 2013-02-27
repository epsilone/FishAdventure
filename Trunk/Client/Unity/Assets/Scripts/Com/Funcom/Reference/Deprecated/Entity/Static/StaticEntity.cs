#region Copyright
/***************************************************
Author: Keven Poulin
Company: Funcom
Project: FishAdventure (LEGO)
****************************************************/
#endregion

#region Usings


#endregion

namespace Com.Funcom.FishAdventure.Component.Entity.Static
{ 
	public abstract class StaticEntity:Entity 
	{
		#region Member Variables

		#endregion

        protected override void Awake()
        {
            base.Awake();
        }

		protected override void Start () 
		{
            base.Start();
		}

        protected override void Update() 
		{
            base.Update();
		}
	}
}
