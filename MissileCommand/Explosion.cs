using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Explosion : GameObject
    {
        private const double FADE_DURATION = 0.5;
        private const double SHAKE_FACTOR = 1.2;

        public double Radius { get; private set; }

        public Explosion(Vector position, double radius, double duration)
        {
            var circle = new Ellipse();
            circle.Fill = new SolidColorBrush(Colors.White);

            Lerp.Duration(0, radius, duration, r => 
            {
                Radius = r;
                circle.Width = r;
                circle.Height = r;
                Canvas.SetLeft(circle, position.X - r / 2 + Random(-1, 1) * SHAKE_FACTOR);
                Canvas.SetTop(circle, position.Y - r / 2 + Random(-1, 1) * SHAKE_FACTOR);
            }, t => Math.Pow(t, 1.0 / 3.0));

            Timer.DoUntil(duration, _ => Objects.FindAll(o => o is Trail m && m.Position.DistanceTo(position) <= Radius / 2).ForEach(o => ((Trail)o).Cancel()));
            Timer.At(duration, () => Lerp.Duration(1, 0, FADE_DURATION, o => circle.Opacity = o));
            Timer.At(duration + FADE_DURATION, () => this.Destroy());

            Add(circle);
            ScreenFlash.Flash();
        }
    }
}
