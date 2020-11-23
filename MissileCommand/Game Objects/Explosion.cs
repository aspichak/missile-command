using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;
using MissileCommand.Utility_Objects;

namespace MissileCommand
{
    class Explosion : GameObject
    {
        private const double FADE_DURATION = 0.5;
        private const double SHAKE_FACTOR = 1.2;

        private double size, t, duration;
        private Vector position;
        private Ellipse circle;

        public Vector Position => position;
        //public double Radius => Lerp(0, size, Math.Pow(Math.Min(t / duration, 1), 1.0/3.0));
        public double Radius { get; private set; }
        public event Action Faded;

        public Explosion(Vector position, double size, double speed)
        {
            this.size = size;
            this.position = position;
            this.duration = size / speed;
            
            circle = new Ellipse();
            circle.Fill = new SolidColorBrush(Colors.White);

            // Explode
            Utility_Objects.Lerp.Speed(0, size, speed, radius =>
            {
                Radius = radius;
                circle.Width = Radius;
                circle.Height = Radius;
                Canvas.SetLeft(circle, position.X - Radius / 2 + Random(-1, 1) * SHAKE_FACTOR);
                Canvas.SetTop(circle, position.Y - Radius / 2 + Random(-1, 1) * SHAKE_FACTOR);
            }, t => Math.Pow(t, 1.0 / 3.0));

            // Fade
            Timer.At(duration, () => Utility_Objects.Lerp.Duration(1, 0, FADE_DURATION, (opacity) => circle.Opacity = opacity));

            // Clean up
            Timer.At(duration + FADE_DURATION, () =>
            {
                Faded?.Invoke();
                this.Destroy();
            });

            Add(circle);
            //ScreenEffects.Flash();
        }

        public override void Update(double dt)
        {
            t += dt;

            circle.Width = Radius;
            circle.Height = Radius;
            Canvas.SetLeft(circle, position.X - Radius / 2 + Random(-1, 1) * SHAKE_FACTOR);
            Canvas.SetTop(circle, position.Y - Radius / 2 + Random(-1, 1) * SHAKE_FACTOR);

            if (t <= duration)
            {
                //foreach (var o in Objects)
                //{
                //    if (o is Trail && ((Trail)o).Position.DistanceTo(Position) <= Radius / 2)
                //    {
                //        ((Trail)o).Cancel();
                //    }
                //}
            }

            if (t >= duration)
            {
                circle.Opacity = Lerp(1, 0, (t - duration) / FADE_DURATION);
            }

            if (t >= duration + FADE_DURATION)
            {
                Faded?.Invoke();
                this.Destroy();
            }

            //GameObject.Objects.FindAll((o) => o is Missile && ((Missile)o).Position.DistanceTo(position) <= Radius).ForEach((o) => (Missile)o.Explode());
        }
    }
}
