using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class Screen : GameElement
    {
        private const double TRANSITION_DURATION = 0.25;

        public UserControl Current { get; private set; }

        public Screen(UserControl screen = null)
        {
            Current = screen;
            Add(screen);
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
