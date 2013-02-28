using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	class ItemsRegistry
	{


        static Dictionary<EntityType, List<ItemInfo>> itemsDictionary;
        static ItemsRegistry INSTANCE;


        static ItemsRegistry() {
            itemsDictionary = new Dictionary<EntityType, List<ItemInfo>>();
            List<ItemInfo> curList = new List<ItemInfo>();
            // JELLY FISH
            EntityType curType = EntityType.JELLYFISH;
            curList.Add(new ItemInfo(ItemType.ANGRY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.HAPPY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.PIRATE_EYES, 5));
            curList.Add(new ItemInfo(ItemType.ANGRY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.SHY_EYE, 5));
            

            itemsDictionary.Add(curType, curList);

            // FISH

            curType = EntityType.FISH;
            curList = new List<ItemInfo>();
            curList.Add(new ItemInfo(ItemType.ANGRY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.HAPPY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.PIRATE_EYES, 5));
            curList.Add(new ItemInfo(ItemType.ANGRY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.SHY_EYE, 5));
            itemsDictionary.Add(curType, curList);

            // JELLY FISH
            curType = EntityType.CRUSTACEAN;
            curList = new List<ItemInfo>();
            curList.Add(new ItemInfo(ItemType.ANGRY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.HAPPY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.PIRATE_EYES, 5));
            curList.Add(new ItemInfo(ItemType.ANGRY_EYES, 5));
            curList.Add(new ItemInfo(ItemType.SHY_EYE, 5));
            itemsDictionary.Add(curType, curList);

            curType = EntityType.BUILDING;
            curList = new List<ItemInfo>();
            curList.Add(new ItemInfo(ItemType.CASTLE_FLAG, 5));

            itemsDictionary.Add(curType, curList);
        }

        public static ItemsRegistry getInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new ItemsRegistry();
            }
            return INSTANCE;
        }

     

        public List<ItemInfo> getSupportedItemsForType(EntityType type)
        {
            List<ItemInfo> items; 
            if (itemsDictionary.TryGetValue(type, out items)) {
                return items;
            }
            return null;
        }
	}
