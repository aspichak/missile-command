using MissileCommand.Screens;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MissileCommand
{
    public class LaunchMissileCommand : ICommand
    {
        // Specify the keys and mouse actions that invoke the command. 
        public Key GestureKey { get; set; }
        public ModifierKeys GestureModifier { get; set; }
        public MouseAction MouseGesture { get; set; }
        private GameScreen screen;

        public LaunchMissileCommand(GameScreen s) {
            screen = s;
        }

        public void Execute(object parameter)
        {
            if (screen.Paused) return;
            var pos = Mouse.GetPosition(screen.GameScreenGrid);
            if (pos.X > 0 && pos.Y > 0)
                new Missile(new(640, 700), new(pos.X, pos.Y), 400);
        }

        public bool CanExecute(object parameter) { return true; }
        public event EventHandler CanExecuteChanged;
    }
}
