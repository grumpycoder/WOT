﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WOT.Server.Models;
using WOT.Server.Properties;

namespace WOT.Server
{

    public partial class MainWindow
    {
        private Person _currentPerson;
        private readonly IList<Person> _personList;
        private readonly Canvas _canvas;
        private readonly double _canvasWidth;
        private readonly double _canvasHeight;
        private readonly DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();

            _canvas = WallCanvas;
            _canvas.Height = SystemParameters.PrimaryScreenHeight;
            _canvas.Width = SystemParameters.PrimaryScreenWidth;
            _canvas.UpdateLayout();

            _canvasWidth = _canvas.Width;
            _canvasHeight = _canvas.Height;
            ExpanderSettings.Width = _canvasWidth;

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.ItemAddSpeed) };
            timer.Tick += timer_Tick;

            _personList = CreatePersonList();

            timer.Start();

        }

        private static IList<Person> CreatePersonList()
        {
            var list = new List<Person>();

            for (int i = 1; i < 101; i++)
            {
                list.Add(new Person
                {
                    Name = "Person " + (i % 4 == 0 ? "VIP" : "") + i,
                    Id = i,
                    IsDonor = true,
                    IsVIP = i % 4 == 0
                });
            }
            return list;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _currentPerson = _personList.Next(_currentPerson);
            var attr = CreateNewRandomAttr(_currentPerson.IsVIP);
            CreateNewTextBlock(_currentPerson, attr);
        }

        private TextBlockAttribute CreateNewRandomAttr(bool? vip = false)
        {
            var leftMargin = Settings.Default.LeftMargin;
            var rightMargin = _canvasWidth - Settings.Default.RightMargin;
            var maxFontSize = vip.GetValueOrDefault() ? Settings.Default.MaxFontSizeVIP : Settings.Default.MaxFontSize;
            var minFontSize = vip.GetValueOrDefault() ? Settings.Default.MinFontSizeVIP : Settings.Default.MinFontSize;

            var rnd = new Random();
            var xAxis = rnd.Next(leftMargin, rightMargin.ToInt());
            var yAxis = rnd.Next(0, 10);
            var size = rnd.Next(minFontSize, maxFontSize);

            var speed = (Settings.Default.DefaultScrollSpeed / (double)size) * 10;
            var color = RandomColor();

            var attr = new TextBlockAttribute
            {
                Left = xAxis,
                Top = yAxis,
                Bottom = _canvasHeight,
                Speed = speed,
                Size = size,
                Color = color,
            };
            if (vip.GetValueOrDefault()) attr.ZIndex = 998;

            rnd = null;
            GC.Collect();
            return attr;
        }

        private static Color RandomColor()
        {
            var rnd = new Random();
            var r = Convert.ToByte(rnd.Next(0, 255));
            var g = Convert.ToByte(rnd.Next(0, 255));
            var b = Convert.ToByte(rnd.Next(0, 255));
            var color = Color.FromRgb(r, g, b);
            rnd = null;
            GC.Collect();
            return color;
        }

        private void CreateNewTextBlock(Person person, TextBlockAttribute attr)
        {
            var animation = new DoubleAnimation
            {
                From = attr.Top,
                To = attr.Bottom,
                Duration = new Duration(TimeSpan.FromSeconds(attr.Speed))
            };

            var textBlock = new TextBlock
            {
                Name = "TextBlock" + person.Id,
                Text = person.Name,
                Tag = "TextBlock" + person.Id,
                FontSize = attr.Size,
                FontWeight = attr.Weight,
                Foreground = new SolidColorBrush(attr.Color)
            };

            var e = new AnimationEventArgs { TagName = textBlock.Name };
            animation.Completed += (sender, args) => AnimationOnCompleted(e);

            Canvas.SetLeft(textBlock, attr.Left);
            Canvas.SetTop(textBlock, attr.Top);
            Panel.SetZIndex(textBlock, attr.ZIndex);
            _canvas.Children.Add(textBlock);

            textBlock.BeginAnimation(TopProperty, animation);
            GC.Collect();
        }

        private void AnimationOnCompleted(AnimationEventArgs eventArgs)
        {
            var tagName = eventArgs.TagName;
            var result = _canvas.Children.Cast<FrameworkElement>().First(x => x.Tag != null && x.Tag.ToString() == tagName);

            result.BeginAnimation(TopProperty, null);
            _canvas.Children.Remove(result);
            GC.Collect();
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            ExpanderSettings.IsExpanded = false;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            //{
            //    //Debug.WriteLine(currentProperty.Name);
            //    //Properties.Settings.Default[currentProperty.Name] = result.ToString();
            //    //Properties.Settings.Default.Save();
            //}
            //var settings = System.Configuration.ConfigurationManager.AppSettings;
            //Settings.Default.ItemAddSpeed = Convert.ToDouble(settings["ItemAddSpeed"]);

            //foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            //{
            //    Settings.Default.Properties["MyPropertyName"].Reset();
            //    Debug.WriteLine(string.Format("{0} {1}", currentProperty.Name, currentProperty.DefaultValue));
            //}

            //foreach (var key in settings.AllKeys)
            //{

            //    Debug.WriteLine(settings.GetValues(key));
            //    Debug.WriteLine(key);
            //}
            //foreach (SettingsProperty currentProperty in ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings)
            //{
            //    Debug.WriteLine(currentProperty.Name);
            //    //Properties.Settings.Default[currentProperty.Name] = result.ToString();
            //    //Properties.Settings.Default.Save();
            //}

            //var settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
            //foreach (var setting in settings)
            //{

            //}

            ////Settings.Default.ItemAddSpeed = 0.1;
            //var s = Settings.Default.Properties["ItemAddSpeed"].DefaultValue;
            //var s2 = Settings.Default.ItemAddSpeed; 
            Properties.Settings.Default.Reset();
            //s = Settings.Default.Properties["ItemAddSpeed"].DefaultValue;
            //s2 = Settings.Default.ItemAddSpeed; 

            Settings.Default.Save();
        }

        private void sldAddSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.ItemAddSpeed = e.NewValue;
            if (timer != null) timer.Interval = TimeSpan.FromSeconds(e.NewValue);
        }

        private void SldScrollSpeed_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.DefaultScrollSpeed = e.NewValue.ToInt();
        }

        private void SldMaxFont_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.MaxFontSize = e.NewValue.ToInt();
        }

        private void SldMinFont_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.MinFontSize = e.NewValue.ToInt();
        }

        private void SldMinFontSizeVIP_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.MinFontSizeVIP = e.NewValue.ToInt();
        }

        private void SldMaxFontSizeVIP_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Settings.Default.MaxFontSizeVIP = e.NewValue.ToInt();
        }
    }

    public class AnimationEventArgs : EventArgs
    {
        public string TagName { get; set; }
    }

    public class TextBlockAttribute
    {
        public TextBlockAttribute()
        {
            Weight = FontWeights.Normal;
            ZIndex = 1;
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public double Bottom { get; set; }
        public int Size { get; set; }
        public double Speed { get; set; }
        public int ZIndex { get; set; }
        public FontWeight Weight { get; set; }
        public Color Color { get; set; }
    }
}
