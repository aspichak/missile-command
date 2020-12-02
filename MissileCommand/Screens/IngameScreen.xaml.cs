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

        public void Add(UIElement element)
        {
            GameCanvas.Children.Add(element);
        }

        public void LayoutBuildings()
        {
            var bottom = 720 - City.Size.Height - 32;
            var totalWidth = 1280;
            var outerSiloOffset = 128;

            var bankWidth = (totalWidth - Silo.Size.Width) / 2 - outerSiloOffset - Silo.Size.Width;

            var leftCityBank = new Rect(outerSiloOffset + Silo.Size.Width, bottom, bankWidth, City.Size.Height);
            var rightCityBank = new Rect((totalWidth + Silo.Size.Width) / 2, bottom, bankWidth, City.Size.Height);

            for (int i = 0; i < 3; i++)
            {
                var n = 3;
                var margin = (leftCityBank.Width - City.Size.Width * n) / (n + 1);
                var left = margin * (i + 1) + City.Size.Width * i;

                var city = new City();
                Canvas.SetTop(city, bottom);
                Canvas.SetLeft(city, leftCityBank.X + left);

                cities.Add(city);
            }

            for (int i = 0; i < 3; i++)
            {
                var n = 3;
                var margin = (leftCityBank.Width - City.Size.Width * n) / (n + 1);
                var left = margin * (i + 1) + City.Size.Width * i;

                var city = new City();
                Canvas.SetTop(city, bottom);
                Canvas.SetLeft(city, rightCityBank.X + left);

                cities.Add(city);
            }

            cities.ForEach(city => Add(city));

            var silo1 = new Silo();
            var silo2 = new Silo();
            var silo3 = new Silo();

            Canvas.SetTop(silo1, bottom);
            Canvas.SetTop(silo2, bottom);
            Canvas.SetTop(silo3, bottom);

            Canvas.SetLeft(silo1, outerSiloOffset);
            Canvas.SetLeft(silo2, (totalWidth - Silo.Size.Width) / 2);
            Canvas.SetLeft(silo3, totalWidth - outerSiloOffset - Silo.Size.Width);

            Add(silo1);
            Add(silo2);
            Add(silo3);
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
                var targetPos = new Vector(Canvas.GetLeft(target) + City.Size.Width / 2, Canvas.GetTop(target) + 16);
                targetPos.X += Random(-City.Size.Width / 2, City.Size.Width / 2);
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
            //    cities.ForEach(c => c.Rebuild());
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
