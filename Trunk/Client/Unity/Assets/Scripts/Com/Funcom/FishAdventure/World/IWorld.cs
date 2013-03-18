using UnityEngine;

internal interface IWorld
{
    float GetMinX();

    float GetMinY();

    float GetMinZ();

    float GetMaxX();

    float GetMaxY();

    float GetMaxZ();

    bool IsWithinBounds(Vector3 point);
}