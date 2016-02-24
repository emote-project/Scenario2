using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThalamusEnercities
{
    public struct Vector2D
    {
        public static Vector2D Zero = new Vector2D(0, 0);

        private static Random random = new Random();
        public static Vector2D Random(double range = 1.0)
        {
            return new Vector2D((random.NextDouble() > 0.5 ? -1 : 1) * random.NextDouble() * range, (random.NextDouble() > 0.5 ? -1 : 1) * random.NextDouble() * range);
        }

        public static Vector2D Random(double rangeX = 1.0, double rangeY = 1.0)
        {
            return new Vector2D((random.NextDouble() > 0.5 ? -1 : 1) * random.NextDouble() * rangeX, (random.NextDouble() > 0.5 ? -1 : 1) * random.NextDouble() * rangeY);
        }

        public double X { get; set; }
        public double Y { get; set; }

        public Vector2D(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        static public Vector2D Direction(Vector2D fromPoint, Vector2D toPoint)
        {
            Vector2D result = new Vector2D(0, 0);
            result.X = toPoint.X - fromPoint.X;
            result.Y = toPoint.Y - fromPoint.Y;
            return result.Normalized();
        }

        /// <summary>
        /// Angle in degrees between two directions. The angle is positive if the direction passed as second parameter points at the left of the first one
        /// (considering the first one as a vector pointing forward). The angle is negative otherwise.
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns> Returns an angles between two directions.  </returns>
        static public double AngleBetweenDirections(Vector2D dir1, Vector2D dir2)
        {
            Vector2D dir1norm = dir1.Normalized();
            Vector2D dir2norm = dir2.Normalized();
            double dot = dir1norm.Dot(dir2norm);
            double angle = Math.Acos(dot) * 180 / Math.PI;

            // If the coss product vector points down, than dir2 poitn to the left relatively to dir1
            if (dir1.Cross(dir2) > 0)
                angle *= -1;
            return angle;
        }

        public double Dot(Vector2D secondVector)
        {
            return X * secondVector.X + Y * secondVector.Y;
        }

        public Vector2D Normalized()
        {
            Vector2D result = new Vector2D(0, 0);
            if (Length() != 0)
            {
                result.X = X / Length();
                result.Y = Y / Length();
            }
            return result;
        }

        public double Length()
        {
            return Math.Sqrt((X * X) + (Y * Y));
        }

        /// <summary>
        /// Calculate the cross product of two vectors
        /// if you don't remember ---> http://en.wikipedia.org/wiki/Cross_product#Cross_visualization
        /// </summary>
        /// <param name="v2"></param>
        /// <returns> since the vectors are bidimensional, it returns only the third coordinate of the resulting tridimensional cross vector  </returns>
        public double Cross(Vector2D v2)
        {
            return X * v2.Y - Y * v2.X; ;
        }

        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }


        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "[" + X.ToString() + ", " + Y.ToString() + "]";
        }
    }
}
