using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MissileCommand.Util;

namespace MissileCommand
{
    class ScreenShake : Sequence
    {
        private double strength, duration;
        private Vector shakeOffset;

        public ScreenShake(double strength, double duration)
        {
            this.strength = strength;
            this.duration = duration;

            Loaded += ScreenShake_Loaded;
        }

        private void ScreenShake_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = Parent as Panel;

            Add(Lerp.Duration(strength, 0, duration, t =>
            {
                Matrix matrix = parent.RenderTransform.Value;
                matrix.TranslatePrepend(-shakeOffset.X, -shakeOffset.Y);
                shakeOffset = new Vector(Random(-1, 1), Random(-1, 1)) * (double)t;
                matrix.TranslatePrepend(shakeOffset.X, shakeOffset.Y);
                parent.RenderTransform = new MatrixTransform(matrix);
<<<<<<< HEAD
            }));

            Add(Timer.At(duration, () => this.Destroy()));
=======
            }).Then(() =>
            {
                OnCompleted();
                Destroy();
            }));
>>>>>>> 6723f75... Added screen scaling, building layout logic
        }
    }
}
