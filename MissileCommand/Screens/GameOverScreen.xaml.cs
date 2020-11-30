using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public bool ScoreSaved { get; private set; } = false;
        public GameOverScreen() : this (0) { }
        public GameOverScreen(int score)
        {
            InitializeComponent();
            this.DataContext = this;
            Score = score;
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            if (ScoreSaved) // sentinal
                return;

            if (!String.IsNullOrEmpty(NameField.Text) && !String.IsNullOrWhiteSpace(NameField.Text))
            {
                // SAVE IT!
                using ScoreContext scores = new ScoreContext();
                scores.ScoreEntries.Add(new ScoreEntry(NameField.Text, Score));
                scores.SaveChanges();
                ScoreSaved = true;
                SaveButton.IsEnabled = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
