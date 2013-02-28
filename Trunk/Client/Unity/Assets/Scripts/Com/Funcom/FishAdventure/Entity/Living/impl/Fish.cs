using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	class Fish : BaseLivingEntity
	{
        Aura aura = null;

        public override EntityType GetEntityType()
        {
            return EntityType.FISH;
        }

        public override Aura getAura()
        {
            if (aura == null) {
                aura = new Aura(1, true);
            }
            return aura;
        }
    }
