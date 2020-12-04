using System;
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

            LayoutBuildings();
            StartWave();
        }

        private void Add(UIElement element)
        {
            GameCanvas.Children.Add(element);
        }

        public void LayoutBuildings()
        {
            for (int i = 0; i < 6; i++)
            {
                cities.Add(new City());
            }

            var bottom = 40.0;

            var outerSiloOffset = 96.0;
            var totalWidth = 1280.0 - outerSiloOffset * 2;
            var blankSpace = totalWidth - City.Size.Width * cities.Count - Silo.Size.Width * 3;
            var spacing = blankSpace / (cities.Count + 3 - 1);

            var buildings = new List<GameElement>();

            buildings.Add(new Silo());
            buildings.AddRange(cities.Take(cities.Count / 2));
            buildings.Add(new Silo());
            buildings.AddRange(cities.Skip(cities.Count / 2));
            buildings.Add(new Silo());

            var x = outerSiloOffset;

            foreach (var building in buildings)
            {
                Canvas.SetLeft(building, x);
                Canvas.SetBottom(building, bottom);

                if (building is Silo) x += Silo.Size.Width;
                if (building is City) x += City.Size.Width;

                x += spacing;
                Add(building);
            }
        }

        private void Tutorial()
        {
        }

        private void StartWave()
        {
            var waveSequence = WaveNumberAnimation() + RebuildCities;

            for (int i = 0; i < 5; i++)
            {
                var delay = Random(1.0, 3.0);
                var speed = baseEnemySpeed * difficulty.EnemySpeed * Random(0.75, 1.25);
                var target = cities.Random();
                var targetPos = new Vector(Canvas.GetLeft(target) + City.Size.Width / 2, 720 - 80);
                targetPos.X += Random(-(City.Size.Width - 32) / 2, (City.Size.Width - 32) / 2);
                var missile = new EnemyMissile(new(Random(0, 1280), 0), targetPos, speed);
                missile.Target = target;

                waveSequence += Timer.At(delay, () => Add(missile));
                enemies.Add(missile);
            }

            Add(waveSequence);
            Add(Timer.Repeat(0.25, CheckWaveEnd));
        }

        private Sequence WaveNumberAnimation()
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

        private void RebuildCities()
        {
            //if (Wave % difficulty.CityRebuildDelay == 0)
            //{
            cities.ForEach(c => c.Rebuild());
            //}
        }

        private void CheckWaveEnd(Timer timer)
        {
            if (cities.All(c => c.IsDestroyed))
            {
                // Game over
                Add(new ScreenShake(16, 2.0));
                Add(new ScreenFlash(0.75, 2.0));
                Add(Lerp.Time(1.0, 0.2, 1.5, t => GameElement.TimeScale = t));

                Add(Timer.At(2.0, () =>
                {
                    GameElement.TimeScale = 1;
                    (Parent as ScreenManager).Switch(new GameOverScreen());
                }));

                // TODO: disable input

                timer.Destroy();
            }
            else if (enemies.All(m => m.Destroyed))
            {
                // Wave ended
                enemies.Clear();
                Wave++;
                StartWave();
                timer.Destroy();
            }
        }

        //private City RandomTarget()
        //{
        //    return cities.Random();
        //}

        private void GameCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // TODO: don't allow missiles to be fired on game over
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
                }
            }
        }
    }
}
