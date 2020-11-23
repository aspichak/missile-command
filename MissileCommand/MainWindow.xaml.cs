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
        private double t, fps;
        private Random rand = new Random();
        private static TextBlock debugLabel;

        public MainWindow()
        {
            InitializeComponent();

            debugLabel = DebugLabel;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            CompositionTarget.Rendering += (sender, e) =>
            {
                Update(stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
            };

            GameObject.Initialize(Screen);

            var trail = new Trail(new(0, 0), new(800, 400), 100, Colors.Orange, Colors.OrangeRed);
            new DelayedAction(1.25, () => trail.Cancel(), true);

            new DelayedAction(0.5, () => 
            {
                var x = Random(0, 1280);
                new Trail(new(x, 0), new(x, 720), 50, Colors.Orange, Colors.OrangeRed);
            }, true);

            new DelayedAction(0.25, () => FpsCounter.Text = $"{fps:0} FPS", true);

            //PageFrame.Navigate(new MainMenu());
        }

        public static void Debug(string message)
        {
            debugLabel.Text = message;
        }

        private void Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = Mouse.GetPosition(Screen);
            new Missile(new(640, 700), new(pos.X, pos.Y), 400);
            //Debug("Canvas clicked");
            new ScreenShake(8, 1);
        }

        private void Update(double dt)
        {
            t += dt;
            fps = fps * 0.9 + (1.0 / dt) * 0.1;
            GameObject.UpdateAll(dt);
        }
    }
}
