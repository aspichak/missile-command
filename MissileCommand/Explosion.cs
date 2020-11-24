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
        private const double SHAKE_FACTOR = 1.2;

        public Vector Position { get; private set; }
        public double Radius { get; private set; }
        public event Action<Vector, double> Exploding;

        public Explosion(Vector position, double radius, double duration)
        {
            var circle = new Ellipse();
            circle.Fill = new SolidColorBrush(Colors.White);

            Position = position;

            Lerp.Duration(0, radius, duration, r =>
            {
                Radius = r;
                circle.Width = r * 2;
                circle.Height = r * 2;
                Canvas.SetLeft(circle, position.X - r + Random(-1, 1) * SHAKE_FACTOR);
                Canvas.SetTop(circle, position.Y - r + Random(-1, 1) * SHAKE_FACTOR);
                Exploding?.Invoke(Position, Radius);
            }, Lerp.CUBE_ROOT);

            Timer.At(duration, () => Lerp.Duration(1, 0, FADE_DURATION, o => circle.Opacity = o));
            Timer.At(duration + FADE_DURATION, () => this.Destroy());

            Add(circle);
            ScreenEffects.Flash();
        }
    }
}
