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
        //private Rectangle rect = new Rectangle();
        private Polygon poly = new Polygon();
        public bool IsDestroyed { get; private set; } = false;
        public bool OnCooldown { get; private set; } = false;
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

            var animation = Lerp.Time(0, Size.Height - 8, 1.0, t => SetTop(poly, t));

            Add(animation);
            AddToParent(new ScreenShake(10, 1));
        }
        public void Rebuild()
        {
            MissileCount = MaxMissiles;
            // gets it back up
            if (!IsDestroyed) return;

            var animation = Lerp.Time(Size.Height - 8, 0, 1.5, t => SetTop(poly, t));
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
                ((Canvas)Parent).Children.Add(new Missile(new(siloPos.X, siloPos.Y), new(pos.X, pos.Y), 400));
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

            poly.Fill = new SolidColorBrush(Colors.IndianRed);
            poly.Width = Size.Width;
            poly.Height = Size.Height;
            PointCollection pc = new();
            pc.Add(new(0, Size.Height));
            pc.Add(new(Size.Width / 2, 0));
            pc.Add(new(Size.Width, Size.Height));
            poly.Points = pc;

            Clip = new RectangleGeometry(new(0, 0, Size.Width, Size.Height));

            Add(poly);
        }
    }
}
