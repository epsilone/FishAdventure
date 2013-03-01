using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;

class AIController
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(AIController));
    // changes of value at runtime are not reflected, we need to restart for changes to take effect
    public static float NEED_REPEAT_RATE = 2f;
    public static float BEHAVIOUR_REPEAT_RATE = 1f;
    private System.Random random;

    private BaseLivingEntity controlledEntity;
    private Dictionary<EntityType, List<IEntity>> neighboursByType;

    private static System.Object mutex = new System.Object();

    public AIController(BaseLivingEntity controlledEntity)
    {
        this.controlledEntity = controlledEntity;
        neighboursByType = new Dictionary<EntityType, List<IEntity>>();
        random = new System.Random(controlledEntity.GetHashCode());
    }

    public void Start()
    {
        this.controlledEntity.InvokeRepeating("generateNeed", 0, NEED_REPEAT_RATE);
        this.controlledEntity.InvokeRepeating("generateBehaviour", 0, BEHAVIOUR_REPEAT_RATE);
    }

    public void GenerateNeed()
    {
        Need newNeed = GetRandomNeed();
        controlledEntity.SetCurrentNeed(newNeed);
    }

    public void GenerateBehaviour()
    {
        IBehaviour newBehaviour = GetRandomBehaviour();
        bool isContinuingPrevBehaviour = false;
        if (controlledEntity.CurrentBehaviour != null)
        {
            if (controlledEntity.CurrentBehaviour.Equals(newBehaviour))
            {
                isContinuingPrevBehaviour = true;
            }
            else
            {
                controlledEntity.CurrentBehaviour.Stop();
            }
        }
        controlledEntity.SetCurrentBehaviour(newBehaviour);
        if (!isContinuingPrevBehaviour)
        {

            controlledEntity.CurrentBehaviour.Start();
        }
    }




    Need GetRandomNeed()
    {
        // recalculting the weight, find a better way !
        int intentionsWeight = 0;
        foreach (Need def in controlledEntity.SupportedNeeds)
        {
            intentionsWeight += def.GetWeight();
        }

        int randomOffset = random.Next(0, intentionsWeight);
        int curWeight = 0;
        for (int i = 0; i < controlledEntity.SupportedNeeds.Count; i++)
        {

            if ((curWeight + controlledEntity.SupportedNeeds[i].GetWeight()) >= randomOffset)
            {
                return controlledEntity.SupportedNeeds[i];
            }
            curWeight += controlledEntity.SupportedNeeds[i].GetWeight();
        }
        // should never get here !
        throw new System.InvalidOperationException();
    }



    IBehaviour GetRandomBehaviour()
    {
        lock (mutex)
        {
    
            //     Debug.Log("Generating random behaviour");
            IBehaviour returnValue = null;
            int behaviourWeight = 0;
            List<IBehaviour> nonTriggered = new List<IBehaviour>();
            foreach (IBehaviour def in controlledEntity.SupportedBehaviours)
            {
                if (!(def is ISocialBehaviour))
                {

                    behaviourWeight += def.GetWeight();
                }
                else
                {
                    ISocialBehaviour socialBehaviour = (ISocialBehaviour)def;
                    bool triggered = false;
                    foreach (EntityType type in socialBehaviour.getTriggeringTypes())
                    {
                        if (neighboursByType.Keys.Contains(type))
                        {
                            if (neighboursByType[type].Count > 0)
                            {

                                triggered = true;
                            }
                        }
                    }
                    if (triggered)
                    {
                        behaviourWeight += def.GetWeight();
                    }
                    else
                    {
                        nonTriggered.Add(def);
                    }
                }

            }
            // double the weight of the current behaviour
            if (controlledEntity.CurrentBehaviour != null && !nonTriggered.Contains(controlledEntity.CurrentBehaviour))
            {
                behaviourWeight += controlledEntity.CurrentBehaviour.GetWeight();
            }


            int randomOffset = random.Next(0, behaviourWeight);

            int curWeight = 0;
            for (int i = 0; i < controlledEntity.SupportedBehaviours.Count; i++)
            {
                if (!nonTriggered.Contains(controlledEntity.SupportedBehaviours[i]))
                {

                    int elWeight = controlledEntity.SupportedBehaviours[i].GetWeight();
                    if (controlledEntity.CurrentBehaviour != null && !nonTriggered.Contains(controlledEntity.CurrentBehaviour) && controlledEntity.CurrentBehaviour.Equals(controlledEntity.SupportedBehaviours[i]))
                    {
                        elWeight += controlledEntity.CurrentBehaviour.GetWeight();
                    }
                    if ((curWeight + elWeight) >= randomOffset)
                    {
                       
                        returnValue = controlledEntity.SupportedBehaviours[i];
                        break;
                    }
                    curWeight += elWeight;
                }
                if (logger.IsExtreme())
                {
                    logger.Log("Entity " + controlledEntity.GetId() + " Random offset " + randomOffset + " full weight " + behaviourWeight);
                }
                

            }
       
            if (returnValue != null)
            {

                if (returnValue is ISocialBehaviour)
                {
                    // generate a target for the social behaviour
                    ISocialBehaviour socialReturnVal = (ISocialBehaviour)returnValue;
                    List<IEntity> potentialTriggerEntities = new List<IEntity>();
                    List<EntityType> triggeringTypes = ((ISocialBehaviour)returnValue).getTriggeringTypes();
                    foreach (EntityType type in triggeringTypes)
                    {
                        if (neighboursByType.ContainsKey(type))
                        {
                            potentialTriggerEntities.AddRange(neighboursByType[type]);
                        }
                    }

                    //      Debug.Log("potential triggers " + potentialTriggerEntities.Count);
                    int randomTarget = random.Next(0, potentialTriggerEntities.Count);
                    IEntity triggerEntity = potentialTriggerEntities[randomTarget];
                    socialReturnVal.SetTargetEntity(triggerEntity);
                }
                return returnValue;
            }
            else
            {
                // should never get here !
                throw new System.InvalidOperationException();
            }
        }
    }

    internal void AddNeighbour(IEntity gameObject)
    {
        lock (mutex)
        {
            if (logger.IsExtreme())
            {
                logger.Log("New Neighbour" + gameObject);
            }
            IEntity entity = (IEntity)gameObject;
            List<IEntity> neighbouringEntities;
            if (neighboursByType.ContainsKey(entity.GetEntityType()))
            {
                if (!neighboursByType.TryGetValue(entity.GetEntityType(), out neighbouringEntities))
                {
                    neighbouringEntities = new List<IEntity>();
                }
            }
            else
            {
                neighbouringEntities = new List<IEntity>();
            };
            neighbouringEntities.Add(entity);
            neighboursByType[entity.GetEntityType()] = neighbouringEntities;

        }

    }

    internal void RemoveNeighbour(IEntity gameObject)
    {
        lock (mutex)
        {
            if (logger.IsExtreme())
            {
                logger.Log("Remove Neighbour" + gameObject);
            }
            IEntity entity = (IEntity)gameObject;
            List<IEntity> neighbouringEntities;
            if (neighboursByType.ContainsKey(entity.GetEntityType()))
            {
                if (!neighboursByType.TryGetValue(entity.GetEntityType(), out neighbouringEntities))
                {
                    neighbouringEntities = new List<IEntity>();
                }
            }
            else
            {
                neighbouringEntities = new List<IEntity>();
            };
            neighbouringEntities.Remove(entity);
            neighboursByType[entity.GetEntityType()] = neighbouringEntities;
        }

    }



}
