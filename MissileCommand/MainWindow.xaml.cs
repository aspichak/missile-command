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
        private Game game;
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

            game = new Game(GameCanvas);

            var trail = new Trail(new(0, 0), new(800, 400), 100, Colors.Orange, Colors.OrangeRed);
            Timer.At(1.25, () => trail.Cancel(), true);

            Timer.Repeat(0.5, () => 
            {
                var x = Random(0, 1280);
                new EnemyMissile(new(x, 0), new(x, 720), 50);
            });

            Timer.Repeat(0.25, () => FpsCounter.Text = $"{fps:0} FPS");

            //Screen.Switch(new MainMenu());
        }

        public static void Debug(string message)
        {
            debugLabel.Text = message;
        }

        private void Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!stopwatch.IsRunning)
                return;
            var pos = Mouse.GetPosition(GameCanvas);
            new Missile(new(640, 700), new(pos.X, pos.Y), 400);
        }

        private void Update(double dt)
        {
            fps = fps * 0.9 + (1.0 / dt) * 0.1;
            game.Update(dt);
        }

        private void CanExecutePauseHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void OnPauseHandler(object sender, ExecutedRoutedEventArgs e)
        {
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
