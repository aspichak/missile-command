using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
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

namespace MissileCommand.Screens
{
    /// <summary>
    /// Interaction logic for GameOverScreen.xaml
    /// </summary>
    public partial class GameOverScreen : UserControl, INotifyPropertyChanged
    {
        private int _score;

        public int Score { get =>  _score; private set { _score = value; NotifyPropertyChanged(); } }
        //private SoundPlayer player = new(Properties.Resources.game_over);
        private MediaPlayer player = new();
        private readonly Uri soundBgm = new("file://" + System.IO.Path.GetFullPath(@"Resources\game_over.mp3"));
        public event PropertyChangedEventHandler PropertyChanged;

        public GameOverScreen(int score = 0)
        {
            InitializeComponent();
            this.DataContext = this;
            Score = score;
            Unloaded += GameOverScreen_Unloaded;
            Loaded += GameOverScreen_Loaded;
        }

        private void GameOverScreen_Unloaded(object sender, RoutedEventArgs e)
        {
            player.Stop();
        }

        private void GameOverScreen_Loaded(object sender, RoutedEventArgs e)
        {
            player.Open(soundBgm);
            player.MediaEnded += (s, e) => {
                player.Position = System.TimeSpan.Zero;
                player.Play();
            };
            player.Play();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(NameField.Text))
            {
                SaveButton.Content = "Continue Without Saving";
            }
            else
            {
                SaveButton.Content = "Save and Continue";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;

            if (!String.IsNullOrWhiteSpace(NameField.Text))
            {
                using ScoreContext scores = new ScoreContext();
                scores.ScoreEntries.Add(new ScoreEntry(NameField.Text, Score));
                scores.SaveChanges();
            }

            (Parent as ScreenManager).Switch(new ScoreBoardScreen());
        }
    }
}
