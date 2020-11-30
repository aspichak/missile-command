using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MissileCommand
{
    class Silo : GameElement, ITargetable, ICommand
    {
        private bool infiniteAmmo;
        private bool destroyed = false;
        private Vector Position { get; set; }

        #region ITargetable contract
        public Vector TargetPosition { get; }
        public void Explode()
        {
            // does that boom boom thing
            destroyed = true;
        }
        public void Rebuild()
        {
            // gets it back up
            destroyed = false;
        }
        #endregion
        #region ICommand contract
        public void Execute(object parameter)
        {
            /*if (screen.Paused) return;
            var pos = Mouse.GetPosition(screen.GameScreenGrid);
            if (pos.X > 0 && pos.Y > 0)
                new Missile(new(X, Y), new(pos.X, pos.Y), 400);*/
        }

        public bool CanExecute(object parameter) { return !destroyed; }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        #endregion

        public Silo(Vector position, bool infAmmo = false)
        {
            Position = position;
            infiniteAmmo = infAmmo;
        }
    }
}
