using UnityEngine;
using System.Collections;

public static class Extensions
{
    public static bool Between(this int value, int left, int right)
    {
        return value > left && value < right;
    }
}
