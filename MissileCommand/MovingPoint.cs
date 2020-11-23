using System;
using System.Windows;
using static MissileCommand.Util;

namespace MissileCommand
{
    class MovingPoint : GameObject
    {
        private Vector from, to;
        private double duration, t;

        public Vector Position => Lerp(from, to, Math.Min(t / duration, 1));
        public event Action Completed;
        public event Action Move;

        public MovingPoint(Vector from, Vector to, double speed, Action<MovingPoint> func = null)
        {
            this.from = from;
            this.to = to;
            this.duration = from.DistanceTo(to) / speed;
            this.Move += () => func.Invoke(this);
        }

        public override void Update(double dt)
        {
            t += dt;

            if (t >= duration)
            {
                Completed?.Invoke();
                this.Destroy();
            }
            else
            {
                Move?.Invoke();
            }
        }

        public void Cancel()
        {
            this.Destroy();
        }

        public static implicit operator Vector(MovingPoint p)
        {
            return p.Position;
        }

        public static implicit operator Point(MovingPoint p)
        {
            return p.Position.ToPoint();
        }
    }
}