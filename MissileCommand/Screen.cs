using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static MissileCommand.Util;

namespace MissileCommand
{
    class Screen : GameObject
    {
        private const double FLASH_STRENGTH = 0.1;
        private const double FLASH_DURATION = 0.5;
        private const double TRANSITION_DURATION = 0.25;

        private static Vector shakeOffset = new(0, 0);
        private static Lerp shakeLerp;
        private static Rectangle rect;

        public static UserControl Current { get; private set; }

        public static UserControl Switch(UserControl screen)
        {
            var grid = (Grid)Canvas.Parent;
            var lastScreen = Current;

            Current = screen;
            screen.Opacity = 0;
            grid.Children.Add(screen);

            Lerp.Duration(0, 1, TRANSITION_DURATION, t => screen.Opacity = t);

            if (lastScreen != null)
            {
                Lerp.Duration(1, 0, TRANSITION_DURATION, t => lastScreen.Opacity = t);
            }

            Timer.At(TRANSITION_DURATION, () => ((Grid)Canvas.Parent).Children.Remove(lastScreen));

            return screen;
        }

        public static UserControl Overlay(UserControl screen)
        {
            var grid = (Grid)Canvas.Parent;

            screen.Opacity = 0;
            Lerp.Duration(0, 1, TRANSITION_DURATION, t => screen.Opacity = t);
            grid.Children.Add(screen);

            return screen;
        }

        public static void CloseOverlay(UserControl screen)
        {
            var grid = (Grid)Canvas.Parent;

            Lerp.Duration(1, 0, TRANSITION_DURATION, t => screen.Opacity = t);
            Timer.At(TRANSITION_DURATION, () => grid.Children.Remove(screen));
        }

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
