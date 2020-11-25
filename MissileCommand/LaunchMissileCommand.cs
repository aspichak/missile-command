using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MissileCommand
{
    public class LaunchMissileCommand : ICommand
    {
        // Specify the keys and mouse actions that invoke the command. 
        public Key GestureKey { get; set; }
        public ModifierKeys GestureModifier { get; set; }
        public MouseAction MouseGesture { get; set; }

        Action<object> _executeDelegate;

        public LaunchMissileCommand() { }
        /*public LaunchMissileCommand(Action<object> executeDelegate)
        {
            _executeDelegate = executeDelegate;
        }*/

        public void Execute(object parameter)
        {
            //_executeDelegate(parameter);
            const string message = "this is a popup";
            MessageBox.Show(message);
        }

        public bool CanExecute(object parameter) { return true; }
        public event EventHandler CanExecuteChanged;
    }
}
