using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class Trail : GameObject
    {
        private const double TRAIL_SIZE = 200;
        private Lerp position;

        public Vector Position => position;
        public event Action Completed;

        public Trail(Vector from, Vector to, double speed, Color color1, Color color2)
        {
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

            position = Lerp.Speed(from, to, speed, (p) =>
            {
                line.X2 = p.X;
                line.Y2 = p.Y;
            });

            var brushPosition = Lerp.Speed(from, to + trailVector, speed, (p) =>
            {
                brush.StartPoint = (p - trailVector).ToPoint();
                brush.EndPoint = p;
            });

            position.Completed += () =>
            {
                // Destination reached
                Completed?.Invoke();
            };

            brushPosition.Completed += () =>
            {
                // Trail faded completely
                this.Destroy();
            };

            Add(line);
        }

        public void Cancel()
        {
            position.Cancel();
        }
    }
}
