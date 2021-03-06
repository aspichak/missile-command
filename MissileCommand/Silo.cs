﻿using System;
using System.IO;
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
        private int MaxMissiles;
        private readonly Color ActiveColor = Colors.IndianRed;
        private readonly Color InactiveColor = Colors.AliceBlue;
        public static readonly Size Size = new Size(64, 64);
        private readonly Uri soundShoot = new("file://" + Path.GetFullPath(@"Resources\LASER_SHOT.mp3"));
        private readonly Uri soundExplode = new("file://" + Path.GetFullPath(@"Resources\explosion.mp3"));
        private readonly Uri soundMissileExplode = new("file://" + Path.GetFullPath(@"Resources\MissileBurst1.mp3"));
        MediaPlayer playerShoot = new();
        MediaPlayer playerMissileExplode = new();
        MediaPlayer playerExplode = new();
        // TODO: move text size stuffs here
        #endregion

        private SolidColorBrush colorBrush;
        public event Action<Vector, double> Payload;
        public bool IsDestroyed { get; private set; } = false;
        private bool _OnCooldown = false;
        private double missileSpeed;
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
        private Label missileCountText = new();
        private int _MissileCount;
        public int MissileCount
        {
            get { return _MissileCount; }
            private set
            {
                _MissileCount = value;
                missileCountText.Content = value;
            }
        }

        #region ITargetable contract and inut binding stuff
        public Key GestureKey { get; set; } = Key.None;
        public ModifierKeys GestureModifier { get; set; } = ModifierKeys.None;
        public Vector TargetPosition { get { return new Vector(Canvas.GetLeft(this) + Size.Width / 2, Canvas.GetTop(this)); } }
        public void Explode()
        {
            // does that boom boom thing
            if (IsDestroyed) return;

            IsDestroyed = true;
            playerExplode.Position = System.TimeSpan.Zero;
            playerExplode.Play();

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
            if (MissileCount == 0)
                return;
            MissileCount--;
            playerShoot.Position = System.TimeSpan.Zero;
            playerShoot.Play();
            OnCooldown = true;
            ((Canvas)Parent).Children.Add(Timer.At(cooldownTime, () => { OnCooldown = false; }));
            var pos = Mouse.GetPosition((Canvas)Parent);
            if (pos.X > 0 && pos.Y > 0)
            {
                Point siloPos = this.TransformToAncestor((Canvas)Parent).Transform(new Point(0, 0));
                Missile m = new Missile(new(siloPos.X + (Size.Width / 2), siloPos.Y), new(pos.X, pos.Y), 400 * missileSpeed);
                m.Payload += (position, radius) =>
                {
                    if (playerMissileExplode.Position == System.TimeSpan.Zero)
                        playerMissileExplode.Play();
                    this.Payload?.Invoke(position, radius);
                };
                ((Canvas)Parent).Children.Add(m);
            }
        }

        public bool CanExecute(object parameter) { return !IsDestroyed && !OnCooldown; }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        #endregion

        public Silo(int missileCount, double missileSpeed)
        {
            this.missileSpeed = missileSpeed;
            playerShoot.Open(soundShoot);
            playerExplode.Open(soundExplode);
            playerMissileExplode.MediaEnded += (s, e) =>
            {
                playerMissileExplode.Stop();
                playerMissileExplode.Position = System.TimeSpan.Zero;
            };
            playerMissileExplode.Open(soundMissileExplode);
            MaxMissiles = missileCount;
            MissileCount = MaxMissiles;
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
            missileCountText.FontSize = 30;
            Grid grid = new(); // cause free fill and centering with grid!
            grid.Width = Size.Width;
            grid.Height = Size.Height;
            grid.Children.Add(missileCountText);
            Children.Add(grid);
            Clip = new RectangleGeometry(new(0, 0, Size.Width, Size.Height));
        }
    }
}
