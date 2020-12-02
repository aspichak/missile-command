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
        private static TextBlock debugLabel;
        Stopwatch stopwatch { get; } = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            debugLabel = DebugLabel;

            stopwatch.Start();

            CompositionTarget.Rendering += (sender, e) =>
            {
                if (!stopwatch.IsRunning)
                    return;
                Update(stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
            };

            GameGrid.Children.Add(Timer.Repeat(0.25, _ => FpsCounter.Text = $"{fps:0} FPS"));

            var screen = new ScreenManager(new MainMenuScreen());
            GameGrid.Children.Add(screen);
        }

        public static void Debug(string message)
        {
            debugLabel.Text = message;
        }

        private void Update(double dt)
        {
            fps = fps * 0.9 + (1.0 / dt) * 0.1;
        }

        private void CanExecutePauseHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OnPauseHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: set game paused = true
            if (stopwatch.IsRunning)
                stopwatch.Stop();
            else
                stopwatch.Start();
        }
<<<<<<< HEAD

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double scale = Math.Min(GameCanvas.ActualWidth / 1280, GameCanvas.ActualHeight / 720);
            //GameCanvas.RenderTransform = new ScaleTransform(scale, scale);
        }
=======
>>>>>>> 6723f75... Added screen scaling, building layout logic
    }
}
