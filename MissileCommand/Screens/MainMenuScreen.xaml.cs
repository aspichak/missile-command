﻿using System;
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
    /// Interaction logic for MainMenuScreen.xaml
    /// </summary>
    public partial class MainMenuScreen : UserControl, INotifyPropertyChanged
    {
        private string _randScore = "";
        public string RandScore { get => _randScore; private set { _randScore = value; NotifyPropertyChanged(); } }
        private Random rand = new Random();
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
            var result = results[rand.Next(0, results.Count() - 1)];
            RandScore = $"Player {result.Name} scored {result.Score}! In a past game!";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (Parent as ScreenManager).Switch(new IngameScreen());
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
