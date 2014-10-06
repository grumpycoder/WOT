using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WOT.Kiosk
{
    public partial class FinishPage : Page
    {
        public FinishPage(string message)
        {
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(message))
            {
                statusMessage.Text = message; 
            }
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(10)};
            timer.Tick += TimerOnTick;
            timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (NavigationService != null) NavigationService.Navigate(new WelcomePage());
            ((DispatcherTimer) sender).Stop();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WelcomePage());
        }
    }
}
