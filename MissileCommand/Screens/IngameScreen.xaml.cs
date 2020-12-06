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
        private const double basePlayerMissileSpeed = 160;

        private readonly List<City> cities = new List<City>();
        private readonly List<EnemyMissile> enemies = new List<EnemyMissile>();
        private int score = 0;
        private Difficulty difficulty;
        private int numCities, numMissiles;

        public Silo Silo1 { get; private set; }
        public Silo Silo2 { get; private set; }
        public Silo Silo3 { get; private set; }

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

            this.numCities = numCities;
            this.numMissiles = numMissiles;
            this.difficulty = difficulty;

            Focusable = true;
            Loaded += (_, _) => Keyboard.Focus(this);

            Silo1 = new Silo(false);
            Silo1.GestureKey = Key.D1;

            Silo2 = new Silo(false);
            Silo2.GestureKey = Key.D2;

            Silo3 = new Silo(false);
            Silo3.GestureKey = Key.D3;

            DataContext = this;

            LayoutBuildings();
            StartWave();
        }

        private void Add(UIElement element)
        {
            GameCanvas.Children.Add(element);
        }

        public void LayoutBuildings()
        {
            for (int i = 0; i < numCities; i++)
            {
                cities.Add(new City());
            }

            var bottom = 720.0 - 16.0;

            var outerSiloOffset = 96.0;
            var totalWidth = 1280.0 - outerSiloOffset * 2;
            var blankSpace = totalWidth - City.Size.Width * cities.Count - Silo.Size.Width * 3;
            var spacing = blankSpace / (cities.Count + 3 - 1);

            var buildings = new List<GameElement>();

            buildings.Add(Silo1);
            buildings.AddRange(cities.Take(cities.Count / 2));
            buildings.Add(Silo2);
            buildings.AddRange(cities.Skip(cities.Count / 2));
            buildings.Add(Silo3);

            var x = outerSiloOffset;

            foreach (var building in buildings)
            {
                Canvas.SetLeft(building, x);

                if (building is Silo)
                {
                    x += Silo.Size.Width;
                    Canvas.SetTop(building, bottom - Silo.Size.Height);
                }

                if (building is City)
                {
                    x += City.Size.Width;
                    Canvas.SetTop(building, bottom - City.Size.Height);
                }

                x += spacing;
                Add(building);
            }
        }

        private void Tutorial()
        {
        }

        private void StartWave()
        {
            var waveSequence = WaveNumberAnimation() + RebuildTargets;

            for (int i = 0; i < 5; i++)
            {
                var delay = Random(1.0, 3.0);
                var speed = baseEnemySpeed * difficulty.EnemySpeed * Random(0.75, 1.25);
                var missile = new EnemyMissile(new(Random(0, 1280), 0), speed);

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

        private void RebuildTargets()
        {
            var silos = GameCanvas.Children.OfType<Silo>();

            foreach (Silo silo in silos)
            {
                silo.Rebuild();
            }

            if (Wave % difficulty.CityRebuildDelay == 0)
            {
                cities.ForEach(c => c.Rebuild());
            }
        }

        private void CheckWaveEnd(Timer timer)
        {
            var cities = GameCanvas.Children.OfType<City>();
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

        private void Pause()
        {
            Paused = true;
            GameCanvas.Children.OfType<GameElement>().ForEach(g => g.Active = false);

            var pauseOverlay = new PauseOverlay();
            pauseOverlay.Closing += () => Resume();

            (Parent as ScreenManager).Overlay(pauseOverlay);
        }

        private void Resume()
        {
            Paused = false;
            GameCanvas.Children.OfType<GameElement>().ForEach(g => g.Active = true);
        }

        private void PauseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Pause();
        }

        private void CanPauseHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
