using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MissileCommand
{
    class Game : GameObject
    {
        public int Score { get; set; }
        public int Round { get; set; }
        public bool Paused { get; set; }

        public Game(Canvas canvas)
        {
            Canvas = canvas;
            Game = this;
        }

        public override void Update(double dt)
        {
            //if (Paused) return;
            base.Update(dt);
        }
    }
}
