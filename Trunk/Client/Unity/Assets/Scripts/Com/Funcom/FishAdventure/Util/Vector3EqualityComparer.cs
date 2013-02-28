using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class Vector3EqualityComparer : IEqualityComparer<Vector3>
{

    public float precisionMultiplier = 100f;
    static Vector3EqualityComparer INSTANCE;

    public bool Equals(Vector3 a, Vector3 b)
    {
        return AlmostEquals(a, b);
    }

    private bool AlmostEquals(Vector3 a, Vector3 b)
    {
        int ax = (int)(a.x * precisionMultiplier);
        int bx = (int)(b.x * precisionMultiplier);


        int ay = (int)(a.y * precisionMultiplier);
        int by = (int)(b.y * precisionMultiplier);

        int az = (int)(a.z * precisionMultiplier);
        int bz = (int)(b.z * precisionMultiplier);


        return ax.Equals(bx) && ay.Equals(by) && az.Equals(bz);
    }

    public int GetHashCode(Vector3 obj)
    {

        return ((int)(obj.x * precisionMultiplier)).GetHashCode() ^ ((int)(obj.y * precisionMultiplier)).GetHashCode() ^ ((int)(obj.z * precisionMultiplier)).GetHashCode();
    }


    public static Vector3EqualityComparer instance()
    {

        if (INSTANCE == null)
        {
            INSTANCE = new Vector3EqualityComparer();
        }
        return INSTANCE;
    }
}

