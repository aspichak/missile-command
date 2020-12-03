using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class City : GameElement, ITargetable
    {
        private Rectangle rect = new Rectangle();

        public static readonly Size Size = new Size(64, 64);
        public bool IsDestroyed { get; private set; }

        public City()
        {
            rect.Fill = new SolidColorBrush(Colors.AntiqueWhite);
            rect.Width = Size.Width;
            rect.Height = Size.Height;

            Clip = new RectangleGeometry(new(0, 0, Size.Width, Size.Height));

            Add(rect);
        }

        #region ITargetable contract
        public Vector TargetPosition {
            get { return new Vector( Canvas.GetLeft(this) + Size.Width / 2, Canvas.GetTop(this)); }
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
        #endregion
    }
}
