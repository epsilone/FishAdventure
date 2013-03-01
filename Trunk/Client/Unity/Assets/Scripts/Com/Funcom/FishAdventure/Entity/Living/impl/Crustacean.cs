using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Crustacean : BaseLivingEntity
{
    private Aura aura = null;

    public override EntityType GetEntityType()
    {
        return EntityType.CRUSTACEAN;
    }

    public override Aura getAura()
    {
        if (aura == null) {
            aura = new Aura(2, true);
        }
        return aura;
    }
}

