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
using static MissileCommand.Util;

namespace MissileCommand.Screens
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : UserControl
    {
        public int Score { get; set; }
        public int Round { get; private set; } = 1;
        public bool Paused { get; set; }
        private LaunchMissileCommand launchCommand1;
        public LaunchMissileCommand LaunchCommand1 { get { return launchCommand1; } }

        private void InitializeCommand()
        {
            launchCommand1 = new LaunchMissileCommand(this);

            DataContext = this;
            launchCommand1.GestureKey = Key.D1;
        }

        public GameScreen()
        {
            InitializeComponent();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
            InitializeCommand();

            GameObject.Game = this;
            StartRound();
        }

        public void StartRound()
        {
            // TODO: "Round X" text
            // TODO: Create cities

            var t = 0.0;

            for (int i = 0; i < 5; i++)
            {
                t += Random(1, 3);
                var speed = Random(50, 80);

                Timer.At(t, () => 
                {
                    var missile = new EnemyMissile(new(Random(0, 1280), 0), new(Random(0, 1280), 720), speed);
                });
            }
        }

        public void EndRound()
        {
            Round++;
            StartRound();
            MainWindow.Debug(Round.ToString());
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Paused) return;
            var pos = Mouse.GetPosition((Grid)sender);
            new Missile(new(640, 700), new(pos.X, pos.Y), 400);
        }
    }
}
