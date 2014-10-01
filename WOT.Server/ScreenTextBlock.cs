using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WOT.Server
{
    public class ScreenTextBlock
    {
        public ScreenTextBlock()
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

        public Label TextBlock { get; set; }
        public DoubleAnimation Animation { get; set; }
        public Storyboard Storyboard { get; set; }
    }
}