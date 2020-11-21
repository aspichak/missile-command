using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Missile
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
            this.duration = (to - from).Length / speed;

            var color1 = Colors.Orange;
            var color2 = Colors.OrangeRed;
            color1.A = 0;
            brush = new LinearGradientBrush(color1, color2, new Point(from.X, from.Y), new Point(to.X, to.Y));
            brush.MappingMode = BrushMappingMode.Absolute;

            line = new Line();

            line.Stroke = brush;
            line.StrokeThickness = 8;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;

            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = from.X;
            line.Y2 = from.Y;
        }

        public void Update(double dt)
        {
            t += dt;

            line.X2 = Position.X;
            line.Y2 = Position.Y;

            var brushStart = Lerp(from, to, (t - TRAIL_DURATION) / duration);
            var brushEnd   = Lerp(from, to, t / duration);

            brush.StartPoint = new Point(brushStart.X, brushStart.Y);
            brush.EndPoint   = new Point(brushEnd.X, brushEnd.Y);

            if (t >= duration)
            {
                // Destination reached
            }

            if (t >= duration + TRAIL_DURATION)
            {
                // Trail faded completely
            }
        }
    }
}
