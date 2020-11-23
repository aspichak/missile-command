using System;
using System.Windows.Media;
using static MissileCommand.Util;

namespace MissileCommand
{
    class ScreenShake : GameObject
    {
        private double strength, duration, t;

        public ScreenShake(double strength, double duration)
        {
            this.strength = strength;
            this.duration = duration;
        }

        public override void Update(double dt)
        {
            t = Math.Min(t + dt, duration);
            var shakeStrength = (1 - t / duration) * strength;
            Canvas.RenderTransform = new TranslateTransform(Random(-1, 1) * shakeStrength, Random(-1, 1) * shakeStrength);
        }
    }
}
