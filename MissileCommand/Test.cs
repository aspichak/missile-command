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

            this.Unloaded += Test_Unloaded;
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
            // TODO: This is probably bad. We don't necessarily know if the parent is a Canvas.
            ((Canvas)Parent).Children.Remove(this);
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
            // TODO: Fix this
            double maxWidth = 0, maxHeight = 0;

            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
                var pos = GetPosition(element);
                maxWidth = Math.Max(maxWidth, element.DesiredSize.Width + pos.X);
                maxHeight = Math.Max(maxHeight, element.DesiredSize.Height + pos.Y);
            }

            return new Size(maxWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // TODO: Fix this
            double maxWidth = 0, maxHeight = 0;

            foreach (UIElement element in Children)
            {
                //element.Arrange(new Rect(20,20,50,50));
                var pos = GetPosition(element);
                //element.Arrange(new Rect(pos.X, pos.Y, element.DesiredSize.Width, element.DesiredSize.Height));
                element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));
                maxWidth = Math.Max(maxWidth, element.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
            }

            return new Size(0, 0);
        }
    }
}
