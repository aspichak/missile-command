using System;
using System.Windows;
using System.Windows.Media;

namespace MissileCommand
{
    class EnemyMissile : GameElement
    {
        private Trail trail;

        public Vector Position { get; private set; }
        public City Target { get; set; }
        public event Action Exploded;

        public EnemyMissile(Vector from, Vector to, double speed)
        {
            trail = new Trail(from, to, speed, Colors.Orange, Colors.OrangeRed);
            trail.Moving += pos => Position = pos;
            trail.Completed += TargetReached;

            AddToParent(trail);
        }

        public void Explode()
        {
            Exploded?.Invoke();
            trail.Cancel();
            AddToParent(new Explosion(Position, 20, 0.25));
            Destroy();
        }

        public void TargetReached()
        {
            Explode();
            Target?.Explode();
        }
    }
}
