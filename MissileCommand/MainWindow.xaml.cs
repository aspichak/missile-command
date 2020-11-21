using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace MissileCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = null;
        private Missile missile;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // done loading, start running if needed
            Random rand = new Random();
            // basic logic so it won't start in the exact corners ever
            // we get one corner excluded for "free" so a simple minus one gets both
            int start = rand.Next(Convert.ToInt32(Screen.ActualWidth) - 1) + 1;
            int end = rand.Next(Convert.ToInt32(Screen.ActualWidth) - 1) + 1;
            missile = new Missile(Screen, start, 0, end, Convert.ToInt32(Screen.ActualHeight));
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            missile.Fly();
        }
    }
}
