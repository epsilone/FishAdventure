using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    //intinterface ILivingEntityListener
interface ILivingEntityListener
	{
        void BehaviourChange(ILivingEntity entity);

        void NeedChanged(ILivingEntity entity);

        void DirectionChanged(ILivingEntity entity);

	}
