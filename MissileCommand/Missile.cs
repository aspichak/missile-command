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
        private Explosion explosion;

        public Vector Position => trail.Position;

        public Missile(Vector from, Vector to, double speed)
        {
            trail = new Trail(from, to, speed, Colors.Blue, Colors.BlueViolet);

            trail.Completed += () =>
            {
                explosion = new Explosion(Position, Random(30, 100), 100);
                this.Destroy();
            };
        }

        public override void Update(double dt)
        {
        }
    }
}
