using UnityEngine;

public class World
{
    private static float segmentLength = 10;
    private static float MinX = -segmentLength;
    private static float MinY = -segmentLength;
    private static float MinZ = -segmentLength;

    private static float MaxX = segmentLength;
    private static float MaxY = segmentLength;
    private static float MaxZ = segmentLength;

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