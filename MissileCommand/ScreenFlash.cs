using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace MissileCommand
{
    class ScreenFlash : GameObject
    {
        private const double FLASH_STRENGTH = 0.1;
        private const double FLASH_DURATION = 0.5;
        private const double RECT_MARGIN = 100;
        
        private static double t;
        private static ScreenFlash screenFlash;

        private Rectangle rect;

        private ScreenFlash()
        {
            rect = new Rectangle();

            rect.Fill = new SolidColorBrush(Colors.White);
            rect.Opacity = 0;
            rect.Width = Canvas.ActualWidth + RECT_MARGIN * 2;
            rect.Height = Canvas.ActualHeight + RECT_MARGIN * 2;

            Canvas.SetLeft(rect, -RECT_MARGIN);
            Canvas.SetTop(rect, -RECT_MARGIN);
            Canvas.SetZIndex(rect, -1);

            Canvas.Children.Add(rect);
        }

        public override void Update(double dt)
        {
            t = Math.Min(t + dt, FLASH_DURATION);

            rect.Opacity = (1 - t / FLASH_DURATION) * FLASH_STRENGTH;
            rect.Width = Canvas.ActualWidth + RECT_MARGIN * 2;
            rect.Height = Canvas.ActualHeight + RECT_MARGIN * 2;
        }

        public static void Flash()
        {
            screenFlash ??= new ScreenFlash();
            t = 0;
        }
    }
}
