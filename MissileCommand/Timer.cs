using System;

namespace MissileCommand
{
    class Timer : GameObject
    {
        private double t, duration;
        private bool repeat;

        public event Action Completed;
        public event Action<double> Updating;

        public Timer(double t, Action action, bool repeat = false)
        {
            this.duration = t;
            this.repeat = repeat;
            this.Completed += action;
        }
        public static Timer At(double t, Action action, bool repeat = false)
        {
            return new Timer(t, action, repeat);
        }

        public static Timer Repeat(double t, Action action)
        {
            return At(t, action, true);
        }

        public static Timer DoUntil(double t, Action<double> action)
        {
            var timer = At(t, null);
            timer.Updating = action;
            return timer;
        }

        public override void Update(double dt)
        {
            t += dt;

            if (t >= duration)
            {
                Completed?.Invoke();

                if (repeat)
                {
                    t = 0;
                }
                else
                {
                    this.Destroy();
                }
            }
            else
            {
                Updating?.Invoke(t);
            }
        }

        public void Cancel()
        {
            this.Destroy();
        }
    }
}
