using System;
using System.Windows;
using System.Windows.Media;

namespace MissileCommand
{
    class Missile : GameElement
    {
        public Vector Position { get; private set; }
        public event Action<Vector, double> Exploding;
        public event Action<Vector, double> Payload;

        public Missile(Vector from, Vector to, double speed)
        {
            var trail = new Trail(from, to, speed, Colors.Blue, Colors.BlueViolet);
            trail.Moving += pos => Position = pos;
            trail.Completed += Explode;

            AddToParent(trail);
        }

        private void Explode()
        {
            var explosion = new Explosion(Position, 50, 0.25);
            explosion.Exploding += Exploding;
            explosion.Payload += Payload;

            AddToParent(explosion);
            Destroy();
        }
    }
}
