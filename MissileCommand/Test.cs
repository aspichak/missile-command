using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    class Test : FrameworkElement
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(Vector), typeof(Test));

        private int i;
        protected readonly VisualCollection Children;
        public static Canvas canvas;
        public double Xyz;
        public Vector Foo { get; set; }

        public Test()
        {
            var r = new Rectangle();
            r.Width = 203;
            r.Height = 304;
            r.Fill = new SolidColorBrush(Colors.BlueViolet);
            SetPosition(r, new(400, 200));

            //CompositionTarget.Rendering += CompositionTarget_Rendering;
            Children = new VisualCollection(this);
            Children.Add(r);

            this.Loaded += Test_Loaded;
            this.Unloaded += Test_Unloaded;
        }

        private void Test_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected static void SetPosition(UIElement element, Vector position)
        {
            element.SetValue(PositionProperty, position);
        }

        protected static void SetPosition(UIElement element, double X, double Y)
        {
            SetPosition(element, new Vector(X, Y));
        }

        protected static Vector GetPosition(UIElement element)
        {
            return (Vector)element.GetValue(PositionProperty);
        }

        protected void Add(UIElement element)
        {
            Children.Add(element);
        }

        public void Destroy()
        {
            (Parent as Panel).Children.Remove(this);
        }

        private void Test_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            MainWindow.Debug((i++).ToString());
        }

        protected override int VisualChildrenCount
        {
            get { return Children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return Children[index];
        }

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

            //if (double.IsNaN(finalSize.Width) || double.IsNaN(finalSize.Height) || finalSize.IsEmpty)
            //{
            //    return new Size(0, 0);
            //}

            return finalSize;
        }
    }
}
