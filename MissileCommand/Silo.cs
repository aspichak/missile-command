using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MissileCommand
{
    public class Silo : GameElement, ITargetable, ICommand
    {
        #region internal consts
        private double cooldownTime = 0.75;
        private const int MaxMissiles = 9;
        private readonly Color ActiveColor = Colors.IndianRed;
        private readonly Color InactiveColor = Colors.AliceBlue;
        public static readonly Size Size = new Size(64, 64);
        // TODO: move text size stuffs here
        #endregion

        private SolidColorBrush colorBrush;
        public bool IsDestroyed { get; private set; } = false;
        private bool _OnCooldown = false;
        public bool OnCooldown
        {
            get { return _OnCooldown; }
            private set
            {
                _OnCooldown = value;
                if (value == false)
                    colorBrush.Color = ActiveColor;
                else
                    colorBrush.Color = InactiveColor;
            }
        }
        private bool infiniteAmmo = true;
        private Label missileCountText = new();
        private int _MissileCount = MaxMissiles;
        public int MissileCount { get { return _MissileCount; }
            private set {
                _MissileCount = value;
                missileCountText.Content = value;
            }
        }

        #region ITargetable contract and inut binding stuff
        public Key GestureKey { get; set; } = Key.None;
        public ModifierKeys GestureModifier { get; set; } = ModifierKeys.None;
        public Vector TargetPosition { get; }
        public void Explode()
        {
            // does that boom boom thing
            if (IsDestroyed) return;

            IsDestroyed = true;

            var animation = Lerp.Time(0, Size.Height - 8, 1.0,
                t =>
                {
                    Clip = new RectangleGeometry(new(0, 0 + t, Size.Width, Size.Height - t));
                });

            Add(animation);
            AddToParent(new ScreenShake(10, 1));
        }
        public void Rebuild()
        {
            MissileCount = MaxMissiles;
            OnCooldown = false;

            // gets it back up
            if (!IsDestroyed) return;

            var animation = Lerp.Time(0, Size.Height - 8, 1.0,
                t =>
                {
                    Clip = new RectangleGeometry(new(0, Size.Height - 8 - t, Size.Width, Size.Height - 8 + t));
                });

            IsDestroyed = false;
            Add(animation);
        }
        #endregion

        #region ICommand contract
        public void Execute(object parameter)
        {
            if (!infiniteAmmo)
            {
                if (MissileCount == 0)
                    return;
                MissileCount--;
            }
            OnCooldown = true;
            ((Canvas)Parent).Children.Add(Timer.At(cooldownTime, () => { OnCooldown = false; }));
            var pos = Mouse.GetPosition((Canvas)Parent);
            if (pos.X > 0 && pos.Y > 0)
            {
                Point siloPos = this.TransformToAncestor((Canvas)Parent).Transform(new Point(0, 0));
                ((Canvas)Parent).Children.Add(new Missile(new(siloPos.X + (Size.Width / 2), siloPos.Y), new(pos.X, pos.Y), 400));
            }
        }

        public bool CanExecute(object parameter) { return !IsDestroyed && !OnCooldown; }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        #endregion

        public Silo(bool infAmmo = true)
        {
            infiniteAmmo = infAmmo;
            colorBrush = new SolidColorBrush(ActiveColor);

            Pen myPen = new(colorBrush, 1.0);
            GeometryDrawing drawing = new GeometryDrawing();
            drawing.Pen = myPen;
            drawing.Brush = colorBrush;

            PathFigureCollection figCollection = new();
            PathSegmentCollection segCollection = new();
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(0, Size.Height);
            figure.IsClosed = true;

            segCollection.Add(new LineSegment(new Point(Size.Width / 2, 0), true));
            segCollection.Add(new LineSegment(new Point(Size.Width, Size.Height), true));

            figure.Segments = segCollection;
            figCollection.Add(figure);

            PathGeometry geometry = new();
            geometry.Figures = figCollection;
            drawing.Geometry = geometry;

            DrawingBrush drawingBrush = new();
            drawingBrush.Drawing = drawing;
            Background = drawingBrush;

            this.Width = Size.Width;
            this.Height = Size.Height;

            missileCountText.Background = new SolidColorBrush(Colors.Transparent);
            missileCountText.Content = MissileCount;
            missileCountText.HorizontalAlignment = HorizontalAlignment.Center;
            missileCountText.VerticalAlignment = VerticalAlignment.Center;
            Thickness m = new();
            m.Left = (Size.Width - missileCountText.ActualWidth) / 2;
            m.Right = (Size.Width - missileCountText.ActualWidth) / 2;
            m.Top = 10;
            missileCountText.Margin = m;
            missileCountText.FontSize = 30;
            Children.Add(missileCountText);
            Clip = new RectangleGeometry(new(0, 0, Size.Width, Size.Height));
        }
    }
}
