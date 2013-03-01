using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IBehaviour
{
    int GetWeight();

    BehaviourType GetBehaviourType();
    
    void Start();

    void TweenUpdate();

    void Update();
    
    void OnDrawGizmos();

    void Stop();   
}
