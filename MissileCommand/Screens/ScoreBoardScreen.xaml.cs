using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<ScoreEntry> ScoreList { get; } = new List<ScoreEntry>();

        public ScoreBoardScreen()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ScoresDb = new ScoreContext();
            if (!ScoresDb.ScoreEntries.Any())
            { // initialize db if empty
                ScoresDb.Add( new ScoreEntry("The Dog", 1));
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
            LoadList();
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
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
            ScoreView.ItemsSource = ScoreList;
        }

        public static void AddScore(string name, int score)
        { // just adds a new entry to the scores DB
            using ScoreContext scores = new ScoreContext();
            scores.Add(new ScoreEntry(name, score));
            scores.SaveChanges();
        }
    }
}
