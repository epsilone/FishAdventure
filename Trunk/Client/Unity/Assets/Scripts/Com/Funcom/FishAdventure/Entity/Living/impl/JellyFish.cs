internal class JellyFish : BaseLivingEntity
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
            aura = new Aura(20, true);
        }
        return aura;
    }
}