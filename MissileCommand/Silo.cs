﻿using MissileCommand.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MissileCommand
{
    public class Silo : GameElement, ITargetable, ICommand
    {
        private bool infiniteAmmo = true;
        private double cooldownTime = 0.75;
        private const int MaxMissiles = 10;
        private SolidColorBrush colorBrush = new SolidColorBrush(Colors.IndianRed);
        public bool IsDestroyed { get; private set; } = false;
        private bool _OnCooldown = false;
        public bool OnCooldown
        {
            get { return _OnCooldown; }
            private set
            {
                _OnCooldown = value;
                if (value == false)
                    colorBrush.Color = Colors.IndianRed;
                else
                    colorBrush.Color = Colors.AliceBlue;
            }
        }
        public int MissileCount { get; private set; } = MaxMissiles;
        public static readonly Size Size = new Size(64, 64);
        public Key GestureKey { get; set; }
        public ModifierKeys GestureModifier { get; set; }

        #region ITargetable contract
        public Vector TargetPosition { get; }
        public void Explode()
        {
            // does that boom boom thing
            if (IsDestroyed) return;

            IsDestroyed = true;

            var animation = Lerp.Time(0, Size.Height - 8, 1.0,
                t =>
                {
                    Clip = new RectangleGeometry(new(0, Clip.Bounds.Y + 1, Size.Width, Clip.Bounds.Height - 1));
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

            var animation = Lerp.Time(0, Size.Height - 7, 1.0,
                t =>
                {
                    Clip = new RectangleGeometry(new(0, Clip.Bounds.Y - 1, Size.Width, Clip.Bounds.Height + 1));
                });

            IsDestroyed = false;
            Add(animation);
            Children.Add(Timer.At(8.0, () => { Clip = new RectangleGeometry(new(0, 0, Size.Width, Size.Height)); }));
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
            TextBox text = new TextBox();
            text.Text = "woof";
            Children.Add(text);
            Clip = new RectangleGeometry(new(0, 0, Size.Width, Size.Height));
        }
    }
}
