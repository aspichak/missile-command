using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MissileCommand
{
    class Missile : GameObject
    {
        public Vector Position { get; private set; }

        public Missile(Vector from, Vector to, double speed)
        {
            var trail = new Trail(from, to, speed, Colors.Blue, Colors.BlueViolet);
            trail.Moving += pos => Position = pos;
            trail.Completed += Explode;
        }

        private void Explode()
        {
            new Explosion(Position, 50, 0.25).Exploding += (pos, radius) =>
            {
                Game.OfType<EnemyMissile>().Where(m => m.Position.DistanceTo(pos) <= radius).ForEach(m =>
                {
                    m.Explode();
                    Game.Score += 1;
                });
            };

            this.Destroy();
        }
    }
}
