using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SwinBehaviour : BaseBehaviour
{
    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(SwinBehaviour));
    int pointCount = 6;
    float pathLength = 10;
    float pointDeviation = 1f;
    float animationTime = 2;
    List<Vector3> path = null;
    float pointGap;
    System.Random directionRandomizer;
    Direction direction;
    public SwinBehaviour(BehaviourType type, int weight)
        : base(type, weight)
    {
        pointGap = pathLength / pointCount;
    }

    public override void Start()
    {

        directionRandomizer = new System.Random(LivingEntity.GetId());
        direction = Direction.DOWN;
        pointGap = pathLength / pointCount;
        iTween.ColorFrom(LivingEntity.gameObject, Color.red, .3f);
        AssignNewPath();
       
         logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
    }

    private void AssignNewPath()
    {
        GenerateNewDirection();
        GenerateRandomPath();

        iTween.MoveTo(LivingEntity.gameObject, iTween.Hash("path", path.ToArray(), "time", animationTime, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.none));
    }

    /**
     * This method needs adjustement to take into account the size of the mesh.
     * **/
    private void GenerateNewDirection()
    {
        List<Direction> validDirections = GetValidDirections();

        foreach (Direction curDir in validDirections)
        {
            if (logger.IsExtreme())
            {
                logger.Log("Valid direction " + curDir);
            }
        }
        

        //   if (DebugLogger.isExtreme(GetType()))
        //   {
        //      Debug.Log("Valid directions " + validDirections.Count);
        //  }

        int directionIndex = directionRandomizer.Next(validDirections.Count);
        //   if (DebugLogger.isExtreme(GetType()))
        //  {
        //     Debug.Log("New direction generated " + directionIndex);
        // }

        //TODO : Handle case of empty list, send to opposite direction
        direction = validDirections[directionIndex];
        LivingEntity.informationBoxBehaviour = ToString();
        //if (DebugLogger.isExtreme(GetType()))
        // {
        //    Debug.Log("New direction generated " + direction);
        // }

    }

    List<Direction> GetValidDirections()
    {
        List<Direction> returnValue = new List<Direction>();

        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            if (canSwimIndirection(direction))
            {
                returnValue.Add(direction);
            }
        }
        return returnValue;
    }


    Boolean canSwimIndirection(Direction direction)
    {
        Vector3 rootPosition = LivingEntity.transform.position;
        // TODO : Implement, just a marker for now
        float MaxX;
        float MinY;
        float MaxY;
        float MinX;


        switch (direction)
        {
            case Direction.UP:
                MinX = rootPosition.x - pointDeviation;
                MaxX = rootPosition.x + pointDeviation;
                MinY = rootPosition.y;
                MaxY = rootPosition.y + (pathLength + pointGap);
                break;
            case Direction.DOWN:
                MinX = rootPosition.x - pointDeviation;
                MaxX = rootPosition.x + pointDeviation;
                /*account for deviation*/
                MinY = rootPosition.y - (pathLength + pointGap) - pointDeviation;
                MaxY = rootPosition.y;
                break;
            case Direction.LEFT:
                MinX = rootPosition.x - (pathLength + pointGap) - pointDeviation;
                MaxX = rootPosition.x;
                MinY = rootPosition.y - pointDeviation;
                MaxY = rootPosition.y + pointDeviation;

                break;
            default:// right = default
                MinX = rootPosition.x;
                MaxX = rootPosition.x + (pathLength + pointGap) + pointDeviation;
                MinY = rootPosition.y - pointDeviation;
                MaxY = rootPosition.y + pointDeviation;
                break;
        }

        if (logger.IsExtreme()) {
            logger.Log("Direction " + direction);
            logger.Log("Direction " + direction + "MinX " + MinX + " MaxX " + MaxX + " MinY " + MinY + " MaxY " + MaxY + "can Swim " + (MaxX < World.GetMaxX() && MaxY < World.GetMaxY() && MinY > World.GetMinY() && MinX > World.GetMinX()) + " cur pos " + LivingEntity.transform.position);
        }

        

        return MaxX < World.GetMaxX() && MaxY < World.GetMaxY() && MinY > World.GetMinY() && MinX > World.GetMinX();
    }


    void GenerateRandomPath()
    {
        Vector3 rootPosition = LivingEntity.transform.position;
        path = new List<Vector3>(pointCount + 2);
        path.Add(rootPosition);

        int deviationFactor = 1;
        for (int i = 1; i < pointCount + 1; i++)
        {
            deviationFactor = deviationFactor * -1;
            float randomY;
            float newX;
            switch (direction)
            {
                case Direction.UP:
                    newX = rootPosition.x + pointDeviation * deviationFactor;
                    randomY = rootPosition.y + (pointGap * i);
                    break;
                case Direction.DOWN:
                    newX = rootPosition.x + pointDeviation * deviationFactor;
                    randomY = rootPosition.y - (pointGap * i);
                    break;
                case Direction.LEFT:
                    randomY = rootPosition.y + pointDeviation * deviationFactor;
                    newX = rootPosition.x - (pointGap * i);
                    break;
                default:// right = default
                    randomY = rootPosition.y + pointDeviation * deviationFactor;
                    newX = rootPosition.x + (pointGap * i);
                    break;
            }
            path.Add(new Vector3(newX, randomY, rootPosition.z));
        }

        //path[0] = rootPosition;
        float endX = rootPosition.x + (pathLength + pointGap);
        float endY = rootPosition.y;

        switch (direction)
        {
            case Direction.UP:
                endX = rootPosition.x;
                endY = rootPosition.y + (pathLength + pointGap);
                break;
            case Direction.DOWN:
                endX = rootPosition.x;
                endY = rootPosition.y - (pathLength + pointGap);
                break;
            case Direction.LEFT:
                endX = rootPosition.x - (pathLength + pointGap);
                endY = rootPosition.y;
                break;
            default:// right = default
                endX = rootPosition.x + (pathLength + pointGap);
                endY = rootPosition.y;
                break;
        }
        path.Add(new Vector3(endX, endY, rootPosition.z));

    }

    public override void Update()
    {

        iTween.ColorFrom(LivingEntity.gameObject, Color.red, .3f);
        if (LivingEntity != null && LivingEntity.transform.position == path[path.Count - 1])
        {

            AssignNewPath();
        }
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " update");
        }
    }

    public override void Stop()
    {
        logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " stop");

        iTween.Stop(LivingEntity.gameObject);
        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " stop");
        }
    }

    public override void OnDrawGizmos()
    {
        if (path != null)
        {
            if (path.Count > 0)
            {
                iTween.DrawPath(path.ToArray());
            }
        }
    }

    public override void TweenUpdate()
    {

    }

    public override string ToString()
    {
        return base.ToString() + ":" + direction;
    }
}
