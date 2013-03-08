using System.Collections.Generic;

internal interface ISocialBehaviour : IBehaviour
{
    List<EntityType> getTriggeringTypes();

    void SetTargetEntity(IEntity entity);
}