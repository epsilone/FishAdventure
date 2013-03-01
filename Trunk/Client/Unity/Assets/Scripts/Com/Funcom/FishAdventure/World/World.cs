using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World
{
    static float segmentLength = 25;
    static float MinX = -segmentLength;
    static float MinY = -segmentLength;
    static float MinZ = -segmentLength;

    static float MaxX = segmentLength;
    static float MaxY = segmentLength;
    static float MaxZ = segmentLength;


    public static float GetMinX()
    {
        return MinX;
    }

    public static float GetMinY()
    {
        return MinY;
    }

    public static float GetMinZ()
    {
        return MinZ;
    }

    public static float GetMaxX()
    {
        return MaxX;
    }

    public static float GetMaxY()
    {
        return MaxY;
    }

    public static float GetMaxZ()
    {
        return MaxZ;
    }


    public void Start()
    {

    }

    public static bool IsWithinBounds(Vector3 point)
    {
        if (point.x > GetMaxX() || point.x < GetMinX() || point.y > GetMaxY() || point.y < GetMinY() || point.z > GetMaxZ() || point.z < GetMinZ())
        {
            return false;
        }
        return true;
    }
}
