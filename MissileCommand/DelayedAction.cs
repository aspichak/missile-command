using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissileCommand
{
    class DelayedAction : GameObject
    {
        private double t, duration;
        private Action action;
        private bool repeat;

        public DelayedAction(double t, Action action, bool repeat = false)
        {
            this.duration = t;
            this.action = action;
            this.repeat = repeat;
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
