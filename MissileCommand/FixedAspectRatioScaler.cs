using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MissileCommand
{
    class FixedAspectRatioScaler : Canvas
    {
        public Size BaseSize { get; set; }
        public double Scale { get; private set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            Scale = Math.Min(availableSize.Width / BaseSize.Width, availableSize.Height / BaseSize.Height);

            var x = (availableSize.Width - BaseSize.Width * Scale) / 2;
            var y = (availableSize.Height - BaseSize.Height * Scale) / 2;

            var scaleTransform = new ScaleTransform(Scale, Scale);
            var translateTransform = new TranslateTransform(x, y);
            var transformGroup = new TransformGroup();

            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            RenderTransform = transformGroup;

            return BaseSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in Children)
            {
                element.Arrange(new Rect(0, 0, BaseSize.Width, BaseSize.Height));
            }

            return finalSize;
        }
    }
}
