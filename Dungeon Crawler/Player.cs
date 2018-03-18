using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Dungeon_Crawler
{
    class Player
    {
        public Rectangle CollisionRectangle;
        public float Rotation;
        public Vector2 Center;
        
        public Player(Rectangle theRectangle, float theInitialRotation)
        {
            CollisionRectangle = theRectangle;
            Rotation = theInitialRotation;
            Center = new Vector2((int)theRectangle.Width / 2, (int)theRectangle.Height / 2);
        }

        /// based on http://www.xnadevelopment.com/tutorials/rotatedrectanglecollisions/rotatedrectanglecollisions.shtml
        /// <summary>
        /// This intersects method can be used to check a standard XNA framework Rectangle
        /// object and see if it collides with a Rotated Rectangle object
        /// </summary>
        /// <param name="theRectangle"></param>
        /// <returns></returns>
        public bool Intersects(Rectangle theRectangle)
        {
            return Intersects(new Player(theRectangle, 0.0f));
        }

        /// <summary>
        /// Check to see if two Rotated Rectangls have collided
        /// </summary>
        /// <param name="theRectangle"></param>
        /// <returns></returns>
        public bool Intersects(Player theRectangle)
        {
            
            List<Vector2> rectangleAxis = new List<Vector2>();
            rectangleAxis.Add(UpperRightCorner() - UpperLeftCorner());
            rectangleAxis.Add(UpperRightCorner() - LowerRightCorner());
            rectangleAxis.Add(theRectangle.UpperLeftCorner() - theRectangle.LowerLeftCorner());
            rectangleAxis.Add(theRectangle.UpperLeftCorner() - theRectangle.UpperRightCorner());

           
            foreach (Vector2 axis in rectangleAxis)
            {
                if (!IsAxisCollision(theRectangle, axis))
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Determines if a collision has occurred on an Axis of one of the
        /// planes parallel to the Rectangle
        /// </summary>
        /// <param name="theRectangle"></param>
        /// <param name="aAxis"></param>
        /// <returns></returns>
        private bool IsAxisCollision(Player theRectangle, Vector2 aAxis)
        {
            List<int> aRectangleAScalars = new List<int>();
            aRectangleAScalars.Add(GenerateScalar(theRectangle.UpperLeftCorner(), aAxis));
            aRectangleAScalars.Add(GenerateScalar(theRectangle.UpperRightCorner(), aAxis));
            aRectangleAScalars.Add(GenerateScalar(theRectangle.LowerLeftCorner(), aAxis));
            aRectangleAScalars.Add(GenerateScalar(theRectangle.LowerRightCorner(), aAxis));
            
            List<int> aRectangleBScalars = new List<int>();
            aRectangleBScalars.Add(GenerateScalar(UpperLeftCorner(), aAxis));
            aRectangleBScalars.Add(GenerateScalar(UpperRightCorner(), aAxis));
            aRectangleBScalars.Add(GenerateScalar(LowerLeftCorner(), aAxis));
            aRectangleBScalars.Add(GenerateScalar(LowerRightCorner(), aAxis));
            
            int aRectangleAMinimum = aRectangleAScalars.Min();
            int aRectangleAMaximum = aRectangleAScalars.Max();
            int aRectangleBMinimum = aRectangleBScalars.Min();
            int aRectangleBMaximum = aRectangleBScalars.Max();
            
            if (aRectangleBMinimum <= aRectangleAMaximum && aRectangleBMaximum >= aRectangleAMaximum)
            {
                return true;
            }
            else if (aRectangleAMinimum <= aRectangleBMaximum && aRectangleAMaximum >= aRectangleBMaximum)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates a scalar value that can be used to compare where corners of 
        /// a rectangle have been projected onto a particular axis. 
        /// </summary>
        /// <param name="theRectangleCorner"></param>
        /// <param name="theAxis"></param>
        /// <returns></returns>
        private int GenerateScalar(Vector2 theRectangleCorner, Vector2 theAxis)
        {
            float aNumerator = (theRectangleCorner.X * theAxis.X) + (theRectangleCorner.Y * theAxis.Y);
            float aDenominator = (theAxis.X * theAxis.X) + (theAxis.Y * theAxis.Y);
            float aDivisionResult = aNumerator / aDenominator;
            Vector2 aCornerProjected = new Vector2(aDivisionResult * theAxis.X, aDivisionResult * theAxis.Y);
            
            float aScalar = (theAxis.X * aCornerProjected.X) + (theAxis.Y * aCornerProjected.Y);
            return (int)aScalar;
        }

        /// <summary>
        /// Rotate a point from a given location and adjust using the Center we
        /// are rotating around
        /// </summary>
        /// <param name="thePoint"></param>
        /// <param name="theOrigin"></param>
        /// <param name="theRotation"></param>
        /// <returns></returns>
        private Vector2 RotatePoint(Vector2 thePoint, Vector2 theOrigin, float theRotation)
        {
            Vector2 aTranslatedPoint = new Vector2();
            aTranslatedPoint.X = (float)(theOrigin.X + (thePoint.X - theOrigin.X) * Math.Cos(theRotation)
                - (thePoint.Y - theOrigin.Y) * Math.Sin(theRotation));
            aTranslatedPoint.Y = (float)(theOrigin.Y + (thePoint.Y - theOrigin.Y) * Math.Cos(theRotation)
                + (thePoint.X - theOrigin.X) * Math.Sin(theRotation));
            return aTranslatedPoint;
        }
                
        public Vector2 UpperLeftCorner()
        {
            Vector2 aUpperLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Top);
            aUpperLeft = RotatePoint(aUpperLeft, aUpperLeft + Center, Rotation);
            return aUpperLeft;
        }

        public Vector2 UpperRightCorner()
        {
            Vector2 aUpperRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Top);
            aUpperRight = RotatePoint(aUpperRight, aUpperRight + new Vector2(-Center.X, Center.Y), Rotation);
            return aUpperRight;
        }

        public Vector2 LowerLeftCorner()
        {
            Vector2 aLowerLeft = new Vector2(CollisionRectangle.Left, CollisionRectangle.Bottom);
            aLowerLeft = RotatePoint(aLowerLeft, aLowerLeft + new Vector2(Center.X, -Center.Y), Rotation);
            return aLowerLeft;
        }

        public Vector2 LowerRightCorner()
        {
            Vector2 aLowerRight = new Vector2(CollisionRectangle.Right, CollisionRectangle.Bottom);
            aLowerRight = RotatePoint(aLowerRight, aLowerRight + new Vector2(-Center.X, -Center.Y), Rotation);
            return aLowerRight;
        }

    }
}
