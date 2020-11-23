using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Missile : GameObject
    {
        private Trail trail;

        public Vector Position => trail.Position;

        public Missile(Vector from, Vector to, double speed)
        {
            trail = new Trail(from, to, speed, Colors.Blue, Colors.BlueViolet);
            var explosion = new Explosion(to, Random(30, 100), 100);

            trail.Completed += () => Add(explosion);
            explosion.Faded += () => this.Destroy();

            Add(trail);
        }
    }
}
