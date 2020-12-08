/*
 * Missile command game clone by Jeff Nix and Alex Spichak. This project requires [.NET SDK 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) and Visual Studio 2019 16.8.0 or later to compile. 
 * 
 * Controls:
 *     - Aim with the mouse and press 1, 2, or 3 to launch a missile from the corresponding silo.
 *     - Press Escape to pause the game.
 * 
 * Extra credit features:
 *     - Custom InputBindings
 *     - Custom Command for bindings
 *     - Super fun dynamic LINQ shenanigans!
 *     - Database is Entity Framework
 *     - XAML event triggers
 *     - Databinding used in some places
 *     - Controller? What controller! Is MVVM baby
 *     - Did you see that happen? No? Well not my fault you didn’t subscribe to the event!
 *     - Buttery smooth animations
 *     - Screen flash / shake effects
 *     - Background music and sound effects
 *     - Proper scaling on window resize
 *     - Microtransaction-free
 *     
 * Shortcomings:
 *     - Screen flash effect doesn’t extend to entire screen when window aspect ratio is not 16:9
 *     - Enemy/player missile code needs refactoring
 *     - Funny sounds on screen swap related to MediaPlayer
 */

using MissileCommand.Screens;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MissileCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double fps;
        Stopwatch stopwatch { get; } = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            stopwatch.Start();
            Mouse.OverrideCursor = Cursors.Cross;

            CompositionTarget.Rendering += (sender, e) =>
            {
                if (!stopwatch.IsRunning)
                    return;
                Update(stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();
            };

            MainGrid.Children.Add(Timer.Repeat(0.25, _ => FpsCounter.Text = $"{fps:0} FPS"));

            var screen = new ScreenManager(new MainMenuScreen());
            MainGrid.Children.Add(screen);
        }

        private void Update(double dt)
        {
            // https://stackoverflow.com/questions/87304/calculating-frames-per-second-in-a-game
            fps = fps * 0.9 + (1.0 / dt) * 0.1;
        }
    }
}
