using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            StartWave();
        }

        private void Add(UIElement element)
        {
            GameCanvas.Children.Add(element);
        }

        private Sequence WaveNumberAnimation(int wave)
        {
            var delay = 1.0;
            var transitionTime = 1.0;
            var holdTime = 1.0;

            var animation =
                Timer.Delay(delay)

                + Lerp.Time(0, 1, transitionTime, t => WaveLabel.Opacity = t, Lerp.Sine)
                * Lerp.Time(160, 100, transitionTime, t => WaveLabel.Margin = new Thickness(0, t, 0, 0), Lerp.Sine)

                + Timer.Delay(holdTime)

                + Lerp.Time(1, 0, transitionTime, t => WaveLabel.Opacity = t, Lerp.Sine)
                * Lerp.Time(100, 160, transitionTime, t => WaveLabel.Margin = new Thickness(0, t, 0, 0), Lerp.Sine);

            WaveLabel.Opacity = 0;
            WaveLabel.Text = $"Wave {Wave}";

            return animation;
        }

        private void StartWave()
        {
            // TODO: Create cities
            Add(new City());

            var waveSequence = WaveNumberAnimation(Wave);

            for (int i = 0; i < 5; i++)
            {
                var delay = Random(1, 3);
                var speed = Random(50, 80);
                var missile = new EnemyMissile(new(Random(0, 1280), 0), new(Random(0, 1280), 720), speed);

                waveSequence += Timer.At(delay, () => Add(missile));
                enemies.Add(missile);
            }

            Add(waveSequence);

            Add(Timer.Repeat(0.25, timer =>
            {
                if (enemies.All(m => m.Destroyed))
                {
                    enemies.Clear();
                    EndWave();
                    timer.Destroy();
                }
            }));
        }

        private void EndWave()
        {
            Wave++;
            StartWave();
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
