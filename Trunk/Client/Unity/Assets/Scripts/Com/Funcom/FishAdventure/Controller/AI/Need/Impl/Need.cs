internal class Need : INeed
{
    public Need(NeedType type, int weight)
    {
        this.type = type;
        this.weight = weight;
    }

    private NeedType type;

    public NeedType Type
    {
        set { type = value; }
    }

    private int weight;

    public int Weight
    {
        set { weight = value; }
    }

    public NeedType GetNeedType()
    {
        return type;
    }

    public int GetWeight()
    {
        return weight;
    }
}