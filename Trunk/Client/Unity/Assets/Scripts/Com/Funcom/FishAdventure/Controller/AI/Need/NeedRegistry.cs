using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class NeedRegistry
{
    static Dictionary<EntityType, List<NeedInfo>> needsDictionary;

    static NeedRegistry INSTANCE;

    static NeedRegistry()
    {
        needsDictionary = new Dictionary<EntityType, List<NeedInfo>>();

        List<NeedInfo> curList = new List<NeedInfo>();
        // JELLY FISH
        EntityType curType = EntityType.JELLYFISH;
        curList.Add(new NeedInfo(NeedType.EAT, 1));
        curList.Add(new NeedInfo(NeedType.BUILD, 1));
        curList.Add(new NeedInfo(NeedType.INTERACT, 3));
        curList.Add(new NeedInfo(NeedType.NONE, 1));


        needsDictionary.Add(curType, curList);

        // FISH

        curType = EntityType.FISH;
        curList = new List<NeedInfo>();
        curList.Add(new NeedInfo(NeedType.EAT, 1));
        curList.Add(new NeedInfo(NeedType.BUILD, 1));
        curList.Add(new NeedInfo(NeedType.INTERACT, 2));
        curList.Add(new NeedInfo(NeedType.NONE, 1));

        needsDictionary.Add(curType, curList);

        // JELLY FISH
        curType = EntityType.CRUSTACEAN;
        curList = new List<NeedInfo>();
        curList.Add(new NeedInfo(NeedType.EAT, 1));
        curList.Add(new NeedInfo(NeedType.BUILD, 4));
        curList.Add(new NeedInfo(NeedType.INTERACT, 2));
        curList.Add(new NeedInfo(NeedType.NONE, 1));

        needsDictionary.Add(curType, curList);

    }

    public List<NeedInfo> getSupportedNeedsForType(EntityType type)
    {
        List<NeedInfo> needs;
        if (needsDictionary.TryGetValue(type, out needs))
        {
            return needs;
        }
        return null;
    }

    public static NeedRegistry getInstance()
    {
        if (INSTANCE == null)
        {
            INSTANCE = new NeedRegistry();
        }
        return INSTANCE;
    }


    public class NeedInfo
    {
        public NeedInfo(NeedType type, int weight)
        {
            this.type = type;
            this.weight = weight;
        }

        private NeedType type;

        public NeedType Type
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
    }
}
