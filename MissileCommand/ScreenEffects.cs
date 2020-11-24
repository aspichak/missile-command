using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class ScreenEffects : GameObject
    {
        private const double FLASH_STRENGTH = 0.1;
        private const double FLASH_DURATION = 0.5;

        private static Vector shakeOffset = new(0, 0);
        private static Lerp shakeLerp;
        private static Rectangle rect;

        public static void Flash()
        {
            if (rect == null)
            {
                rect = new Rectangle();

                rect.Fill = new SolidColorBrush(Colors.White);
                rect.Opacity = 0;

                ((Grid)Canvas.Parent).Children.Add(rect);
                Grid.SetZIndex(rect, -1);
            }

            Lerp.Duration(FLASH_STRENGTH, 0, FLASH_DURATION, t => rect.Opacity = t);
        }

        public static void Shake(double strength, double duration)
        {
            shakeLerp?.Cancel();

            shakeLerp = Lerp.Duration(strength, 0, duration, t =>
            {
                Matrix matrix = Canvas.RenderTransform.Value;
                matrix.TranslatePrepend(-shakeOffset.X, -shakeOffset.Y);
                shakeOffset = new Vector(Random(-1, 1), Random(-1, 1)) * (double)t;
                matrix.TranslatePrepend(shakeOffset.X, shakeOffset.Y);
                Canvas.RenderTransform = new MatrixTransform(matrix);
            });
        }
    }
}
