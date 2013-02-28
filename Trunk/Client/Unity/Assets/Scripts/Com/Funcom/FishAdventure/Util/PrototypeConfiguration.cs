using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	class AIPrototypeConfiguration
	{

        static AIPrototypeConfiguration INSTANCE;

        enum CollisionStrategy { 
            // no collision detection at all
            NONE,
            // ai manager makes sure that every fish is in its own path
            AVOID,
            // every fish is in its own layer
            SINGLE_LAYER,
            // when a collision occurs the fish will change his layer temp
            OFFSET_LAYER,
            // when a collision occurs we ask the AIController to generate a new path
            PATH_REGEN
        }

       private CollisionStrategy strategy;

        public static  AIPrototypeConfiguration instance() {
            if (INSTANCE == null ){
                INSTANCE = new AIPrototypeConfiguration();
            }
            return INSTANCE;
        }

        public Boolean isCollisionDetectionEnabled() {
            return !CollisionStrategy.NONE.Equals(strategy) && !CollisionStrategy.SINGLE_LAYER.Equals(strategy);
        }



	}
