using System;
using System.Windows;
using System.Windows.Media;

namespace MissileCommand
{
    class EnemyMissile : GameElement
    {
        private Trail trail;

        public Vector Position { get; private set; }
<<<<<<< HEAD
=======
        public City Target { get; set; }
>>>>>>> 6723f75... Added screen scaling, building layout logic
        public event Action Exploded;

        public EnemyMissile(Vector from, Vector to, double speed)
        {
            trail = new Trail(from, to, speed, Colors.Orange, Colors.OrangeRed);
            trail.Moving += pos => Position = pos;
            trail.Completed += Explode;
            AddToParent(trail);
        }

        public void Explode()
        {
            Exploded?.Invoke();
            trail.Cancel();
            AddToParent(new Explosion(Position, 20, 0.25));
            Destroy();
        }
    }
}
