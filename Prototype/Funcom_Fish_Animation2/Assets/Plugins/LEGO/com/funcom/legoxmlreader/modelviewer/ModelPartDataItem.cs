using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class ModelPartDataItem
    {
        private Vector3 assembledPosition; // assembled position
        private Quaternion assembledRotation;	// assembled rotation

        private Vector3[] explodeTrajectory;
        private Vector3[] implodeTrajectory;
        private Vector3[] controlPoints;

        public ModelPartDataItem(Vector3 p, Quaternion q)
        {
            assembledPosition = p;
            assembledRotation = q;
            controlPoints = new Vector3[3];
        }
        public Vector3 AssembledPosition
        {
            get
            {
                return assembledPosition;
            }
            set
            {
                assembledPosition = value;
            }
        }
        public Quaternion AssembledRotation
        {
            get
            {
                return assembledRotation;
            }
            set
            {
                assembledRotation = value;
            }
        }

        public Vector3[] ExplodeTrajectory
        {
            get
            {
                return explodeTrajectory;
            }
        }
        public Vector3[] ImplodeTrajectory
        {
            get
            {
                return implodeTrajectory;
            }
        }
        private int betweenNodeCount = 40;

        public void ComputeExplodeTrajectory(Vector3 explodedPosition)
        {
            // here we compute the trajectory from the assembled position out to the common exploded position
            // therefore need to make sure the part is in it's correct assembled position before the calculation
            explodeTrajectory = new Vector3[betweenNodeCount * (controlPoints.Length - 1) + controlPoints.Length];
            // calculate the end (exploded) point and the part way along mid point for catmull rom mid point
            // take a position on the direct line from the start position to the end(exploded) position +/- random variation, then
            // rotate a point out along the xz plane - this will be the catmull rom mid point

            Vector3 midPoint = explodedPosition - assembledPosition;
            midPoint = midPoint * UnityEngine.Random.Range(0.60f, 0.80f);
            Quaternion q = Quaternion.Euler(UnityEngine.Random.Range(-10.0f, 10.0f), //Z
                                            UnityEngine.Random.Range(15.0f, 55.0f), //Y
                                            0.0f/*UnityEngine.Random.Range(-10.0f,10.0f)*/);//X
            midPoint = q * midPoint;

            controlPoints[0] = assembledPosition;
            controlPoints[1] = midPoint + assembledPosition;
            controlPoints[2] = explodedPosition;

            IEnumerator<Vector3> nodes = Interpolate.NewCatmullRom(controlPoints, betweenNodeCount, false).GetEnumerator();

            for (int i = 0; i < explodeTrajectory.Length; ++i)
            {
                if (nodes.MoveNext())
                {
                    explodeTrajectory[i] = nodes.Current;
                }
            }
        }

        public void ComputeImplodeTrajectory(Vector3 explodedPosition)
        {
            // here we compute the trajectory from the common exploded position in to the assembled position
            // therefore need to make sure the part is in it's correct assembled position before the calculation
            implodeTrajectory = new Vector3[betweenNodeCount * (controlPoints.Length - 1) + controlPoints.Length];
            // calculate the end (assembled) point and the part way along mid point for catmull rom mid point
            // take a position on the direct line from the start position to the end(assembled) position +/- random variation, then
            // rotate a point out along the xz plane - this will be the catmull rom mid point

            Vector3 midPoint = explodedPosition - assembledPosition;
            midPoint = midPoint * UnityEngine.Random.Range(0.60f, 0.80f);
            Quaternion q = Quaternion.Euler(UnityEngine.Random.Range(-10.0f, 10.0f), //Z
                                            UnityEngine.Random.Range(-15.0f, -55.0f), //Y
                                            0.0f/*UnityEngine.Random.Range(-10.0f,10.0f)*/);//X
            midPoint = q * midPoint;

            controlPoints[0] = assembledPosition;
            controlPoints[1] = midPoint + assembledPosition;
            controlPoints[2] = explodedPosition;

            IEnumerator<Vector3> nodes = Interpolate.NewCatmullRom(controlPoints, betweenNodeCount, false).GetEnumerator();

            for (int i = 0; i < implodeTrajectory.Length; ++i)
            {
                if (nodes.MoveNext())
                {
                    implodeTrajectory[i] = nodes.Current;
                }
            }

            //		implodeTrajectory = new Vector3[explodeTrajectory.Length];
            //		for( int i = 1; i < explodeTrajectory.Length-1; i++)
            //		{
            //			implodeTrajectory[i] = explodeTrajectory[i];
            //			implodeTrajectory[i].y = explodedPosition.y - assembledPosition.y; // centre on y
            //			implodeTrajectory[i].y *= -1;
            //			implodeTrajectory[i].y += assembledPosition.y;
            //		}
        }
    }
}

