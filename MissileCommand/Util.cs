using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;

namespace MissileCommand
{
    static class Util
    {
        private static Random rand = new Random();
<<<<<<< HEAD
=======
        public static double Random(double min, double max) => min + rand.NextDouble() * (max - min);
        public static int Random(int min, int max) => rand.Next(min, max);
        public static T Random<T>(this IEnumerable<T> list) => list.ElementAt(Random(0, list.Count()));
>>>>>>> 6723f75... Added screen scaling, building layout logic

        public static int Random(int min, int max) => rand.Next(min, max);
        public static double Random(double min, double max) => min + rand.NextDouble() * (max - min);
        public static double Lerp(double from, double to, double t) => from * (1 - t) + to * t;
        public static Vector Lerp(Vector from, Vector to, double t) => from * (1 - t) + to * t;
        public static double Angle(this Vector vector) => Math.Atan2(vector.X, vector.Y);
        public static double AngleTo(this Vector vector, Vector to) => Math.Atan2((to.X - vector.X), (to.Y - vector.Y));
        public static double DistanceTo(this Vector from, Vector to) => (to - from).Length;
        public static Point ToPoint(this Vector vector) => new Point(vector.X, vector.Y);
        public static Vector Normalized(this Vector vector) => vector / vector.Length;
        public static double OrZero(this double value) => double.IsNaN(value) ? 0 : value;

        // https://stackoverflow.com/questions/200574/linq-equivalent-of-foreach-for-ienumerablet
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
