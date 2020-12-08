using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

            var cross1 = new Line();
            cross1.X1 = to.X - 4;
            cross1.Y1 = to.Y - 4;
            cross1.X2 = to.X + 4;
            cross1.Y2 = to.Y + 4;
            cross1.Stroke = new SolidColorBrush(Colors.BlueViolet);
            cross1.StrokeThickness = 2;

            var cross2 = new Line();
            cross2.X1 = to.X - 4;
            cross2.Y1 = to.Y + 4;
            cross2.X2 = to.X + 4;
            cross2.Y2 = to.Y - 4;
            cross2.Stroke = new SolidColorBrush(Colors.BlueViolet);
            cross2.StrokeThickness = 2;

            var missile = new Image();
            missile.Source = (ImageSource)FindResource("Missile");

            trail.Moving += pos =>
            {
                var matrix = new Matrix();
                matrix.RotateAt(180 - from.AngleTo(to) * 180.0 / Math.PI, missile.Source.Width / 2, 0);
                matrix.Translate(pos.X - missile.Source.Width / 2, pos.Y);
                matrix.TranslatePrepend(0, -missile.Source.Height / 2);
                missile.RenderTransform = new MatrixTransform(matrix);
            };
            Canvas.SetZIndex(trail, -1);

            Add(cross1);
            Add(cross2);
            Add(missile);
            AddToParent(trail);
        }

        private void Explode()
        {
            var explosion = new Explosion(Position, 50, 0.75);
            explosion.Exploding += Exploding;
            explosion.Payload += Payload;

            AddToParent(explosion);
            Destroy();
        }
    }
}
