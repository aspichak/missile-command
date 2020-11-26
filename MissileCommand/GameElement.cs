using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MissileCommand
{
    abstract class GameElement : FrameworkElement
    {
        private static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(Vector), typeof(GameElement));

        private bool active = false;
        private Stopwatch stopwatch = new Stopwatch();

        protected readonly VisualCollection Children;
        protected override int VisualChildrenCount => Children.Count;
        protected override Visual GetVisualChild(int index) => Children[index];

        public bool Active { get => active && !Destroyed; set => active = value; }
        public bool Destroyed { get; private set; }

        public GameElement()
        {
            Children = new VisualCollection(this);
            Loaded += GameElement_Loaded;
            Unloaded += GameElement_Unloaded;
        }

        protected virtual void Update(double dt) { }

        public void AddTo(UIElement element)
        {
            (element as Panel)?.Children.Add(this);
            (element as GameElement)?.Children.Add(this);
        }

        public void Destroy()
        {
            Destroyed = true;
            (Parent as Panel)?.Children.Remove(this);
            (Parent as GameElement)?.Children.Remove(this);
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
                (Parent as Panel)?.Children.Add(element);
                (Parent as GameElement)?.Children.Add(element);
            }
        }

        protected void Remove(UIElement element)
        {
            Children.Remove(element);
        }
        #endregion

        #region Child Element Position
        protected static void SetPosition(UIElement element, Vector position)
        {
            element.SetValue(PositionProperty, position);
        }

        protected static void SetPosition(UIElement element, double X, double Y)
        {
            SetPosition(element, new Vector(X, Y));
        }

        protected static void SetX(UIElement element, double x)
        {
            SetPosition(element, x, GetPosition(element).Y);
        }

        protected static void SetY(UIElement element, double y)
        {
            SetPosition(element, GetPosition(element).X, y);
        }

        protected static Vector GetPosition(UIElement element)
        {
            return (Vector)element.GetValue(PositionProperty);
        }

        protected static double GetX(UIElement element)
        {
            return ((Vector)element.GetValue(PositionProperty)).X;
        }

        protected static double GetY(UIElement element)
        {
            return ((Vector)element.GetValue(PositionProperty)).Y;
        }
        #endregion

        #region FrameworkElement Overrides
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxWidth = 0, maxHeight = 0;

            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);

                var pos = GetPosition(element);
                var size = element.DesiredSize;

                maxWidth = Math.Max(maxWidth, pos.X + size.Width);
                maxHeight = Math.Max(maxHeight, pos.Y + size.Height);
            }

            return new Size(maxWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in Children)
            {
                var pos = GetPosition(element);
                var size = element.DesiredSize;

                element.Arrange(new Rect(pos.X, pos.Y, size.Width, size.Height));
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
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void GameElement_Unloaded(object sender, RoutedEventArgs e)
        {
            Active = false;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }
        #endregion
    }
}
