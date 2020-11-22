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

            new DelayedAction(1.0, () => new Trail(new(Random(0, 800), Random(0, 400)), new(Random(0, 800), Random(0, 400)), Random(50, 500)), true);
            new DelayedAction(0.25, () => FpsCounter.Text = $"{fps:0} FPS", true);
        }

        public static void Debug(string message)
        {
            debugLabel.Text = message;
        }

        private void Update(double dt)
        {
            t += dt;
            fps = fps * 0.9 + (1.0 / dt) * 0.1;
            GameObject.UpdateAll(dt);
        }
    }
}
