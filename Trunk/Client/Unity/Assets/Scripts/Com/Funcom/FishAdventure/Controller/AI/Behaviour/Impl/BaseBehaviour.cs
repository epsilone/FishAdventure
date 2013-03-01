using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

abstract class BaseBehaviour : IBehaviour
{
    BaseLivingEntity livingEntity;

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
    public virtual void TweenUpdate()
    {
    }

    public virtual void OnDrawGizmos()
    {
    }

}
