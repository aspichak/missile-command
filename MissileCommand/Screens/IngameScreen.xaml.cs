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
    /// Interaction logic for IngameScreen.xaml
    /// </summary>
    public partial class IngameScreen : UserControl
    {
        private readonly List<EnemyMissile> enemies = new List<EnemyMissile>();
        private int score = 0;

        public int Score 
        {
            get => score;
            set
            {
                score = value;
                ScoreLabel.Text = $"{score}";
            }
        }

        public int Wave { get; private set; } = 1;
        public bool Paused { get; set; }

        public IngameScreen()
        {
            InitializeComponent();

            Focusable = true;
            Loaded += (_, _) => Keyboard.Focus(this);
            StartRound();
        }

        private void Add(UIElement element)
        {
            GameCanvas.Children.Add(element);
        }

        private void StartRound()
        {
            WaveLabel.Opacity = 0;
            WaveLabel.Text = $"Wave {Wave}";

            Add(Timer.At(1, () =>
            {
                var lerp = Lerp.Duration(0, 1, 1, t =>
                {
                    WaveLabel.Opacity = t;
                    WaveLabel.Margin = new Thickness(0, 160 - t * 60, 0, 0);
                    MainWindow.Debug(((double)t).ToString());
                }, Lerp.Sine).Then(() =>
                {
                    Add(Timer.At(1, () =>
                    {
                        Add(Lerp.Duration(1, 0, 1, t1 =>
                        {
                            double t = t1;
                            WaveLabel.Opacity = t;
                            WaveLabel.Margin = new Thickness(0, 100 + (1 - t1) * 60, 0, 0);
                        }, Lerp.Sine));
                    }));
                });

                Add(lerp);
            }));

            Add(Timer.At(3.0, () =>
            {
                // TODO: Create cities

                var t = 0.0;

                for (int i = 0; i < 5; i++)
                {
                    t += Random(1, 3);
                    var speed = Random(50, 80);

                    var missile = new EnemyMissile(new(Random(0, 1280), 0), new(Random(0, 1280), 720), speed);
                    enemies.Add(missile);

                    Add(Timer.At(t, () => GameCanvas.Children.Add(missile)));
                }

                Add(Timer.Repeat(0.25, timer =>
                {
                    if (enemies.All(m => m.Destroyed))
                    {
                        enemies.Clear();
                        EndRound();
                        timer.Destroy();
                    }
                }));

            }));

        }

        private void EndRound()
        {
            Wave++;
            StartRound();
            MainWindow.Debug(Wave.ToString());
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Paused) return;
            var pos = Mouse.GetPosition(GameCanvas);
            var missile = new Missile(new(640, 700), new(pos.X, pos.Y), 1000);

            missile.Exploding += (pos, radius) =>
            {
                foreach (var enemy in enemies)
                {
                    if (enemy.Active && enemy.Position.DistanceTo(pos) <= radius)
                    {
                        enemy.Explode();
                        Score += 1;
                        MainWindow.Debug(score.ToString());
                    }
                }
            };

            GameCanvas.Children.Add(missile);
        }
    }
}
