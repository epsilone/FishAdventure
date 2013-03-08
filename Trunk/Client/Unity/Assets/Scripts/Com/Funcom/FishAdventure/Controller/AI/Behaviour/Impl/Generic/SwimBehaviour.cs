using System;
using System.Collections.Generic;
using UnityEngine;

internal class SwinBehaviour : BaseBehaviour
{

    private static String MOVE_COMPLETE = "MOVE_COMPLETE";
    private static String NEXT_PATH_TRANSITION_COMPLETE = "TRANSITION_COMPLETE";


    private const float ANIMATION_MIN = 10, ANIMATION_MAX = 75;
    private const float PATH_MIN = 1, PATH_MAX = 3;
    private const float DEVIATION_MIN = 0.5f, DEVIATION_MAX = 10f;
    private const int POINT_COUNT_MIN = 1, POINT_COUNT_MAX = 10;

    private static IDebugLogger logger = DebugManager.getDebugLogger(typeof(SwinBehaviour));
    private float animationTime = 5; // max 10 min 75
    private float transitionAnimationTime = 1;
    private Direction direction;
    private System.Random directionRandomizer;
    private List<Vector3> path = null;
    private float pathLength = 5;  // min 1 max 8
    private int pointCount = 5; // min 1 max 10
    private float pointDeviation = 0.5f; // min 0.5 10
    private float pointGap;

    public SwinBehaviour(BehaviourType type, int weight)
        : base(type, weight)
    {

    }



    public override void Start()
    {
        directionRandomizer = new System.Random(LivingEntity.GetId());
        iTween.ColorUpdate(LivingEntity.gameObject, Color.red, .01f);

        AssignNewPath();
        logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " starting");
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

    public override string ToString()
    {
        return base.ToString() + ":" + direction;
    }


    public override void Update()
    {

        iTween.ColorUpdate(LivingEntity.gameObject, Color.red, .01f);

        if (logger.IsExtreme())
        {
            logger.Log(GetType().FullName + " on " + LivingEntity.GetName() + " update");
        }
    }

    private void AssignNewPath()
    {


        //    RandomizePath();

        pointGap = pathLength / pointCount;
        GenerateNewDirection();
        GenerateRandomPath();
        lookTarget = path[1];
        CorrectRotation();
        MoveOnPath();
    }

    private void RandomizePath()
    {
        pathLength = (float)directionRandomizer.NextDouble() * (PATH_MAX - PATH_MIN) + PATH_MIN;
        pointCount = directionRandomizer.Next(POINT_COUNT_MIN, POINT_COUNT_MAX);
        pointDeviation = (float)directionRandomizer.NextDouble() * (DEVIATION_MAX - DEVIATION_MIN) + DEVIATION_MIN;
        animationTime = (float)directionRandomizer.NextDouble() * (ANIMATION_MAX - ANIMATION_MIN) + ANIMATION_MIN;
    }

    private void MoveOnPath()
    {
        iTween.MoveTo(LivingEntity.gameObject,
                       iTween.Hash("path", path.ToArray(), "time", animationTime, "easetype", iTween.EaseType.linear,
                                   "looptype", iTween.LoopType.none, "onnewpathposition", "GenericTweenUpdate", "oncomplete",
                                   "GenericTweenUpdate", "oncompleteparams", MOVE_COMPLETE, "orienttopath", true,
                                   "ignorebuiltinlook", true));
    }

    /**
     * This method needs adjustement to take into account the size of the mesh.
     * **/

