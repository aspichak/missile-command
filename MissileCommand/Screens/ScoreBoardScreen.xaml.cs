using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
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
    /// Interaction logic for ScoreBoardScreen.xaml
    /// </summary>
    public partial class ScoreBoardScreen : UserControl
    {
        private ScoreContext ScoresDb { get; set; }
        public ObservableCollection<ScoreEntry> ScoreList { get; } = new ObservableCollection<ScoreEntry>();
        //private SoundPlayer player = new(Properties.Resources.game_over_ALT);
        private MediaPlayer player = new();
        private readonly Uri soundBgm = new("file://" + System.IO.Path.GetFullPath(@"Resources\game_over_ALT.wav"));

        public ScoreBoardScreen()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += OnLoaded;
            Unloaded += OnUnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            player.Open(soundBgm);
            player.MediaEnded += (s, e) => {
                player.Position = System.TimeSpan.Zero;
                player.Play();
            };
            player.Play();
            ScoresDb = new ScoreContext();
            LoadList();
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            player.Stop();
            ScoresDb.Dispose();
            ScoresDb = null;
        }

        private void LoadList()
        {
            ScoreList.Clear();
            var x = (from item in ScoresDb.ScoreEntries
                     orderby item.Score descending
                    select item).Take(10);

            foreach(ScoreEntry s in x)
            {
                ScoreList.Add(s);
            }
        }

        public static void AddScore(string name, int score)
        { // just adds a new entry to the scores DB
            using ScoreContext scores = new ScoreContext();
            scores.Add(new ScoreEntry(name, score));
            scores.SaveChanges();
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as ScreenManager).Switch(new MainMenuScreen());
        }
    }
}
