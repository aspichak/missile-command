﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MissileCommand.Util;

namespace MissileCommand.Screens
{
    /// <summary>
    /// Interaction logic for MainMenuScreen.xaml
    /// </summary>
    public partial class MainMenuScreen : UserControl, INotifyPropertyChanged
    {
        private string _randScore = "";
        public string RandScore { get => _randScore; private set { _randScore = value; NotifyPropertyChanged(); } }
        public static MediaPlayer player = new();
        private static readonly Uri soundBgm = new("file://" + System.IO.Path.GetFullPath(@"Resources\song_1.mp3"));
        public MainMenuScreen()
        {
            InitializeComponent();
            this.DataContext = this;
            // initialize db if empty
            using ScoreContext ScoresDb = new ScoreContext();
            if (!ScoresDb.ScoreEntries.Any())
            {
                ScoresDb.Add(new ScoreEntry("The Dog", 1));
                ScoresDb.Add(new ScoreEntry("Upside Down Fish", 2));
                ScoresDb.Add(new ScoreEntry("Birb", 3));
                ScoresDb.Add(new ScoreEntry("A dog pile", 4));
                ScoresDb.Add(new ScoreEntry("rabid racoon", 5));
                ScoresDb.Add(new ScoreEntry("Ferocious Ferret", 6));
                ScoresDb.Add(new ScoreEntry("My Neighbor", 7));
                ScoresDb.Add(new ScoreEntry("Rosco Rat", 8));
                ScoresDb.Add(new ScoreEntry("Squeeks the Dolphin", 9));
                ScoresDb.Add(new ScoreEntry("Whiskers McLicken", 1000));
                ScoresDb.SaveChanges();
            }
            var results = (from item in ScoresDb.ScoreEntries
                           select item).ToArray();
            var result = results[Random(0, results.Count())];
            RandScore = $"Player {result.Name} scored {result.Score}! In a past game!";

            var animation = Timer.At(10.0, () => BackgroundGrid.Children.Add(
                Lerp.Time(1.0, 0.0, 2.0, t => ScoreText.Opacity = t) +
                (() =>
                {
                    using ScoreContext ScoresDb = new ScoreContext();
                    var results = (from item in ScoresDb.ScoreEntries
                                   select item).ToArray();
                    var result = results[Random(0, results.Count())];
                    RandScore = $"Player {result.Name} scored {result.Score}! In a past game!";

                }) +
                Lerp.Time(0.0, 1.0, 2.0, t => ScoreText.Opacity = t)
            ), true);

            Loaded += MainMenuScreen_Loaded;
            BackgroundGrid.Children.Add(animation);
        }

        private void MainMenuScreen_Loaded(object sender, RoutedEventArgs e)
        {
            player.Open(soundBgm);
            player.MediaEnded += (s, e) => {
                player.Position = System.TimeSpan.Zero;
                player.Play();
            };
            player.Play();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            new SoundPlayer(System.IO.Path.GetFullPath(@"Resources\FX_ENTER.wav")).PlaySync();
            App.Current.Shutdown();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as ScreenManager).Switch(new GameSetupScreen());
        }
    }
}
