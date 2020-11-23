using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand.Utility_Objects
{
    class Timer : GameObject
    {
        private double t, duration;
        private Action action;
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

        public static Timer Every(double t, Action action)
        {
            return At(t, action, true);
        }

        public static Timer For(double t, Action<double> action)
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
                action?.Invoke();

                if (repeat)
                {
                    t = 0;
                }
                else
                {
                    this.Destroy();
                }
            }
        }

        public void Cancel()
        {
            this.Destroy();
        }
    }
}
