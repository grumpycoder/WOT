using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
        #region Variables

        private static readonly Random random = new Random();
        private const int TopMargin = 10;

        private Person _currentPersonVIP;
        private Person _currentPerson1;
        private Person _currentPerson2;
        private Person _currentPerson3;
        private Person _currentPerson4;
        private readonly IList<Person> _personList;
        private readonly IList<Person> _personVIPList;

        private readonly Canvas _canvas;
        private readonly double _canvasWidth;
        private readonly double _canvasHeight;
        private readonly double _quadSize;
        private const double SpeedModifier = 10;

        private readonly DispatcherTimer _timerVIP;
        private readonly DispatcherTimer _timer1;
        private readonly DispatcherTimer _timer2;
        private readonly DispatcherTimer _timer3;
        private readonly DispatcherTimer _timer4;

        public string ServerURI = Settings.Default.ServerURI;
        public string HubName = Settings.Default.HubName;

        public IHubProxy HubProxy { get; set; }
        public HubConnection Conn { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                            typeof(Timeline),
                            new FrameworkPropertyMetadata { DefaultValue = 80 }
                            );
            _canvas = WallCanvas;
            _canvas.Height = SystemParameters.PrimaryScreenHeight;
            _canvas.Width = SystemParameters.PrimaryScreenWidth;

            SetDataBindings();
            _canvas.UpdateLayout();

            _canvasWidth = _canvas.Width;
            _canvasHeight = _canvas.Height;
            _quadSize = _canvasWidth / 4;
            ExpanderSettings.Width = _canvasWidth;

            // Populate list of people
            var db = new AppContext();

            //TODO: Create VIP list to run on seperate timer
            //_personVIPList = CreateLocalPersonList();
            _personVIPList = db.Persons.Where(x => x.IsVIP == true).ToList();
            _personList = db.Persons.Where(x => x.IsVIP == false).ToList();

            // Setup timers to add person names to display
            _timerVIP = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.ItemAddSpeed)};
            _timerVIP.Tick += (s, args) => timer_Tick(_timerVIP, ref _currentPersonVIP);

            _timer1 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.ItemAddSpeed), Tag = 1 };
            _timer1.Tick += (s, args) => timer_Tick(_timer1, ref _currentPerson1);
            _timer2 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.ItemAddSpeed), Tag = 2 };
            _timer2.Tick += (s, args) => timer_Tick(_timer2, ref _currentPerson2);
            _timer3 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.ItemAddSpeed), Tag = 3 };
            _timer3.Tick += (s, args) => timer_Tick(_timer3, ref _currentPerson3);
            _timer4 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.ItemAddSpeed), Tag = 4 };
            _timer4.Tick += (s, args) => timer_Tick(_timer4, ref _currentPerson4);

            _timerVIP.Start();
            _timer1.Start();
            _timer2.Start();
            _timer3.Start();
            _timer4.Start();
            //TODO: Remove this test
            AddNewNameToDisplay(new Person() { Firstname = "Mark", Lastname = "Lawrence Lawrence Lawrence" }, 4);

            ConnectAsync();
        }

        public void AddNewNameToDisplay(Person person, int quadrant)
        {
            var minFontSize = Settings.Default.MinFontSize + (Settings.Default.MinFontSize * .10).ToInt();
            var maxFontSize = Settings.Default.MaxFontSize + (Settings.Default.MaxFontSize * .10).ToInt();
            var speed = ((Settings.Default.DefaultScrollSpeed / (double)minFontSize) * SpeedModifier).ToInt();

            var rightMargin = (_canvasWidth / 4 * quadrant).ToInt();
            var leftMargin = (rightMargin - _quadSize).ToInt();
            var left = RandomNumber(leftMargin, rightMargin);
            var midPoint = _canvasHeight / 2;

            // Create a name scope for the page.
            NameScope.SetNameScope(this, new NameScope());
            var myLabel = new Label() { Content = person.ToString(), FontSize = maxFontSize, Name = "label1", Foreground = new SolidColorBrush(Colors.White) };
            this.RegisterName(myLabel.Name, myLabel);

            // Correct left position if name is too long to fit within canvas right margin
            myLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            myLabel.Arrange(new Rect(myLabel.DesiredSize));
            var w = myLabel.ActualWidth;
            if ((left + w) > _canvasWidth)
            {
                left = left > (_canvasWidth - w).ToInt() ? (_canvasWidth - w).ToInt() : RandomNumber(leftMargin, (_canvasWidth - w).ToInt());
            }

            myLabel.FontSize = 1;

            var growAnimation = new DoubleAnimation
            {
                From = 1,
                To = maxFontSize,
                Duration = new Duration(TimeSpan.FromSeconds(3)),
                BeginTime = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTargetName(growAnimation, myLabel.Name);
            Storyboard.SetTargetProperty(growAnimation, new PropertyPath(FontSizeProperty));

            var shrinkAnimation = new DoubleAnimation
            {
                From = maxFontSize,
                To = maxFontSize - maxFontSize * .20,
                BeginTime = TimeSpan.FromSeconds(5),
                Duration = new Duration(TimeSpan.FromSeconds(5))
            };
            Storyboard.SetTargetName(shrinkAnimation, myLabel.Name);
            Storyboard.SetTargetProperty(shrinkAnimation, new PropertyPath(FontSizeProperty));

            var upAnimation = new DoubleAnimation
            {
                From = midPoint,
                To = TopMargin,
                BeginTime = TimeSpan.FromSeconds(5),
                Duration = new Duration(TimeSpan.FromSeconds(5))
            };
            Storyboard.SetTargetName(upAnimation, myLabel.Name);
            Storyboard.SetTargetProperty(upAnimation, new PropertyPath(TopProperty));

            var mySolidColorBrush = new SolidColorBrush { Color = Colors.White };
            this.RegisterName("mySolidColorBrush", mySolidColorBrush);

            myLabel.Foreground = mySolidColorBrush;

            var colorAnimation = new ColorAnimation()
            {
                From = RandomColor(),
                To = RandomColor(),
                BeginTime = TimeSpan.FromSeconds(5),
                Duration = new Duration(TimeSpan.FromSeconds(5))
            };
            Storyboard.SetTargetName(colorAnimation, "mySolidColorBrush");
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));


            var fallAnimation = new DoubleAnimation
            {
                From = TopMargin,
                To = _canvasHeight,
                BeginTime = TimeSpan.FromSeconds(10),
                Duration = new Duration(TimeSpan.FromSeconds(speed))
            };
            Storyboard.SetTargetName(fallAnimation, myLabel.Name);
            Storyboard.SetTargetProperty(fallAnimation, new PropertyPath(TopProperty));

            var myStoryboard = new Storyboard();
            myStoryboard.Children.Add(growAnimation);
            myStoryboard.Children.Add(shrinkAnimation);
            myStoryboard.Children.Add(colorAnimation);
            myStoryboard.Children.Add(upAnimation);
            myStoryboard.Children.Add(fallAnimation);

            Canvas.SetLeft(myLabel, left);

            Canvas.SetTop(myLabel, midPoint);
            Panel.SetZIndex(myLabel, 9999);
            _canvas.Children.Add(myLabel);
            _canvas.UpdateLayout();

            myStoryboard.Begin(this);

        }

        private void AddNameToDisplay(Person person, int quadrant)
        {
            var maxFontSize = person.IsVIP.GetValueOrDefault() ? Settings.Default.MaxFontSizeVIP : Settings.Default.MaxFontSize;
            var minFontSize = person.IsVIP.GetValueOrDefault() ? Settings.Default.MinFontSizeVIP : Settings.Default.MinFontSize;
            var name = "label" + RandomNumber(1, 1000);

            // Create a name scope for the page.
            NameScope.SetNameScope(this, new NameScope());

            var label = new Label()
            {
                Content = person.ToString(),
                FontSize = RandomNumber(minFontSize, maxFontSize),
                Name = name,
                Tag = name,
                Uid = name,
                Foreground = new SolidColorBrush(RandomColor())
            };
            var speed = ((Settings.Default.DefaultScrollSpeed / (double)label.FontSize) * SpeedModifier).ToInt();
            this.RegisterName(label.Name, label);

            var rightMargin = quadrant == 0 ? _canvasWidth.ToInt() : (_canvasWidth / 4 * quadrant).ToInt();
            var leftMargin = (rightMargin - _quadSize).ToInt();
            if (quadrant == 0)
            {
                leftMargin = 0;
            }

            // Required to calculate actual size to determine overflow off viewable area
            label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            label.Arrange(new Rect(label.DesiredSize));

            var width = label.ActualWidth;
            var leftPos = RandomNumber(leftMargin, rightMargin);
            if (leftPos + width > _canvasWidth)
            {
                leftPos = RandomNumber(leftMargin, (_canvasWidth - width).ToInt());
            }

            var fallAnimation = new DoubleAnimation
            {
                From = TopMargin,
                To = _canvasHeight,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(speed))
            };
            Storyboard.SetTargetName(fallAnimation, label.Name);
            Storyboard.SetTargetProperty(fallAnimation, new PropertyPath(TopProperty));

            var storyboard = new Storyboard();
            storyboard.Children.Add(fallAnimation);

            var e = new AnimationEventArgs { TagName = label.Uid };
            storyboard.Completed += (sender, args) => StoryboardOnCompleted(e);
            Canvas.SetLeft(label, leftPos);

            Canvas.SetTop(label, TopMargin);
            _canvas.Children.Add(label);
            _canvas.UpdateLayout();

            storyboard.Begin(this);
        }

        public void AddPersonToDisplayFromKiosk(string name, string location)
        {
            var firstname = name.Split(' ')[0];
            var lastname = name.Split(' ')[1];
            var person = new Person
            {
                Firstname = firstname,
                Lastname = lastname,
                Id = _personList.Count + 1,
                IsVIP = true
            };

            _personList.Add(person);
            int quad;
            int.TryParse(location, out quad);
            AddNewNameToDisplay(person, quad);
        }


        public ScreenTextBlock GenerateDisplayTextBlock(Person person, int quadrant)
        {
            var rightMargin = (_canvasWidth / 4 * quadrant).ToInt();
            var leftMargin = (rightMargin - _quadSize).ToInt();
            var maxFontSize = person.IsVIP.GetValueOrDefault() ? Settings.Default.MaxFontSizeVIP : Settings.Default.MaxFontSize;
            var minFontSize = person.IsVIP.GetValueOrDefault() ? Settings.Default.MinFontSizeVIP : Settings.Default.MinFontSize;
            var size = RandomNumber(minFontSize, maxFontSize);
            var speed = ((Settings.Default.DefaultScrollSpeed / (double)size) * SpeedModifier).ToInt();
            var name = Regex.Replace(person.Lastname + person.Id.ToString(), @"[^A-Za-z]+", "");

            var t = new ScreenTextBlock
            {
                TextBlock = new Label()
                {
                    Name = Regex.Replace(person.Lastname + person.Id.ToString(), @"[^A-Za-z]+", ""),
                    Uid = Regex.Replace(person.Lastname + person.Id.ToString(), @"[^A-Za-z]+", ""),
                    Content = person.ToString(),
                    Tag = name,
                    FontSize = RandomNumber(minFontSize, maxFontSize),
                    FontWeight = FontWeights.Normal,
                    Foreground = new SolidColorBrush(RandomColor())
                },
                Left = RandomNumber(leftMargin, rightMargin),
                Top = TopMargin,
                Animation = CreateAnimation(speed, name)
            };

            // Correct left position if name is too long to fit within canvas right margin
            t.TextBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            t.TextBlock.Arrange(new Rect(t.TextBlock.DesiredSize));
            var w = t.TextBlock.ActualWidth;
            if ((t.Left + w) > _canvasWidth)
            {
                t.Left = RandomNumber(leftMargin, (_canvasWidth - w).ToInt());
            }

            return t;
        }

        private void AddTextBlockToDisplay(ScreenTextBlock screenTextBlock)
        {
            Canvas.SetLeft(screenTextBlock.TextBlock, screenTextBlock.Left);
            Canvas.SetTop(screenTextBlock.TextBlock, screenTextBlock.Top);
            Panel.SetZIndex(screenTextBlock.TextBlock, screenTextBlock.ZIndex);
            _canvas.Children.Add(screenTextBlock.TextBlock);

            screenTextBlock.TextBlock.BeginAnimation(TopProperty, screenTextBlock.Animation);
        }

        private DoubleAnimation CreateAnimation(int speed, string textBlockName)
        {
            var animation = new DoubleAnimation
            {
                From = TopMargin,
                To = _canvasHeight,
                Duration = new Duration(TimeSpan.FromSeconds(speed))
            };
            var e = new AnimationEventArgs { TagName = textBlockName };
            animation.Completed += (sender, args) => AnimationOnCompleted(e);

            return animation;
        }


        public int RandomNumber(int min, int max)
        {
            return random.Next(min, max);
        }

        private Color RandomColor()
        {
            var r = Convert.ToByte(RandomNumber(0, 255));
            var g = Convert.ToByte(RandomNumber(0, 255));
            var b = Convert.ToByte(RandomNumber(0, 255));
            var color = Color.FromRgb(r, g, b);
            return color;
        }

        private async void ConnectAsync()
        {
            Conn = new HubConnection(ServerURI);
            Conn.StateChanged += ConnOnStateChanged;
            HubProxy = Conn.CreateHubProxy(HubName);

            HubProxy.On<string, string>("sendName", (name, message) => Dispatcher.Invoke(() => AddPersonToDisplayFromKiosk(message, name)));
            try
            {
                await Conn.Start();
            }
            catch (HttpRequestException e)
            {
                //TODO: Log connection error
                Debug.WriteLine("Unable to connect to server: Start server before connecting clients");
                Debug.WriteLine(e.InnerException.ToString());
                return;
            }
            catch (HttpClientException ce)
            {
                //TODO: Log connection error
                Debug.WriteLine("Unable to connect to server: Start server before connecting clients");
                Debug.WriteLine(ce.ToString());
                return;
            }
            Debug.WriteLine("Connected to server at {0}", ServerURI);
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

        #region Events

        private void ConnOnStateChanged(StateChange stateChange)
        {
            Dispatcher.Invoke(() => ConnectionStatus.Text = Conn.State.ToString());
        }

        void timer_Tick(object sender, ref Person person)
        {
            var dt = (DispatcherTimer)(sender);
            if (dt == null) return;

            // Determine timer quadrant of screen responsible
            var quad = Convert.ToInt32(dt.Tag);

            person = dt.Tag == null ? _personVIPList.Next(person) : _personList.Next(person, (_personList.Count - 1)/4*quad);

            // Add name to screen
            AddNameToDisplay(person, quad);

            //var textBlock = GenerateDisplayTextBlock(person, quad);
            //AddTextBlockToDisplay(textBlock);
        }

        private void StoryboardOnCompleted(AnimationEventArgs eventArgs)
        {
            var tagName = eventArgs.TagName;

            foreach (UIElement child in _canvas.Children)
            {
                if (tagName != child.Uid) continue;
                child.BeginAnimation(TopProperty, null);
                _canvas.Children.Remove(child);
                return;
            }
        }

        private void AnimationOnCompleted(AnimationEventArgs eventArgs)
        {
            var tagName = eventArgs.TagName;

            var result = _canvas.Children.Cast<FrameworkElement>().First(x => x.Uid != null && x.Uid.ToString() == tagName);

            result.BeginAnimation(TopProperty, null);
            _canvas.Children.Remove(result);
            //GC.Collect();
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
            if (_timer1 != null) _timer1.Interval = TimeSpan.FromSeconds(e.NewValue);
            if (_timer2 != null) _timer2.Interval = TimeSpan.FromSeconds(e.NewValue);
            if (_timer3 != null) _timer3.Interval = TimeSpan.FromSeconds(e.NewValue);
            if (_timer4 != null) _timer4.Interval = TimeSpan.FromSeconds(e.NewValue);
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
}
