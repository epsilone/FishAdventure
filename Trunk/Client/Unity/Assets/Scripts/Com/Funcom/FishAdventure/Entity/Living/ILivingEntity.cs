using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ILivingEntity : IEntity 
{
    IBehaviour GetCurrentBehaviour();

    INeed GetCurrentNeed();
}
