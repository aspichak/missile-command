using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class Missile
    {
        private Vector from, to;
        private double duration, t;
        private Line line;

        public Vector Position => from * (1 - t) + to * t;
        public Line Line => line;

        public Missile(Vector from, Vector to, double speed)
        {
            this.from = from;
            this.to = to;
            this.duration = (to - from).Length / speed;
            
            line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Orange);
            line.StrokeThickness = 4;

            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = to.X;
            line.Y2 = to.Y;

            //var a = new PointAnimation(from, to, TimeSpan.FromSeconds(speed * (to - from).Length));
        }

        public void Update(double dt)
        {
            t = Math.Min(t + dt / duration, 1);

            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = Position.X;
            line.Y2 = Position.Y;

            if (t >= 1)
            {
                // Destination reached
            }
        }
    }
}
