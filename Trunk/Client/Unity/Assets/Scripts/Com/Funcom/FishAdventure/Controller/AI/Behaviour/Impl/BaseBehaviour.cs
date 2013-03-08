internal abstract class BaseBehaviour : IBehaviour
{
    private float animationSpeed;

    public float AnimationSpeed
    {
        get { return animationSpeed; }
        set { animationSpeed = value; }
    }

    private BaseLivingEntity livingEntity;

    public BaseLivingEntity LivingEntity
    {
        get { return livingEntity; }
        set { livingEntity = value; }
    }

    protected BaseBehaviour(BehaviourType type, int weight)
    {
        this.type = type;
        this.weight = weight;
    }

    private BehaviourType type;

    public BehaviourType Type
    {
        get { return type; }
        set { type = value; }
    }

    private int weight;

    public int Weight
    {
        get { return weight; }
        set { weight = value; }
    }

    public BehaviourType GetBehaviourType()
    {
        return type;
    }

    public int GetWeight()
    {
        return weight;
    }

    public abstract void Start();

    public abstract void Update();

    public abstract void Stop();

    public virtual void OnDrawGizmos()
    {
    }

    public virtual void GenericTweenUpdate(object args)
    {
    }


    public virtual void LateUpdate()
    {

    }
}