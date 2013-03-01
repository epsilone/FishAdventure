using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IEntity
{
    int GetId();

    EntityType GetEntityType();

    String GetName();
}
