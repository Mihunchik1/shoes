using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZOO_magazin.Pages;

namespace ZOO_magazin.Pages
{
    public partial class АuthorizationPage : Page
    {
        public АuthorizationPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var currentuser = App.Context.User
                .FirstOrDefault(p => p.Login == TBoxLogin.Text && p.Password == PboxPassword.Password);
            if (currentuser != null)
            {
                App.CurrentUser = currentuser;
                NavigationService.Navigate(new ProductPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.");
            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService.Navigate(new ProductPage());
        }
    }
}