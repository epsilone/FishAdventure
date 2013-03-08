using System;
using System.Collections.Generic;

internal class FishTankFactory
{
    private static FishTankFactory INSTANCE;

    public static FishTankFactory getInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new FishTankFactory();
        }
        return INSTANCE;
    }

    public List<IBehaviour> createSupportedBehaviours(BaseLivingEntity entity)
    {
        List<IBehaviour> returnValue = new List<IBehaviour>();
        List<BehaviourRegistry.BehaviourInfo> behaviours = new List<BehaviourRegistry.BehaviourInfo>(BehaviourRegistry.getInstance().getDefinedBehavioursForEntityType(entity.GetEntityType()));
        foreach (BehaviourRegistry.BehaviourInfo b in behaviours)
        {
            BaseBehaviour behaviour;
            switch (b.Type)
            {
                case BehaviourType.IDLE:
                    behaviour = new IdleBehaviour(b.Type, b.Weight);
                    break;

                case BehaviourType.SWIM:
                    behaviour = new SwinBehaviour(b.Type, b.Weight);
                    break;

                case BehaviourType.CHASE:
                    behaviour = new ChaseBehaviour(b.Type, b.Weight, b.TriggerEntities);
                    break;

                case BehaviourType.CIRCLE:
                    behaviour = new CircleBehaviour(b.Type, b.Weight, b.TriggerEntities);
                    break;

                case BehaviourType.FLEE:
                    behaviour = new FleeBehaviour(b.Type, b.Weight, b.TriggerEntities);
                    break;

                case BehaviourType.FOLLOW:
                    behaviour = new FollowBehaviour(b.Type, b.Weight, b.TriggerEntities);
                    break;

                default:
                    throw new System.InvalidOperationException();
            }

            behaviour.LivingEntity = entity;
            returnValue.Add(behaviour);
        }
        return returnValue;
    }

    private List<ItemInfo> getSupportedItems(BaseLivingEntity entity)
    {
        return ItemsRegistry.getInstance().getSupportedItemsForType(entity.GetEntityType());
    }

    // TODO : use for testing item integration
    public IItem createRandomSupportedItem(BaseLivingEntity entity)
    {
        List<ItemInfo> supported = getSupportedItems(entity);
        Random r = new Random();
        int randomOffSet = r.Next(0, supported.Count);

        // retrun item not item info, not implemented at the moment
        ItemInfo randomSupportedInfo = supported[randomOffSet];
        return createItem(randomSupportedInfo);
    }

    private IItem createItem(ItemInfo itemInfo)
    {
        throw new NotImplementedException();
    }

    public List<Need> createSupportedNeeds(BaseLivingEntity entity)
    {
        List<Need> returnValue = new List<Need>();
        List<NeedRegistry.NeedInfo> needsInfo = NeedRegistry.getInstance().getSupportedNeedsForType(entity.GetEntityType());
        foreach (NeedRegistry.NeedInfo info in needsInfo)
        {
            Need n = new Need(info.Type, info.Weight);
            returnValue.Add(n);
        }
        return returnValue;
    }
}