using System.Windows;
using System.Windows.Controls;
using Humanizer;
using WOT.Kiosk.Models;

namespace WOT.Kiosk
{
    public partial class AddNameStep2 : Page
    {
        private readonly Person _person; 
        public AddNameStep2(Person person)
        {
            InitializeComponent();
            _person = person;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            _person.Lastname = _person.Lastname.Transform(To.LowerCase, To.TitleCase);
            _person.Firstname = _person.Firstname.Transform(To.LowerCase, To.TitleCase);
            _person.ZipCode = tbZipCode.Text;
            NavigationService.Navigate(new AddNameStep3(_person));
        }

        private void key_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)e.OriginalSource;
            var s = btn.Content.ToString();

            if (s == "DEL" && tbZipCode.Text.Length > 0)
            {
                tbZipCode.Text = tbZipCode.Text.Remove(tbZipCode.Text.Length - 1, 1);
                return;
            }

            if (tbZipCode.Text.Length < tbZipCode.MaxLength && s != "DEL")
            {
                tbZipCode.Text += s;                
            }
        }

    }
}
