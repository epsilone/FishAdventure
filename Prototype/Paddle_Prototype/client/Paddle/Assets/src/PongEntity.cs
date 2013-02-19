using UnityEngine;
using System;
using System.Collections.Generic;

public class PongEntity : MonoBehaviour 
{
    public bool isStatic;
    public float speed = 1.0f;
    public Vector3 normal;

    public Vector3 Displacement { get; set; }

	void Start() 
    {
        if (!isStatic)
        {
            Displacement = new Vector3(1.0f, 1.0f, 0.0f).normalized;
            //Displacement = Vector3.right;   // Make sure it is normalized
        }

        var go = GameObject.Find("GameScene");
        var resolver = go.GetComponent<PongGame>();
        resolver.Register(this);
	}
}
