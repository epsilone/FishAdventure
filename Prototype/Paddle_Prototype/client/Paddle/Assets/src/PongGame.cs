using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class PongGame : MonoBehaviour 
{
    private List<PongEntity> staticEntities;
    private List<PongEntity> dynamicEntities;

    private Transform LeftPaddle { get; set; }
    private Transform RightPaddle { get; set; }
    private Transform Ball { get; set; }

    private int playerOne;
    private int playerTwo;

    public void Register(PongEntity entity)
    {
        var list = entity.isStatic ? staticEntities : dynamicEntities;
        list.Add(entity);
    }

    void Awake()
    {
        staticEntities = new List<PongEntity>();
        dynamicEntities = new List<PongEntity>();
    }

    void Start()
    {
        playerOne = 0;
        playerTwo = 0;
        LeftPaddle = GameObject.Find("LeftPaddle").transform;
        RightPaddle = GameObject.Find("RightPaddle").transform;
        Ball = GameObject.Find("Ball").transform;
    }

    //void Update()
    //{
    //    if (Ball.localPosition.x > RightPaddle.localPosition.x)
    //    {
    //        playerOne += 1;
    //        Ball.transform.localPosition = Vector3.zero;
    //    }

    //    if (Ball.localPosition.x < LeftPaddle.localPosition.x)
    //    {
    //        playerTwo += 1;
    //        Ball.transform.localPosition = Vector3.zero;
    //    }

    //    float dt = Time.deltaTime;
    //    foreach (PongEntity entity in dynamicEntities)
    //    {
    //        var entityBox = new Bounds(entity.transform.localPosition, entity.transform.localScale);
    //        entity.gameObject.transform.Translate(entity.Displacement * entity.speed * dt);
    //        foreach (PongEntity staticEntity in staticEntities)
    //        {
    //            var staticEntityBox = new Bounds(staticEntity.transform.localPosition, staticEntity.transform.localScale);
    //            if (entityBox.Intersects(staticEntityBox))
    //            {
    //                Debug.Log(entity.transform.localPosition);
    //                Debug.Log(entity.gameObject.name + " collides with " + staticEntity.gameObject.name);
    //                var dotProduct = Vector3.Dot(entity.Displacement, staticEntity.normal);
    //                entity.Displacement = entity.Displacement - (2 * dotProduct * staticEntity.normal);
    //                entity.Displacement.Normalize();
    //                var x = entity.transform.localPosition.x;
    //                var y = entity.transform.localPosition.y;
    //                Debug.Log("Before: " + entity.transform.localPosition);
    //                if (staticEntity.normal.x > 0)
    //                {
    //                    x += (staticEntity.transform.localScale.x + entity.transform.localScale.x) / 2.0f;
    //                }
    //                else if (staticEntity.normal.x < 0)
    //                {
    //                    x -= (staticEntity.transform.localScale.x + entity.transform.localScale.x) / 2.0f;
    //                }

    //                if (staticEntity.normal.y > 0)
    //                {
    //                    y += (staticEntity.transform.localScale.y + entity.transform.localScale.y) / 2.0f;
    //                }
    //                else if (staticEntity.normal.y < 0)
    //                {
    //                    y -= (staticEntity.transform.localScale.y + entity.transform.localScale.y) / 2.0f;
    //                }

    //                entity.transform.localPosition = new Vector3(x, y, 0.0f);
    //                Debug.Log("After: " + entity.transform.localPosition);
    //            }
    //        }
    //    }
    //}

    void Update()
    {
        if (Ball.localPosition.x > RightPaddle.localPosition.x + 5.0f)
        {
            playerOne += 1;
            Ball.transform.localPosition = Vector3.zero;
        }

        if (Ball.localPosition.x < LeftPaddle.localPosition.x - 5.0f)
        {
            playerTwo += 1;
            Ball.transform.localPosition = Vector3.zero;
        }
        float dt = Time.deltaTime;
        float consummed = dt;
        var ballPongEntity = Ball.GetComponent<PongEntity>();
        while (consummed > 0)
        {
            var displacement = ballPongEntity.Displacement * ballPongEntity.speed * consummed;
            foreach (PongEntity staticEntity in staticEntities)
            {
                var d0 = Vector3.Dot(Ball.localPosition - staticEntity.transform.localPosition, staticEntity.normal);
                var translatedEntity = new Vector3(Ball.localPosition.x + displacement.x, Ball.localPosition.y + displacement.y, 0.0f);
                var d1 = Vector3.Dot(translatedEntity - staticEntity.transform.localPosition, staticEntity.normal);
                if (d0 > Ball.localScale.x && d1 < Ball.localScale.x)
                {
                    var u = (d0 - Ball.localScale.x) / (d0 - d1);
                    var intersectionPoint = ((1 - u) * Ball.localPosition) + (u * translatedEntity);
                    var scale = new Vector3(staticEntity.transform.localScale.x + 1.0f, staticEntity.transform.localScale.y + 1.0f, staticEntity.transform.localScale.z);
                    var bounds = new Bounds(staticEntity.transform.localPosition, scale);
                    if (bounds.Contains(intersectionPoint))
                    {
                        var distanceToIntersect = Vector3.Distance(Ball.localPosition, intersectionPoint);
                        consummed -= (distanceToIntersect * consummed / displacement.magnitude);
                        Ball.transform.localPosition = intersectionPoint;
                        var dotProduct = Vector3.Dot(ballPongEntity.Displacement, staticEntity.normal);
                        ballPongEntity.Displacement = ballPongEntity.Displacement - (2 * dotProduct * staticEntity.normal);

                        Debug.Log("Ball position: " + Ball.localPosition);
                        Debug.Log("DeltaTime: " + consummed);
                        Debug.Log("IntersectionPoint :" + intersectionPoint + " time: " + u);
                        Debug.Log("Distance: " + distanceToIntersect);
                        Debug.Log("displacement: " + displacement.magnitude);
                        Debug.Log("translatedEntity: " + translatedEntity);
                        Debug.Log("Consummed: " + consummed);
                    }
                }
            }

            if (consummed > 0)
            {
                Ball.Translate(displacement);
                consummed = 0.0f;
            }
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
        GUILayout.BeginVertical();
        var p1Style = new GUIStyle(GUI.skin.label);
        p1Style.fontSize = 20;
        GUILayout.Label("Player 1", p1Style);
        p1Style = new GUIStyle(GUI.skin.label);
        p1Style.fontSize = 40;
        GUILayout.Label("" + playerOne, p1Style);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        var p2Style = new GUIStyle(GUI.skin.label);
        p2Style.fontSize = 20;
        p2Style.alignment = TextAnchor.MiddleRight;
        GUILayout.Label("Player 2", p2Style);
        p2Style = new GUIStyle(GUI.skin.label);
        p2Style.fontSize = 40;
        p2Style.alignment = TextAnchor.MiddleRight;
        GUILayout.Label("" + playerTwo, p2Style);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

}
