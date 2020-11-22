using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Missile : GameObject
    {
        private const double TRAIL_DURATION = 3.0; // in seconds

        private Vector from, to;
        private double duration, t;
        private Line line;
        private LinearGradientBrush brush;

        public Vector Position => Lerp(from, to, Math.Min(t / duration, 1));
        public Line Line => line;

        public Missile(Vector from, Vector to, double speed)
        {
            this.from = from;
            this.to = to;
            this.duration = from.DistanceTo(to) / speed;

            var color1 = Colors.Orange;
            var color2 = Colors.OrangeRed;
            color1.A = 0;
            brush = new LinearGradientBrush(color1, color2, from.ToPoint(), to.ToPoint());
            brush.MappingMode = BrushMappingMode.Absolute;

            line = new Line();

            line.Stroke = brush;
            line.StrokeThickness = 6;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;

            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = from.X;
            line.Y2 = from.Y;

            Canvas.Children.Add(line);
        }

        public override void Update(double dt)
        {
            t += dt;

            line.X2 = Position.X;
            line.Y2 = Position.Y;

            var brushStart = Lerp(from, to, (t - TRAIL_DURATION) / duration);
            var brushEnd   = Lerp(from, to, t / duration);

            brush.StartPoint = brushStart.ToPoint();
            brush.EndPoint = brushEnd.ToPoint();

            if (t >= duration)
            {
                // Destination reached
                //Objects.Add(new Missile(from, to, 100));
            }

            if (t >= duration + TRAIL_DURATION)
            {
                // Trail faded completely
                //Canvas.Children.Remove(line);
                //Destroy(this);
            }
        }
    }
}
