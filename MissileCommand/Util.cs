using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand
{
    static class Util
    {
        public static double Lerp(double from, double to, double t) => from * (1 - t) + to * t;
        public static Vector Lerp(Vector from, Vector to, double t) => from * (1 - t) + to * t;
        public static double Angle(this Vector vector) => Math.Atan2(vector.X, vector.Y);
        public static double AngleTo(this Vector vector, Vector to) => Math.Atan2((to.X - vector.X), (to.Y - vector.Y));
        public static double DistanceTo(this Vector from, Vector to) => (to - from).Length;
        public static Point ToPoint(this Vector vector) => new Point(vector.X, vector.Y);

        public static Vector Normalized(this Vector vector)
        { 
            var res = new Vector(vector.X, vector.Y);
            res.Normalize();
            return res;
        }
    }
}
