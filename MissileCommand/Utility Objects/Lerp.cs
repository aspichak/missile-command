using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static MissileCommand.Util;

namespace MissileCommand.Utility_Objects
{
    class Lerp : GameObject
    {
        private Vector from, to;
        private double duration, t;

        public Vector Position => Lerp(from, to, EasingFunction.Invoke(t / duration));
        public double X => Position.X;
        public double Y => Position.Y;
        public Func<double, double> EasingFunction = t => t;
        public event Action Completed;
        public event Action Move;

        public Lerp(Vector from, Vector to, double duration, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            this.from = from;
            this.to = to;
            this.duration = duration;
            this.Move += () => action.Invoke(this);
            this.EasingFunction = easingFunction ?? (t => t);
        }

        public static Lerp Duration(Vector from, Vector to, double duration, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return new Lerp(from, to, duration, action, easingFunction);
        }

        public static Lerp Duration(double from, double to, double duration, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return Duration(new Vector(from, 0), new Vector(to, 0), duration, action, easingFunction);
        }

        public static Lerp Speed(Vector from, Vector to, double speed, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return new Lerp(from, to, from.DistanceTo(to) / speed, action, easingFunction);
        }

        public static Lerp Speed(double from, double to, double speed, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return Speed(new Vector(from, 0), new Vector(to, 0), speed, action, easingFunction);
        }

        //public static Vector Lerp(Vector from, Vector to, double t) => from * (1 - t) + to * t;
        //public static double Lerp(double from, double to, double t) => from * (1 - t) + to * t;

        public static implicit operator Vector(Lerp p)
        {
            return p.Position;
        }

        public static implicit operator Point(Lerp p)
        {
            return p.Position.ToPoint();
        }

        public static implicit operator double(Lerp p)
        {
            return p.Position.X;
        }

        public override void Update(double dt)
        {
            t = Math.Min(t + dt, 1);

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
    }
}
