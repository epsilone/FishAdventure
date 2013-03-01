using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    float followDistance = 200;

    void Update()
    {
        iTween.MoveUpdate(gameObject, new Vector3(target.position.x, 0, -followDistance), .8f);
    }
}


