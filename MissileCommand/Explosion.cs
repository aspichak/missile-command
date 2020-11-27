﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Explosion : GameElement
    {
        private const double FADE_DURATION = 0.5;
        private const double SHAKE_FACTOR = 1.2;

        private Ellipse circle;

        public Vector Position { get; private set; }
        public double Radius { get; private set; }
        public event Action<Vector, double> Exploding;

        public Explosion(Vector position, double radius, double duration)
        {
            circle = new Ellipse();
            circle.Fill = new SolidColorBrush(Colors.White);

            Position = position;

            Add(Lerp.Duration(0, radius, duration, r =>
            {
                Radius = r;
                circle.Width = r * 2;
                circle.Height = r * 2;
                Canvas.SetLeft(circle, position.X - r + Random(-1, 1) * SHAKE_FACTOR);
                Canvas.SetTop(circle, position.Y - r + Random(-1, 1) * SHAKE_FACTOR);
                Exploding?.Invoke(Position, Radius);
            }, Lerp.CubeRoot));

            Add(Timer.At(duration, () => Add(Lerp.Duration(1, 0, FADE_DURATION, o => circle.Opacity = o))));
            Add(Timer.At(duration + FADE_DURATION, () => this.Destroy()));
            Add(circle);
            AddToParent(new ScreenFlash(0.1, 0.5));
        }

        protected override void Update(double dt)
        {
            var transform = new TranslateTransform(Random(-1, 1) * SHAKE_FACTOR, Random(-1, 1) * SHAKE_FACTOR);
            circle.RenderTransform = transform;
        }
    }
}
