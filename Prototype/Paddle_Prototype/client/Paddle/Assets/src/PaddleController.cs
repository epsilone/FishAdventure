using UnityEngine;
using System.Collections.Generic;

public class PaddleController : MonoBehaviour 
{
    private const float PADDLE_SPEED = 25.0f;
    public Transform controlled;
    private Transform topBorder;
    private Transform bottomBorder;
    private Vector3 max;
    private Vector3 min;
    private float top;
    private float bottom;

    // Use this for initialization
    void Start() 
    {
        topBorder = GameObject.Find("TopBorder").transform;
        bottomBorder = GameObject.Find("BottomBorder").transform;
        top = topBorder.localPosition.y - topBorder.localScale.y / 2.0f;
        bottom = bottomBorder.localPosition.y + bottomBorder.localScale.y / 2.0f;
        max = new Vector3(controlled.localPosition.x, top - controlled.localScale.y / 2.0f, 0.0f);
        min = new Vector3(controlled.localPosition.x, bottom + controlled.localScale.y / 2.0f, 0.0f);
    }
    
    // Update is called once per frame
    void Update() 
    {
        float vertical = Input.GetAxis("Vertical");
        controlled.Translate(transform.up * vertical * Time.deltaTime * PADDLE_SPEED);
        if (controlled.localPosition.y + controlled.localScale.y / 2.0f > top)
        {
            controlled.localPosition = max;
        }
        else if (controlled.localPosition.y - controlled.localScale.y / 2.0f < bottom)
        {
            controlled.localPosition = min;
        }
    }
}
