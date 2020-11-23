using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Trail : GameObject
    {
        private const double TRAIL_SIZE = 200;
        private MovingPoint position;

        public Vector Position => position;
        public event Action Completed;

        public Trail(Vector from, Vector to, double speed, Color color1, Color color2)
        {
            var duration = from.DistanceTo(to) / speed;
            var trailVector = (to - from).Normalized() * TRAIL_SIZE;

            color1.A = 0;
            var brush = new LinearGradientBrush(color1, color2, from.ToPoint(), to.ToPoint());
            brush.MappingMode = BrushMappingMode.Absolute;

            var line = new Line();

            line.Stroke = brush;
            line.StrokeThickness = 2;
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;

            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = from.X;
            line.Y2 = from.Y;

            position = new MovingPoint(from, to, speed, (p) => {
                line.X2 = p.Position.X;
                line.Y2 = p.Position.Y; // TODO: return vector instead?
            });

            var brushPoint = new MovingPoint(from, to + trailVector, speed, (p) => {
                brush.StartPoint = (p - trailVector).ToPoint();
                brush.EndPoint = p;
            });

            position.Completed += () =>
            {
                // Destination reached
                Completed?.Invoke();
            };

            brushPoint.Completed += () =>
            {
                // Trail faded completely
                Canvas.Children.Remove(line);
                this.Destroy();
            };

            Canvas.Children.Add(line);
        }

        public void Cancel()
        {
            position.Cancel();
        }
    }
}
