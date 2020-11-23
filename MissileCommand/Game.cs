using System;
using System.Collections.Generic;
using System.Windows;

namespace MissileCommand
{
    static class Game
    {
        public static int Score { get; set; }
        public static int Round { get; set; }
        public static bool Paused { get; set; }

        public static List<GameObject> Enemies;
        public static List<GameObject> Silos;
        public static List<GameObject> Cities;

        public static GameObject NearestSilo(Vector vector)
        {
            throw new NotImplementedException();
        }
    }
}
