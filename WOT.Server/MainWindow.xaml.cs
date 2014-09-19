using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;
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
        private readonly double _quadSize; 
        private readonly DispatcherTimer timer;
        private readonly AppContext db; 

        public string ServerURI = Settings.Default.ServerURI;
        public string HubName = Settings.Default.HubName; 

        public IHubProxy HubProxy { get; set; }
        public HubConnection Conn { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _canvas = WallCanvas;
            _canvas.Height = SystemParameters.PrimaryScreenHeight;
            _canvas.Width = SystemParameters.PrimaryScreenWidth;

            db = new AppContext();
            SetDataBindings();
            _canvas.UpdateLayout();

            _canvasWidth = _canvas.Width;
            _canvasHeight = _canvas.Height;
            _quadSize = _canvasWidth/4;
            ExpanderSettings.Width = _canvasWidth;

            timer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(Settings.Default.ItemAddSpeed) };
            timer.Tick += timer_Tick;

            //_personList = CreateLocalPersonList();
            _personList = db.Persons.ToList();
            timer.Start();

            ConnectAsync();
        }

        private async void ConnectAsync()
        {
            Conn = new HubConnection(ServerURI);
            Conn.StateChanged += ConnOnStateChanged;
            HubProxy = Conn.CreateHubProxy(HubName);

            HubProxy.On<string, string>("sendName", (name, message) => this.Dispatcher.Invoke(() => AddPersonToListFromKiosk(message, name)));
            try
            {
                await Conn.Start();
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Unable to connect to server: Start server before connecting clients");
                Debug.WriteLine(e.InnerException.ToString());
                return;
            }
            Debug.WriteLine("Connected to server at {0}", ServerURI);
        }

        private static IList<Person> CreateLocalPersonList()
        {
            var list = new List<Person>();

            for (int i = 1; i < 101; i++)
            {
                list.Add(new Person
                {
                    Firstname = "Person " + (i % 4 == 0 ? "VIP" : "") + i,
                    Id = i,
                    IsDonor = true,
                    IsVIP = i % 4 == 0
                });
            }
            return list;
        }

        public void CreateNewTextBlock(Person person, TextBlockAttribute attr)
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
                Text = person.ToString(),
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
            //GC.Collect();
        }

        public void AddPersonToListFromKiosk(string name, string location)
        {
            var person = new Person()
            {
                Firstname = name,
                Id = _personList.Count + 1,
                IsVIP = true
            };

            _personList.Add(person);
            int quad; 
            int.TryParse(location, out quad);
            AddPersonToDisplay(person, quad);
        }

        public void AddPersonToDisplay(Person person, int quadrant)
        {
            CreateNewTextBlock(person, CreateNewRandomAttr(person.IsVIP));
        }

        private void SetDataBindings()
        {
            var bMinFontSize = new Binding { Source = Settings.Default, Path = new PropertyPath("MinFontSize") };
            sldMinFont.SetBinding(RangeBase.ValueProperty, bMinFontSize);

            var bMaxFontSize = new Binding { Source = Settings.Default, Path = new PropertyPath("MaxFontSize") };
            sldMaxFont.SetBinding(RangeBase.ValueProperty, bMaxFontSize);

            var bMinFontSizeVIP = new Binding { Source = Settings.Default, Path = new PropertyPath("MinFontSizeVIP") };
            sldMinFontSizeVIP.SetBinding(RangeBase.ValueProperty, bMinFontSizeVIP);

            var bMaxFontSizeVIP = new Binding { Source = Settings.Default, Path = new PropertyPath("MaxFontSizeVIP") };
            sldMaxFontSizeVIP.SetBinding(RangeBase.ValueProperty, bMaxFontSizeVIP);

            var bScrollSpeed = new Binding { Source = Settings.Default, Path = new PropertyPath("DefaultScrollSpeed") };
            sldScrollSpeed.SetBinding(RangeBase.ValueProperty, bScrollSpeed);

            var bAddSpeed = new Binding { Source = Settings.Default, Path = new PropertyPath("ItemAddSpeed") };
            sldAddSpeed.SetBinding(RangeBase.ValueProperty, bAddSpeed);

            //var bTestMode = new Binding { Source = Settings.Default, Path = new PropertyPath("LocalMode") };
            //ckTestMode.SetBinding(ToggleButton.IsCheckedProperty, bTestMode);

        }

        public TextBlockAttribute CreateNewRandomAttr(bool? vip = false, int quadrant = 0)
        {

            //var leftMargin = Settings.Default.LeftMargin;
            var leftMargin = _quadSize*quadrant; 
            var rightMargin = _canvasWidth - Settings.Default.RightMargin;
            var maxFontSize = vip.GetValueOrDefault() ? Settings.Default.MaxFontSizeVIP : Settings.Default.MaxFontSize;
            var minFontSize = vip.GetValueOrDefault() ? Settings.Default.MinFontSizeVIP : Settings.Default.MinFontSize;

            var rnd = new Random();
            var xAxis = rnd.Next(leftMargin.ToInt(), rightMargin.ToInt());
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

        #region Events

        private void ConnOnStateChanged(StateChange stateChange)
        {
            this.Dispatcher.Invoke(() => ConnectionStatus.Text = Conn.State.ToString());
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _currentPerson = _personList.Next(_currentPerson);
            var attr = CreateNewRandomAttr(_currentPerson.IsVIP);
            CreateNewTextBlock(_currentPerson, attr);
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
            Settings.Default.Reset();
            Settings.Default.Reload();
        }

        private void BtnCloseApp_OnClick(object sender, RoutedEventArgs e)
        {
            Conn.Dispose();
            mainWindow.Close();
        }

        private void sldAddSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Settings.Default.ItemAddSpeed = e.NewValue;
            if (timer != null) timer.Interval = TimeSpan.FromSeconds(e.NewValue);
        }

        private void SldScrollSpeed_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Settings.Default.DefaultScrollSpeed = e.NewValue.ToInt();
        }

        private void SldMaxFont_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Settings.Default.MinFontSize > Settings.Default.MaxFontSize)
            {
                Settings.Default.MinFontSize = Settings.Default.MinFontSize - 1;
            }
        }

        private void SldMinFont_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Settings.Default.MaxFontSize < Settings.Default.MinFontSize)
            {
                Settings.Default.MaxFontSize = Settings.Default.MaxFontSize + 1;
            }
        }

        private void SldMinFontSizeVIP_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Settings.Default.MaxFontSizeVIP < Settings.Default.MinFontSizeVIP)
            {
                Settings.Default.MaxFontSizeVIP = Settings.Default.MaxFontSizeVIP + 1;
            }
        }

        private void SldMaxFontSizeVIP_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Settings.Default.MinFontSizeVIP > Settings.Default.MaxFontSizeVIP)
            {
                Settings.Default.MinFontSizeVIP = Settings.Default.MinFontSizeVIP - 1;
            }
        }

        #endregion


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
