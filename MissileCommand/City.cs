using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MissileCommand.Util;

namespace MissileCommand
{
    class City : GameElement, ITargetable
    {
        private Image city = new Image();
        private Image rubble = new Image();

        public static Size Size = new Size(100, 100);
        public bool IsDestroyed { get; private set; }

        public City()
        {
            city.Source = (ImageSource)FindResource("City");
            rubble.Source = (ImageSource)FindResource("Rubble");

            Size = new(city.Source.Width, city.Source.Height);
            this.Width = city.Source.Width;
            this.Height = city.Source.Height;

            var canvas = new Canvas();

            canvas.Clip = new RectangleGeometry(new(0, 0, Width, Height));
            canvas.Children.Add(city);
            canvas.Children.Add(rubble);

            Add(canvas);
        }

        #region ITargetable contract
        public Vector TargetPosition {
            get { return new Vector( Canvas.GetLeft(this) + Size.Width / 2, Canvas.GetTop(this)); }
        }

        public void Explode()
        {
            if (IsDestroyed) return;

            IsDestroyed = true;

            Sequence animation =
                Lerp.Time(0, Height, 1.0, t => SetTop(city, t)) *
                Timer.At(0.2, () => Add(new Explosion(new(20, 20), 10, 0.25))) *
                Timer.At(0.4, () => Add(new Explosion(new(80, 25), 16, 0.25))) *
                Timer.At(0.6, () => Add(new Explosion(new(50, 16), 12, 0.25)));

            Add(animation);
            Add(new ScreenShake(8, 1.5));
            AddToParent(new ScreenShake(8, 1));
        }

        public void Rebuild()
        {
            if (!IsDestroyed) return;

            var animation =
                Lerp.Time(Height, 0, 1.0 + Random(-0.5, 0.5), t => SetTop(city, t), Lerp.Sine) *
                (Timer.Delay(0.25) +
                Lerp.Speed(0, 12 * Math.PI / 2, Math.PI * 4, t => city.Opacity = Math.Cos(t)));

            IsDestroyed = false;
            Add(animation);
        }
        #endregion
    }
}
