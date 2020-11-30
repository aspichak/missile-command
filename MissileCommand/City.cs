using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class City : GameElement
    {
        private readonly Vector size = new Vector(64, 64);
        private Rectangle rect = new Rectangle();

        public bool IsDestroyed { get; private set; }
        public Vector TargetPosition { get; private set; }

        public City(Vector position) // TODO: City doesn't need to know its position
        {
            this.TargetPosition = position + new Vector(size.X / 2, size.Y - 16);

            rect.Fill = new SolidColorBrush(Colors.AntiqueWhite);
            rect.Width = size.X;
            rect.Height = size.Y;

            SetLeft(this, position.X);
            SetTop(this, position.Y);

            Clip = new RectangleGeometry(new(0, 0, size.X, size.Y));

            Add(rect);
        }

        public void Explode()
        {
            if (IsDestroyed) return;

            IsDestroyed = true;

            var animation = Lerp.Time(0, size.Y - 8, 1.0, t => SetTop(rect, t));

            Add(animation);
            AddToParent(new ScreenShake(10, 1));
        }

        public void Rebuild()
        {
            if (!IsDestroyed) return;

            var animation = Lerp.Time(size.Y - 8, 0, 1.5, t => SetTop(rect, t));
            IsDestroyed = false;
            Add(animation);
        }
    }
}
