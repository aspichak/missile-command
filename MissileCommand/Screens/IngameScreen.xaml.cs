using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        private int numCities;
        //private SoundPlayer player = new(Properties.Resources.song_gameplay);
        private MediaPlayer player = new();
        private readonly Uri soundBgm = new("file://" + Path.GetFullPath(@"Resources\song_gameplay.mp3"));

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

                var animation =
                    Lerp.Time(1.0, 1.4, 0.2, t => ScoreLabel.RenderTransform = new ScaleTransform(t, t)) +
                    Lerp.Time(1.4, 1.0, 0.2, t => ScoreLabel.RenderTransform = new ScaleTransform(t, t));
                
                Add(animation);
            }
        }

        public int Wave { get; private set; } = 1;
        public bool Paused { get; set; }

        public IngameScreen(int numCities, int numMissiles, Difficulty difficulty)
        {
            InitializeComponent();

            this.numCities = numCities;
            this.difficulty = difficulty;

            Focusable = true;
            Loaded += IngameScreen_Loaded;
            Unloaded += IngameScreen_Unloaded;

            Silo1 = new Silo(numMissiles, difficulty.PlayerMissileSpeed);
            Silo1.GestureKey = Key.D1;
            Silo1.Payload += HandlePlayerMissileExplosion;

            Silo2 = new Silo(numMissiles, difficulty.PlayerMissileSpeed);
            Silo2.GestureKey = Key.D2;
            Silo2.Payload += HandlePlayerMissileExplosion;

            Silo3 = new Silo(numMissiles, difficulty.PlayerMissileSpeed);
            Silo3.GestureKey = Key.D3;
            Silo3.Payload += HandlePlayerMissileExplosion;

            DataContext = this;

            LayoutBuildings();
            StartWave();
        }

        private void IngameScreen_Unloaded(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }

        private void IngameScreen_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
            player.Open(soundBgm);
            player.MediaEnded += (s, e) =>
            {
                player.Position = System.TimeSpan.Zero;
                player.Play();
            };
            player.Play();

            var instructions = new InstructionsOverlay();

            Add(Timer.Delay(1.5) + (() => (Parent as ScreenManager).Overlay(instructions)));
            Add(Timer.Delay(6.0) + (() => (Parent as ScreenManager).CloseOverlay(instructions)));
        }

        private void Add(UIElement element)
        {
            GameCanvas.Children.Add(element);
        }

        public void LayoutBuildings()
        {
            for (int i = 0; i < numCities; i++)
            {
                cities.Add(new City(difficulty.CityRebuildDelay));
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

        private void StartWave()
        {
            var waveSequence = WaveNumberAnimation() + RebuildTargets;

            for (int i = 0; i < 10; i++)
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

            cities.ForEach(c => c.Rebuild());
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
                    (Parent as ScreenManager).Switch(new GameOverScreen(Score));
                }));

                // TODO: disable input

                timer.Destroy();
            }
            else if (enemies.All(m => m.Destroyed))
            {
                // Wave ended
                enemies.Clear();
                ScoreWave();
                Wave++;
                StartWave();
                timer.Destroy();
            }
        }

        private void ScoreWave()
        {
            int citiesLeft = GameCanvas.Children.OfType<City>().Count();
            int missilesLeft = 0;
            missilesLeft += !Silo1.IsDestroyed ? Silo1.MissileCount : 0;
            missilesLeft += !Silo2.IsDestroyed ? Silo2.MissileCount : 0;
            missilesLeft += !Silo3.IsDestroyed ? Silo3.MissileCount : 0;
            int bonusPts = (int)(citiesLeft * missilesLeft * difficulty.ScoreMultiplier); // casting is fine. I want floor
            Score += bonusPts;
            ScoreBonusLabel.Text = $"Wave Clear Bonus! +{bonusPts}!";
            Add(Timer.At(3.0, () => { ScoreBonusLabel.Text = null; }));
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
            Focus();
        }

        private void PauseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Pause();
        }

        private void CanPauseHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Paused;
        }

        private void HandlePlayerMissileExplosion(Vector pos, double radius)
        {
            var missiles = GameCanvas.Children.OfType<EnemyMissile>().ToList();
            foreach (EnemyMissile missile in missiles)
            {
                if (missile.Active && missile.Position.DistanceTo(pos) <= radius)
                {
                    missile.Explode();
                    Score += difficulty.PointsPerEnemyMissile;
                }
            }
        }
    }
}
