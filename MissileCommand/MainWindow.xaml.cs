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
        private LaunchMissileCommand launchCommand1;
        public LaunchMissileCommand LaunchCommand1 { get { return launchCommand1; } }

        private void InitializeCommand()
        {
            launchCommand1 = new LaunchMissileCommand();

            DataContext = this;
            launchCommand1.GestureKey = Key.D1;
        }

        #region WindowStuffs
        public MainWindow()
        {
            InitializeComponent();

            GameObject.Canvas = GameCanvas;
            GameObject.Grid = (Grid)GameCanvas.Parent;

            debugLabel = DebugLabel;
            this.Loaded += MainWindow_Loaded;
            InitializeCommand();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // just delaying some stuff for later and splitting up the function some
            stopwatch.Start();

            CompositionTarget.Rendering += (sender, e) =>
            {
                if (!stopwatch.IsRunning)
                    return;
                Update(stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
            };

            Timer.Repeat(0.25, () => FpsCounter.Text = $"{fps:0} FPS");

            Screen.Switch(new GameScreen());
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // stubbing out now cause will be needed later
            base.OnClosing(e);
        }
        #endregion

        public static void Debug(string message)
        {
            debugLabel.Text = message;
        }

        private void Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (!stopwatch.IsRunning)
            //    return;
            //var pos = Mouse.GetPosition(GameCanvas);
            //new Missile(new(640, 700), new(pos.X, pos.Y), 400);
        }

        private void Update(double dt)
        {
            fps = fps * 0.9 + (1.0 / dt) * 0.1;
            GameObject.UpdateAll(dt);
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double scale = Math.Min(GameCanvas.ActualWidth / 1280, GameCanvas.ActualHeight / 720);
            GameCanvas.RenderTransform = new ScaleTransform(scale, scale);
        }
    }
}
