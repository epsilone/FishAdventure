internal class Fish : BaseLivingEntity
{
    private Aura aura = null;

    public override Aura getAura()
    {
        if (aura == null)
        {
            aura = new Aura(1, true);
        }
        return aura;
    }

    public override EntityType GetEntityType()
    {
        return EntityType.FISH;
    }
}