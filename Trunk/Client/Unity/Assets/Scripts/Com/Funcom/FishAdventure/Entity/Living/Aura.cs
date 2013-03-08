using System;

public class Aura
{
    private Boolean isActive;
    private float radius;

    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public Aura(float radius, Boolean isActive)
    {
        this.radius = radius;
        this.isActive = isActive;
    }

    public Boolean IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }
}