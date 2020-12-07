using MissileCommand.Screens;
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
using static MissileCommand.Util;

namespace MissileCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double fps;
        Stopwatch stopwatch { get; } = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            stopwatch.Start();
            Mouse.OverrideCursor = Cursors.Cross;

            CompositionTarget.Rendering += (sender, e) =>
            {
                if (!stopwatch.IsRunning)
                    return;
                Update(stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
            };

            MainGrid.Children.Add(Timer.Repeat(0.25, _ => FpsCounter.Text = $"{fps:0} FPS"));

            var screen = new ScreenManager(new MainMenuScreen());
            MainGrid.Children.Add(screen);
        }

        private void Update(double dt)
        {
            // https://stackoverflow.com/questions/87304/calculating-frames-per-second-in-a-game
            fps = fps * 0.9 + (1.0 / dt) * 0.1;
        }
    }
}
