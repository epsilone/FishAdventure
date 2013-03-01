using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

interface IWorld
{
    float GetMinX();
    float GetMinY();
    float GetMinZ();

    float GetMaxX();
    float GetMaxY();
    float GetMaxZ();

    bool IsWithinBounds(Vector3 point);

}
