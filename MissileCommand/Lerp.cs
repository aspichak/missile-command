using System;
using System.Windows;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Lerp : Sequence
    {
        public static readonly Func<double, double> Linear = t => t;
        public static readonly Func<double, double> CubeRoot = t => Math.Pow(t, 1.0 / 3.0);
        public static readonly Func<double, double> Sine = t => Math.Sin(t * Math.PI / 2.0);

        private Vector from, to;
        private double t;

        public Vector Position => Lerp(from, to, EasingFunction.Invoke(t / Duration));
        public double X => Position.X;
        public double Y => Position.Y;
        public Func<double, double> EasingFunction = t => t;
        public event Action<Lerp> Move;

        public Lerp(Vector from, Vector to, double duration, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            this.from = from;
            this.to = to;
            this.Duration = duration;
            this.Move += action;
            this.EasingFunction = easingFunction ?? Linear;
        }

        public static Lerp Time(Vector from, Vector to, double duration, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return new Lerp(from, to, duration, action, easingFunction);
        }

        public static Lerp Time(double from, double to, double duration, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return Time(new Vector(from, 0), new Vector(to, 0), duration, action, easingFunction);
        }

        public static Lerp Speed(Vector from, Vector to, double speed, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return new Lerp(from, to, from.DistanceTo(to) / speed, action, easingFunction);
        }

        public static Lerp Speed(double from, double to, double speed, Action<Lerp> action = null, Func<double, double> easingFunction = null)
        {
            return Speed(new Vector(from, 0), new Vector(to, 0), speed, action, easingFunction);
        }

        public static implicit operator Vector(Lerp lerp)
        {
            return lerp.Position;
        }

        public static implicit operator Point(Lerp lerp)
        {
            return lerp.Position.ToPoint();
        }

        public static implicit operator double(Lerp lerp)
        {
            return lerp.Position.X;
        }

        protected override void Update(double dt)
        {
            t = Math.Min(t + dt, Duration);
            Move?.Invoke(this);

            if (t >= Duration)
            {

                OnCompleted();
                this.Destroy();
            }
        }

        public void Cancel()
        {
            this.Destroy();
        }
    }
}
