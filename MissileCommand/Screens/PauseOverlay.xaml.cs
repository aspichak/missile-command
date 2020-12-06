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
