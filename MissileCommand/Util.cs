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
    }
}
