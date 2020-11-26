using System;

namespace MissileCommand
{
    class Timer : GameElement
    {
        private double t, duration;
        private bool repeat;

        public double T => t;
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

        public static Timer Repeat(double t, Action<Timer> action)
        {
            var timer = new Timer(t, null, true);
            timer.Completed += () => action?.Invoke(timer);
            return timer;
        }

        public static Timer DoUntil(double t, Action<double> action)
        {
            var timer = At(t, null);
            timer.Updating = action;
            return timer;
        }

        protected override void Update(double dt)
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
