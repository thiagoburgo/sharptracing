using System.Runtime.InteropServices;
using System;
namespace DrawEngine.Renderer.Algebra
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Point2D
    {
        public double X, Y;
        public Point2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public override string ToString()
        {
            return "(X=" + this.X + ", Y=" + this.Y + ")";
        }
        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !p1.Equals(p2);
        }
        public static Point2D operator +(Point2D p1, Point2D p2)
        {
            return new Point2D((p1.X + p2.X), (p1.Y + p2.Y));
        }
        public static Point2D operator +(Point2D p1, Vector2D v2)
        {
            return new Point2D((p1.X + v2.X), (p1.Y + v2.Y));
        }
        public static Point2D operator +(Vector2D v2, Point2D p1)
        {
            return new Point2D((p1.X + v2.X), (p1.Y + v2.Y));
        }
        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            return new Vector2D((p1.X - p2.X), (p1.Y - p2.Y));
        }
        public static Point2D operator -(Point2D p1)
        {
            return new Point2D(-p1.X, -p1.Y);
        }
        public static Point2D operator *(Point2D p1, double scalar)
        {
            return new Point2D((p1.X * scalar), (p1.Y * scalar));
        }
        public static Point2D operator *(double scalar, Point2D p1)
        {
            return new Point2D((p1.X * scalar), (p1.Y * scalar));
        }
        public static Point2D operator /(Point2D p1, double scalar)
        {
            return new Point2D((p1.X * 1 / scalar), (p1.Y * 1 / scalar));
        }
        //TODO Verificar se isso é certo
        public static Point2D operator *(Point2D p1, Point2D p2)
        {
            return new Point2D((p1.X * p2.X), (p1.Y * p2.Y));
        }
        public Vector2D ToVector2D()
        {
            return new Vector2D(this.X, this.Y);
        }
    }
}