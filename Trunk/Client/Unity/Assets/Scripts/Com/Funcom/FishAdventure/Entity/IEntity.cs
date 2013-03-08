using System;

public interface IEntity
{
    int GetId();

    EntityType GetEntityType();

    String GetName();
}