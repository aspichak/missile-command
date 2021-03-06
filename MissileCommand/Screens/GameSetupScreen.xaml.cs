﻿using System.Windows;
using System.Windows.Controls;

namespace MissileCommand.Screens
{
    /// <summary>
    /// Interaction logic for GameSetupScreen.xaml
    /// </summary>
    public partial class GameSetupScreen : UserControl
    {
        public GameSetupScreen()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuScreen.player.Stop();
            Difficulty difficulty = Difficulty.Easy;

            if (EasyMode.IsChecked == true) difficulty = Difficulty.Easy;
            if (NormalMode.IsChecked == true) difficulty = Difficulty.Normal;
            if (HardMode.IsChecked == true) difficulty = Difficulty.Hard;
            if (DebugMode.IsChecked == true) difficulty = Difficulty.Debug;

            var gameScreen = new IngameScreen((int)NumCitiesSlider.Value, (int)NumMissilesSlider.Value, difficulty);
            GameElement.TimeScale = TimescaleSlider.Value;
            (Parent as ScreenManager).Switch(gameScreen);
        }

        private void EasyMode_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
