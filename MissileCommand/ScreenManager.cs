using System.Windows;
using System.Windows.Controls;

namespace MissileCommand
{
    class ScreenManager : GameElement
    {
        private const double TRANSITION_DURATION = 0.25;

        public UserControl CurrentScreen { get; private set; }

        public ScreenManager(UserControl screen = null)
        {
            CurrentScreen = screen;
            Add(screen);
        }

        public UserControl Switch(UserControl screen)
        {
            var lastScreen = CurrentScreen;

            CurrentScreen = screen;
            screen.Opacity = 0;

            var animation =
                Lerp.Time(0, 1, TRANSITION_DURATION, t => screen.Opacity = t) *
                Lerp.Time(1, 0, TRANSITION_DURATION, t => lastScreen.Opacity = t) +
                (() => Remove(lastScreen));

            Add(screen);
            Add(animation);

            InvalidateMeasure();

            return screen;
        }

        public UserControl Overlay(UserControl screen)
        {
            screen.Opacity = 0;
            Add(Lerp.Time(0, 1, TRANSITION_DURATION, t => screen.Opacity = t));
            Add(screen);

            return screen;
        }

        public void CloseOverlay(UserControl screen)
        {
            Add(Lerp.Time(1, 0, TRANSITION_DURATION, t => screen.Opacity = t).Then(() => Remove(screen)));
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
