using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class ScreenFlash : Sequence
    {
        private double strength;

        public ScreenFlash(double strength, double duration)
        {
            this.strength = strength;
            this.Duration = duration;

            Loaded += ScreenFlash_Loaded;
        }

        private void ScreenFlash_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = Parent as Panel;

            while (grid is not Grid)
            {
                grid = (Panel)grid.Parent;
            }

            var rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Colors.White);
            rect.Opacity = strength;
            rect.IsHitTestVisible = false;
            Panel.SetZIndex(rect, 1);

            grid.Children.Add(rect);
<<<<<<< HEAD
            Add(Lerp.Duration(strength, 0, duration, t => rect.Opacity = t));
            Add(Timer.At(duration, () =>
            {
                grid.Children.Remove(rect);
                this.Destroy();
=======

            Add(Lerp.Time(strength, 0, Duration, t => rect.Opacity = t).Then(() =>
            {
                grid.Children.Remove(rect);
                OnCompleted();
                Destroy();
>>>>>>> 6723f75... Added screen scaling, building layout logic
            }));
        }
    }
}
