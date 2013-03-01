using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BehaviourRegistry
{
    static EntityType[] fishTypes = new EntityType[] { EntityType.CRUSTACEAN, EntityType.FISH, EntityType.JELLYFISH };
    static EntityType[] allTypes = Enum.GetValues(typeof(EntityType)).Cast<EntityType>().ToArray();
    static EntityType[] buildingTypes = new EntityType[] { EntityType.BUILDING };

    static Dictionary<EntityType, List<BehaviourInfo>> behaviourDictionary;

    static BehaviourRegistry INSTANCE;

    static BehaviourRegistry()
    {

        behaviourDictionary = new Dictionary<EntityType, List<BehaviourInfo>>();
        // For now we're using generic Behviour classes, could be specialized later
        List<BehaviourInfo> curList = new List<BehaviourInfo>();
        List<EntityType> triggerEntities = new List<EntityType>();
        // JELLY FISH

        EntityType curType = EntityType.JELLYFISH;

          curList.Add(new BehaviourInfo(BehaviourType.FOLLOW, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.CIRCLE, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.FLEE, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.CHASE, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.SWIM, 4, null));
        curList.Add(new BehaviourInfo(BehaviourType.IDLE, 2, null));
        behaviourDictionary.Add(curType, curList);

        // FISH

        curType = EntityType.FISH;
        curList = new List<BehaviourInfo>();
        curList.Add(new BehaviourInfo(BehaviourType.SWIM, 3, null));
        curList.Add(new BehaviourInfo(BehaviourType.IDLE, 2, null));
        curList.Add(new BehaviourInfo(BehaviourType.FOLLOW, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.CIRCLE, 2, buildingTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.CHASE, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.FLEE, 1, fishTypes.ToList()));
        behaviourDictionary.Add(curType, curList);

        // JELLY FISH
        curType = EntityType.CRUSTACEAN;
        curList = new List<BehaviourInfo>();
        curList.Add(new BehaviourInfo(BehaviourType.SWIM, 1, null));
        curList.Add(new BehaviourInfo(BehaviourType.IDLE, 1, null));
        curList.Add(new BehaviourInfo(BehaviourType.FOLLOW, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.CIRCLE, 1, buildingTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.CHASE, 1, fishTypes.ToList()));
        curList.Add(new BehaviourInfo(BehaviourType.FLEE, 1, fishTypes.ToList()));
        behaviourDictionary.Add(curType, curList);

        curType = EntityType.BUILDING;
        curList = new List<BehaviourInfo>();
        curList.Add(new BehaviourInfo(BehaviourType.IDLE, 1, null));
        behaviourDictionary.Add(curType, curList);

    }

    public static BehaviourRegistry getInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new BehaviourRegistry();
        }
        return INSTANCE;
    }


    public List<BehaviourInfo> getDefinedBehavioursForEntityType(EntityType type)
    {
        List<BehaviourInfo> items;
        if (behaviourDictionary.TryGetValue(type, out items))
        {
            return items;
        }
        return null;
    }




    public static IEnumerable<EntityType> types { get; set; }


    public class BehaviourInfo
    {
        private BehaviourType type;

        internal BehaviourType Type
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
        private List<EntityType> triggerEntities;

        public List<EntityType> TriggerEntities
        {
            get { return triggerEntities; }
            set { triggerEntities = value; }
        }
        public BehaviourInfo(BehaviourType type, int weight, List<EntityType> triggerEntities)
        {
            this.triggerEntities = triggerEntities;
            this.type = type;
            this.weight = weight;
        }

    }
}
