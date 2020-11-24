using System.Windows;
using System.Windows.Media;

namespace MissileCommand
{
    class EnemyMissile : GameObject
    {
        private Trail trail;

        public Vector Position { get; private set; }

        public EnemyMissile(Vector from, Vector to, double speed)
        {
            trail = new Trail(from, to, speed, Colors.Orange, Colors.OrangeRed);
            trail.Moving += pos => Position = pos;
            trail.Completed += Explode;
        }

        public void Explode()
        {
            new Explosion(Position, 20, 0.25);
            trail.Cancel();
            ScreenEffects.Shake(8, 1);
            this.Destroy();
        }
    }
}
