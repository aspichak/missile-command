using System;
using System.Windows;
using System.Windows.Media;

namespace MissileCommand
{
    class EnemyMissile : GameObject
    {
        private Trail trail;

        public Vector Position { get; private set; }
        public event Action Exploded;

        public EnemyMissile(Vector from, Vector to, double speed)
        {
            trail = new Trail(from, to, speed, Colors.Orange, Colors.OrangeRed);
            trail.Moving += pos => Position = pos;
            trail.Completed += Explode;
        }

        public void Explode()
        {
            Exploded?.Invoke();
            new Explosion(Position, 20, 0.25);
            trail.Cancel();
            Screen.Shake(8, 1);
            this.Destroy();
        }
    }
}
