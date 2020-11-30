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
        private const double baseEnemySpeed = 60;
        private const double basePlayerMissileSpeed = 100;

        private readonly List<City> cities = new List<City>();
        private readonly List<EnemyMissile> enemies = new List<EnemyMissile>();
        private int score = 0;
        private Difficulty difficulty;

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

        public IngameScreen(int numCities, int numMissiles, Difficulty difficulty)
        {
            InitializeComponent();

            this.difficulty = difficulty;

            Focusable = true;
            Loaded += (_, _) => Keyboard.Focus(this);

            // Create cities
            cities = new List<City>
            {
                new City(new(100, 600)),
                new City(new(200, 600)),
                new City(new(300, 600)),
                new City(new(400, 600))
            };

            cities.ForEach(city => Add(city));

            // Create silos

            // Start the game
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
            var waveSequence = WaveNumberAnimation(Wave);

            for (int i = 0; i < 5; i++)
            {
                var delay = Random(1.0, 3.0);
                var speed = baseEnemySpeed * difficulty.EnemySpeed * Random(0.75, 1.25);
                var missile = new EnemyMissile(new(Random(0, 1280), 0), RandomTarget(), speed);

                waveSequence += Timer.At(delay, () => Add(missile));
                enemies.Add(missile);
            }

            Add(waveSequence);

            //cities.ForEach(c => c.Rebuild());

            Add(Timer.Repeat(0.25, timer =>
            {
                if (cities.All(c => c.IsDestroyed))
                {
                    Add(new ScreenShake(16, 2.0));
                    Add(new ScreenFlash(0.75, 2.0));
                    Add(Lerp.Time(1.0, 0.2, 1.5, t => GameElement.TimeScale = t));

                    Add(Timer.At(2.0, () =>
                    {
                        GameElement.TimeScale = 1;
                        (Parent as ScreenManager).Switch(new GameOverScreen());
                    }));

                    timer.Destroy();
                }
                else if (enemies.All(m => m.Destroyed))
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

        private City RandomTarget()
        {
            return cities[Random(0, cities.Count)];
        }

        private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Paused) return;
            var pos = Mouse.GetPosition(GameCanvas);
            var missile = new Missile(new(640, 700), new(pos.X, pos.Y), basePlayerMissileSpeed * difficulty.PlayerMissileSpeed);

            missile.Exploding += HandlePlayerMissileExplosion;

            GameCanvas.Children.Add(missile);
        }

        private void HandlePlayerMissileExplosion(Vector pos, double radius)
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
        }

        private void HandleEnemyMissileExplosion(Vector pos, double radius)
        {
            //var cities = from c in GameCanvas.Children. select m;
            //var cities = new List<City> { City1, City2, City3 };


            //foreach (var city in cities)
            //{
            //    var cityPos = new Vector(city.Posi)
            //    if (city.Active && city.Position.DistanceTo(pos) <= radius)
            //    {
            //        enemy.Explode();
            //        Score += 1;
            //        MainWindow.Debug(score.ToString());
            //    }
            //}
            //var target = missile.Target;

            //if (target.Active)
            //{
            //    target.Explode();
            //    target.RebuildAfterWaves = 3;
            //}
        }
    }
}
