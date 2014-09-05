using System.Windows;
using WOT.Server.Properties;

namespace WOT.Server
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            
            Settings.Default.MaxFontSize = sldMaxFont.Value.ToInt();
            Settings.Default.MinFontSize = sldMinFont.Value.ToInt();

            Settings.Default.ItemAddSpeed = sldAddSpeed.Value.ToInt();
            Settings.Default.DefaultScrollSpeed = sldScrollSpeed.Value.ToInt(); 
            
            Settings.Default.Save();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reset();
        }

        private void btnCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
