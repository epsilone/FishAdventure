using UnityEngine;
using System;
using System.Collections.Generic;
using Paddle;

public class LocalPlayer : IPaddlePlayer 
{
    public string Name { get; set; }
    public int Points { get; set; }
}
