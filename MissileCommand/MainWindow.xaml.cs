using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MissileCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Missile missile;

        public MainWindow()
        {
            InitializeComponent();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            CompositionTarget.Rendering += (sender, e) =>
            {
                Update(stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
            };

            missile = new Missile(new Vector(50, 0), new Vector(500, 400), 100);
            Screen.Children.Add(missile.Line);
        }

        private void Update(double dt)
        {
            //if (Screen.ActualWidth == 0) return;
            missile.Update(dt);
        }
    }
}
