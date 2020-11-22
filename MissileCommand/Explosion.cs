﻿using System;
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
        private const double SHAKE_FACTOR = 1.0;

        private double size, t, duration;
        private Vector position;
        private Ellipse circle;

        public double Radius => Lerp(0, size, Math.Sqrt(Math.Min(t / duration, 1)));

        public Explosion(Vector position, double size, double speed)
        {
            this.size = size;
            this.position = position;
            this.duration = size / speed;
            circle = new Ellipse();

            circle.Fill = new SolidColorBrush(Colors.White);

            Canvas.Children.Add(circle);
        }

        public override void Update(double dt)
        {
            t += dt;

            circle.Width = Radius;
            circle.Height = Radius;
            Canvas.SetLeft(circle, position.X - Radius / 2 + Random(-1, 1) * SHAKE_FACTOR);
            Canvas.SetTop(circle, position.Y - Radius / 2 + Random(-1, 1) * SHAKE_FACTOR);

            if (t >= duration)
            {
                circle.Opacity = Lerp(1, 0, (t - duration) / FADE_DURATION);
            }

            if (t >= duration + FADE_DURATION)
            {
                Canvas.Children.Remove(circle);
                this.Destroy();
            }

            //GameObject.Objects.FindAll((o) => o is Missile && ((Missile)o).Position.DistanceTo(position) <= Radius).ForEach((o) => (Missile)o.Explode());
            MainWindow.Debug(Radius.ToString());
        }
    }
}
