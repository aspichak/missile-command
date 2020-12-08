using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MissileCommand
{
    class EnemyMissile : GameElement
    {
        private Trail trail;

        public Vector Position { get; private set; }
        public ITargetable Target { get; set; }
        public event Action Exploded;
        private Vector From { get; set; }
        private double Speed { get; set; }

        public EnemyMissile(Vector from, double speed)
        {
            Loaded += EnemyMissile_Loaded;
            Speed = speed;
            From = from;
        }

        private void EnemyMissile_Loaded(object sender, RoutedEventArgs e)
        {
            var targets = from item in ((Canvas)Parent).Children.OfType<ITargetable>()
                          where item.IsDestroyed == false
                          select item;
            var target = targets.Random();
            this.Target = target;
            trail = new Trail(From, Target.TargetPosition, Speed, Colors.Orange, Colors.OrangeRed);
            trail.Moving += pos => Position = pos;
            trail.Completed += TargetReached;

            var missile = new Image();
            missile.Source = (ImageSource)FindResource("Missile");

            var matrix = new Matrix();
            matrix.RotateAt(180 - From.AngleTo(Target.TargetPosition) * 180.0 / Math.PI, missile.Source.Width / 2, 0);
            matrix.Translate(From.X - missile.Source.Width / 2, From.Y);
            matrix.TranslatePrepend(0, -missile.Source.Height / 2);
            missile.RenderTransform = new MatrixTransform(matrix);

            trail.Moving += pos =>
            {
                var matrix = new Matrix();
                matrix.RotateAt(180 - From.AngleTo(Target.TargetPosition) * 180.0 / Math.PI, missile.Source.Width / 2, 0);
                matrix.Translate(pos.X - missile.Source.Width / 2, pos.Y);
                matrix.TranslatePrepend(0, -missile.Source.Height / 2);
                missile.RenderTransform = new MatrixTransform(matrix);
            };
            Canvas.SetZIndex(trail, -1);

            Add(missile);
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
