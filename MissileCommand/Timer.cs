using System;

namespace MissileCommand
{
    class Timer : Sequence
    {
        private double t;
        private bool repeat;

        public double T => t;
        public event Action<double> Updating;

        public Timer(double t, Action action, bool repeat = false)
        {
            this.Duration = t;
            this.repeat = repeat;
            this.Completed += action;
        }

        public static Timer Delay(double t, bool repeat = false)
        {
            return new Timer(t, null, repeat);
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

            if (t >= Duration)
            {
                OnCompleted();

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

        public static implicit operator double(Timer timer) => timer.T;
    }
}