    private Boolean canSwimIndirection(Direction direction)
    {
        // This needs some rework
        Vector3 rootPosition = LivingEntity.transform.position;

        // TODO : Implement, just a marker for now
        float MaxX;
        float MinY;
        float MaxY;
        float MinX;
        switch (direction)
        {
            case Direction.UP:
                MinX = rootPosition.x;
                MaxX = rootPosition.x;
                MinY = rootPosition.y;
                MaxY = rootPosition.y + (pathLength + pointGap);
                break;
            case Direction.DOWN:
                MinX = rootPosition.x;
                MaxX = rootPosition.x;
                /*account for deviation*/
                MinY = rootPosition.y - (pathLength + pointGap) - pointDeviation;
                MaxY = rootPosition.y;
                break;
            case Direction.LEFT:
                MinX = rootPosition.x - (pathLength + pointGap) - pointDeviation;
                MaxX = rootPosition.x;
                MinY = rootPosition.y;
                MaxY = rootPosition.y;

                break;
            default:// right = default
                MinX = rootPosition.x;
                MaxX = rootPosition.x + (pathLength + pointGap) + pointDeviation;
                MinY = rootPosition.y;
                MaxY = rootPosition.y;
                break;
        }

        Boolean canSwim = MaxX < World.GetMaxX() && MaxY < World.GetMaxY() && MinY > World.GetMinY() && MinX > World.GetMinX();
        if (!canSwim)
        {

            logger.Log("Cur pos " + rootPosition + " Can't go direction Direction " + direction + "MinX " + MinX + " MaxX " + MaxX + " MinY " + MinY + " MaxY " + MaxY + "can Swim " + (MaxX < World.GetMaxX() && MaxY < World.GetMaxY() && MinY > World.GetMinY() && MinX > World.GetMinX()) + " cur pos " + LivingEntity.transform.position);

        }
        return canSwim;
    }

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

        if (logger.IsExtreme())
        {
            logger.Log("Valid directions " + validDirections.Count);
        }

        int directionIndex = directionRandomizer.Next(validDirections.Count);
        if (logger.IsExtreme())
        {
            logger.Log("New direction generated " + directionIndex);
        }


        //direction = Direction.DOWN;
        //TODO : FIX ME

        direction = validDirections[directionIndex];

        LivingEntity.informationBoxBehaviour = ToString();

        //if (DebugLogger.isExtreme(GetType()))
        // {
        //    Debug.Log("New direction generated " + direction);
        // }
    }

    private Vector3 nextPathStart;
    private void GenerateRandomPath()
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
        //nextPathStart = new Vector3(endX, endY, rootPosition.z);
        path.Add(new Vector3(endX, endY, rootPosition.z));
    }

    private List<Direction> GetValidDirections()
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

    public override void LateUpdate()
    {
        CorrectRotation();
    }

    private void CorrectRotation()
    {
        if (lookTarget != null)
        {
            Vector3 arg = (Vector3)lookTarget;


            Vector3 lookAtDirection = arg - LivingEntity.transform.position;
            lookAtDirection.Normalize();


            Quaternion targetRotation;

            if (lookAtDirection.Equals(Vector3.zero))
            {
                return;
            }
            if (direction == Direction.UP || direction == Direction.DOWN)
            {
                // fix me !


                targetRotation = Quaternion.LookRotation(lookAtDirection, Vector3.up);

                targetRotation *= Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(0, Vector3.right);
            }
            else
            {
                // good !
                targetRotation = Quaternion.LookRotation(lookAtDirection, Vector3.forward);
                if (direction == Direction.RIGHT)
                {
                    targetRotation *= Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
                }
                else
                {
                    targetRotation *= Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right);
                }
            }

            //LivingEntity.transform.rotation = targetRotation;
            LivingEntity.transform.rotation = Quaternion.Slerp(LivingEntity.transform.rotation,
                                                               Quaternion.RotateTowards(LivingEntity.transform.rotation,
                                                                                        targetRotation, 60), 0.20f);
        }
    }


    private Vector3 lookTarget;
    public override void GenericTweenUpdate(object args)
    {

        base.GenericTweenUpdate(args);


        if (args is Vector3)
        {
            lookTarget = (Vector3)args;
        }
        else if (args is String)
        {

            AssignNewPath();
        }
    }


}