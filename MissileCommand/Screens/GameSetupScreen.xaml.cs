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
            Difficulty difficulty = Difficulty.Easy;

            if (EasyMode.IsChecked == true) difficulty = Difficulty.Easy;
            if (NormalMode.IsChecked == true) difficulty = Difficulty.Normal;
            if (HardMode.IsChecked == true) difficulty = Difficulty.Hard;
            if (DebugMode.IsChecked == true) difficulty = Difficulty.Debug;

            var gameScreen = new IngameScreen((int)NumCitiesSlider.Value, (int)NumMissilesSlider.Value, difficulty);
            GameElement.TimeScale = TimescaleSlider.Value;
            (Parent as ScreenManager).Switch(gameScreen);
        }
    }
}
