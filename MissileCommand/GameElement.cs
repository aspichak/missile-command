using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MissileCommand
{
    abstract class GameElement : Canvas
    {
        private bool active = false;
        private Stopwatch stopwatch = new Stopwatch();

        public bool Active { 
            get => active && !Destroyed;
            set
            {
                if (!active && value) CompositionTarget.Rendering += CompositionTarget_Rendering;
                if (active && !value) CompositionTarget.Rendering -= CompositionTarget_Rendering;
                active = value;
            }
        }
        public bool Destroyed { get; private set; }

        public GameElement()
        {
            Loaded += GameElement_Loaded;
            Unloaded += GameElement_Unloaded;
        }

        protected virtual void Update(double dt) { }

        public void AddTo(Panel element)
        {
            element.Children.Add(this);
        }

        public void Destroy()
        {
            Destroyed = true;
            (Parent as Panel)?.Children.Remove(this);
        }

        #region Add / Remove Element Children
        protected void Add(UIElement element)
        {
            if (element == null) return;
            Children.Add(element);
        }

        protected void AddToParent(UIElement element)
        {
            if (element == null) return;

            if (Parent == null)
            {
                Loaded += (_, _) => AddToParent(element);
            }
            else
            {
                (Parent as Panel).Children.Add(element);
            }
        }

        protected void Remove(UIElement element)
        {
            Children.Remove(element);
        }
        #endregion

        #region FrameworkElement Overrides
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxWidth = 0, maxHeight = 0;

            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);

                var x = Canvas.GetLeft(element).OrZero();
                var y = Canvas.GetTop(element).OrZero();

                maxWidth = Math.Max(maxWidth, x + element.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, y + element.DesiredSize.Height);
            }

            return new Size(maxWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in Children)
            {
                var x = Canvas.GetLeft(element).OrZero();
                var y = Canvas.GetTop(element).OrZero();
                var size = element.DesiredSize;

                element.Arrange(new Rect(x, y, size.Width, size.Height));
            }

            return finalSize;
        }
        #endregion

        #region Event Handlers
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (!Active) return;
            Update(stopwatch.Elapsed.TotalSeconds);
            stopwatch.Restart();
        }

        private void GameElement_Loaded(object sender, RoutedEventArgs e)
        {
            Active = true;
        }

        private void GameElement_Unloaded(object sender, RoutedEventArgs e)
        {
            Active = false;
        }
        #endregion
    }
}
