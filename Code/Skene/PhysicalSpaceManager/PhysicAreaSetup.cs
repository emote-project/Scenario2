using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalSpaceManager
{
    [Serializable]
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

        public Vector2D(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        static public Vector2D Direction(Vector2D fromPoint, Vector2D toPoint) {
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
            if (dir1.Cross(dir2) >  0) 
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
            return v1.X==v2.X && v1.Y==v2.Y;
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

    [Serializable]
    public struct Vector3D
    {
        public double X;
        public double Y;
        public double Z;
        public Vector3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    [Serializable]
    public struct ScreenSetup
    {
        public Vector3D _bottomLeftCorner;
        public Vector2D _size;
        public Vector2D _resolution;

        public ScreenSetup(Vector2D size, Vector2D resolution) : this(new Vector3D(0, 0, 0), size, resolution) { }
        public ScreenSetup(Vector3D bottomLeftCorner, Vector2D size, Vector2D resolution)
        {
            _bottomLeftCorner = bottomLeftCorner;
            _size = size;
            _resolution = resolution;
        }
    }

    [Serializable]
    public class PhysicalSpace
    {
        public string _name;
        public ScreenSetup _screenSetup;
        public Vector3D _headPosition;
        public Vector3D _rightShoulderPosition;
        public Vector3D _leftShoulderPosition;
        public Vector2D _forward;


        public PhysicalSpace(ScreenSetup screenSetup, Vector3D headPosition, Vector2D forward, string name="") : this(screenSetup, headPosition, new Vector3D(0,0,0), new Vector3D(0,0,0), forward, name) {}

        public PhysicalSpace(ScreenSetup screenSetup, Vector3D headPosition, Vector3D rightShoulderPosition, Vector3D leftShoulderPosition, Vector2D forward, string name = "")
        {
            _name = name;
            _screenSetup = screenSetup;
            _headPosition = headPosition;
            _forward = forward;
            _leftShoulderPosition = leftShoulderPosition;
            _rightShoulderPosition = rightShoulderPosition;
        }

        /// <summary>
        /// Given a point on the screen in front of the robot, calculates the angles the head needs to move to gaze to this point
        /// </summary>
        /// <param name="x">x coord of the screen in pixels</param>
        /// <param name="y">y coord of the screen in pixels</param>
        /// <returns>A vector2D containing a x value indicating the horizzontal angle and a y value indicating the vertical angle in degrees the head needs to move to gaze to a point</returns>
        public Vector2D GazeToScreenPoint(double x, double y)
        {
            return AnglesToPoint(x, y, _headPosition);
        }


        public Vector2D PointToScreenPoint(double x, double y)
        {
            Vector2D angles = new Vector2D(0, 0);

            if (IsAtRobotRight(x,y))
            {
                angles = AnglesToPoint(x, y, _rightShoulderPosition);
            }
            else
            {
                angles = AnglesToPoint(x, y, _leftShoulderPosition);
            }

            return angles;
        }

        /// <summary>
        /// Determines if a point on the screen is physically located at the robot's right side
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>true if the coordinates coorespond to a point on the right</returns>
        public bool IsAtRobotRight(double x, double y)
        {
            return AnglesToPoint(x, y, _headPosition).X>=0;
        }

        private Vector2D AnglesToPoint(double x, double y, Vector3D origin)
        {
            Vector2D angles = new Vector2D(0, 0);
            Vector3D screenPointPhysicalCoords = ScreenToPhysicalCoords(new Vector2D(x, y));

            Vector2D directionToPointHorizzontal = Vector2D.Direction(new Vector2D(origin.X, origin.Y), new Vector2D(screenPointPhysicalCoords.X, screenPointPhysicalCoords.Y));
            Vector2D directionToPointVertical = Vector2D.Direction(new Vector2D(origin.Z, origin.Y), new Vector2D(screenPointPhysicalCoords.Z, screenPointPhysicalCoords.Y));

            angles.X = Vector2D.AngleBetweenDirections(_forward, directionToPointHorizzontal);
            angles.Y = Vector2D.AngleBetweenDirections(_forward, directionToPointVertical);

            // HACK to avoid the robot looking upward when he's located on the upper side of the screen
            if (angles.Y > 0) angles.Y *= -1;

            return angles;
        }

        private Vector3D ScreenToPhysicalCoords(Vector2D screenCoords)
        {
            Vector3D screenPointPhysicalCoords = new Vector3D(0, 0, 0);
            screenPointPhysicalCoords.X = screenCoords.X * _screenSetup._size.X / _screenSetup._resolution.X;
            screenPointPhysicalCoords.Y = screenCoords.Y * _screenSetup._size.Y / _screenSetup._resolution.Y;

            screenPointPhysicalCoords.X += _screenSetup._bottomLeftCorner.X;
            screenPointPhysicalCoords.Y += _screenSetup._bottomLeftCorner.Y;
            screenPointPhysicalCoords.Z += _screenSetup._bottomLeftCorner.Z;

            return screenPointPhysicalCoords;
        }

        private double GetAngleOfLineBetweenTwoPoints(Vector2D p1, Vector2D p2)
        {
            double xDiff = p2.X - p1.X;
            double yDiff = p2.Y - p1.Y;
            return Math.Atan2(xDiff, yDiff) * 180 / Math.PI;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
