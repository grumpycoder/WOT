﻿using System.Diagnostics;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;
using WOT.Kiosk.Properties;

namespace WOT.Kiosk
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Height = Settings.Default.AppHeight;
            mainWindow.Width = Settings.Default.AppWidth;
            ConnectionManager.ConnectAsync();

            _mainFrame.Navigate(new WelcomePage());

        }
    }
}