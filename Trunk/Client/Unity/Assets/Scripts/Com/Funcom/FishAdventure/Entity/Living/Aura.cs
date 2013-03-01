using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Aura
{
    float radius;

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }
    Boolean isActive;

    public Boolean IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    public Aura(float radius, Boolean isActive)
    {
        this.radius = radius;
        this.isActive = isActive;
    }
}
