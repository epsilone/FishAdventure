using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class JellyFish : BaseLivingEntity
{
    private Aura aura;

    public override EntityType GetEntityType()
    {
        return EntityType.JELLYFISH;
    }

    public override Aura getAura()
    {
        if (aura == null)
        {
            aura = new Aura(5, true);
        }
        return aura;
    }
}
