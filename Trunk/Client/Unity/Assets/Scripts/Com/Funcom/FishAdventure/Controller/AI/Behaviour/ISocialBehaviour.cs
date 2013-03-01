using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

interface ISocialBehaviour : IBehaviour
{
     List<EntityType> getTriggeringTypes();

     void SetTargetEntity(IEntity entity);
}
