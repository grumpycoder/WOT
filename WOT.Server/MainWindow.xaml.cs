using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WOT.Server.Models;

namespace WOT.Server
{

    public partial class MainWindow
    {
        private Person _currentPerson;
        private readonly IList<Person> _personlList;
        private readonly Canvas _canvas;
        private readonly double _canvasWidth;
        private readonly double _canvasHeight;

        public MainWindow()
        {
            InitializeComponent();
            _canvas = new Canvas
            {
                Height = SystemParameters.PrimaryScreenHeight,
                Width = SystemParameters.PrimaryScreenWidth,
                Background = Brushes.Black
            };
            mainWindow.Content = _canvas;
            _canvasWidth = _canvas.Width;
            _canvasHeight = _canvas.Height;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Properties.Settings.Default.ItemAddSpeed) };
            timer.Tick += timer_Tick;

            _personlList = CreatePersonList();

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
            _currentPerson = _personlList.Next(_currentPerson);
            var attr = CreateNewRandomAttr(_currentPerson.IsVIP);
            CreateNewTextBlock(_currentPerson, attr);
        }

        private TextBlockAttribute CreateNewRandomAttr(bool? vip = false)
        {
            var leftMargin = Properties.Settings.Default.LeftMargin;
            var rightMargin = _canvasWidth - Properties.Settings.Default.RightMargin;
            var maxFontSize = vip.GetValueOrDefault() ? Properties.Settings.Default.MaxFontSizeVIP : Properties.Settings.Default.MaxFontSize; 
            var minFontSize = vip.GetValueOrDefault() ? Properties.Settings.Default.MinFontSizeVIP : Properties.Settings.Default.MinFontSize; 

            var rnd = new Random();
            var xAxis = rnd.Next(leftMargin, rightMargin.ToInt());
            var yAxis = rnd.Next(0, 10);
            var size = rnd.Next(minFontSize, maxFontSize);

            var speed = (Properties.Settings.Default.DefaultScrollSpeed / (double)size) * 10;
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
            if (vip.GetValueOrDefault()) attr.ZIndex = 999;

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
            //Debug.WriteLine(WallCanvas.Children.Count);
            GC.Collect();
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
