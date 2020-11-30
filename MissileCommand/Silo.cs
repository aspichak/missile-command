using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    // TODO: Refactor
    class Silo : GameElement
    {
        private Rectangle rect = new Rectangle();

        public static readonly Size Size = new Size(64, 64);
        public bool IsDestroyed { get; private set; }

        public Silo()
        {
            rect.Fill = new SolidColorBrush(Colors.IndianRed);
            rect.Width = Size.Width;
            rect.Height = Size.Height;

            Clip = new RectangleGeometry(new(0, 0, Size.Width, Size.Height));

            Add(rect);
        }

        public void Explode()
        {
            if (IsDestroyed) return;

            IsDestroyed = true;

            var animation = Lerp.Time(0, Size.Height - 8, 1.0, t => SetTop(rect, t));

            Add(animation);
            AddToParent(new ScreenShake(10, 1));
        }

        public void Rebuild()
        {
            if (!IsDestroyed) return;

            var animation = Lerp.Time(Size.Height - 8, 0, 1.5, t => SetTop(rect, t));
            IsDestroyed = false;
            Add(animation);
        }
    }
}
