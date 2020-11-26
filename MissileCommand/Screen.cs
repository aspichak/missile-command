using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class Screen : GameElement
    {
        private const double FLASH_STRENGTH = 0.1;
        private const double FLASH_DURATION = 0.5;
        private const double TRANSITION_DURATION = 0.25;

        private static Vector shakeOffset = new(0, 0);
        private static Lerp shakeLerp;
        private static Rectangle flashRect;

        public UserControl Current { get; private set; }

        public Screen(UserControl screen = null)
        {
            Current = screen;
            Add(screen);

            if (flashRect == null)
            {
                flashRect = new Rectangle();

                flashRect.Fill = new SolidColorBrush(Colors.White);
                flashRect.Opacity = 0;
                Panel.SetZIndex(flashRect, -1);

                AddToParent(flashRect);
            }
        }

        public UserControl Switch(UserControl screen)
        {
            var lastScreen = Current;

            Current = screen;
            screen.Opacity = 0;
            Add(screen);

            Add(Lerp.Duration(0, 1, TRANSITION_DURATION, t => screen.Opacity = t));

            if (lastScreen != null)
            {
                Add(Lerp.Duration(1, 0, TRANSITION_DURATION, t => lastScreen.Opacity = t));
            }

            Add(Timer.At(TRANSITION_DURATION, () => Remove(lastScreen)));

            InvalidateMeasure();

            return screen;
        }

        public UserControl Overlay(UserControl screen)
        {
            screen.Opacity = 0;
            Add(Lerp.Duration(0, 1, TRANSITION_DURATION, t => screen.Opacity = t));
            Add(screen);

            return screen;
        }

        public void CloseOverlay(UserControl screen)
        {
            Add(Lerp.Duration(1, 0, TRANSITION_DURATION, t => screen.Opacity = t));
            Add(Timer.At(TRANSITION_DURATION, () => Remove(screen)));
        }

        public static void Flash()
        {
            Lerp.Duration(FLASH_STRENGTH, 0, FLASH_DURATION, t => flashRect.Opacity = t);
        }

        public static void Shake(double strength, double duration)
        {
            shakeLerp?.Cancel();

            shakeLerp = Lerp.Duration(strength, 0, duration, t =>
            {
                // TODO: Fix shake effect
                //Matrix matrix = RenderTransform.Value;
                //matrix.TranslatePrepend(-shakeOffset.X, -shakeOffset.Y);
                //shakeOffset = new Vector(Random(-1, 1), Random(-1, 1)) * (double)t;
                //matrix.TranslatePrepend(shakeOffset.X, shakeOffset.Y);
                //RenderTransform = new MatrixTransform(matrix);
            });

            //Add(shakeLerp);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in Children)
            {
                element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }
    }
}
