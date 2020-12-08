using System;
using System.Windows;
using System.Windows.Controls;

namespace MissileCommand.Screens
{
    /// <summary>
    /// Interaction logic for PauseOverlay.xaml
    /// </summary>
    public partial class PauseOverlay : UserControl
    {
        public event Action Closing;

        public PauseOverlay()
        {
            InitializeComponent();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Closing?.Invoke();
            (Parent as ScreenManager).CloseOverlay(this);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            (Parent as ScreenManager).CloseOverlay(this);
            (Parent as ScreenManager).Switch(new MainMenuScreen());
        }
    }
}
