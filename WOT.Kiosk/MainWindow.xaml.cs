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

namespace WOT.Kiosk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //tbFirstName.SizeChanged += new SizeChangedEventHandler(tb_SizeChanged);
            //tbFirstName.TextChanged += tbTextChanged;
        }

        private void tbTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            RecalcFontSize();
        }

        void tb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalcFontSize();
        }

        private void RecalcFontSize()
        {
            if (tbFirstName == null) return;
            Size constraint = new Size(tbFirstName.ActualWidth, tbFirstName.ActualHeight);
            tbFirstName.Measure(constraint);
            while (tbFirstName.DesiredSize.Height < tbFirstName.ActualHeight)
            {
                tbFirstName.FontSize += 1;
                tbFirstName.Measure(constraint);
            }
            tbFirstName.FontSize -= 1;
        }

        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalcFontSize();
        }
    }


}
