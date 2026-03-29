using System.Windows;

namespace ZOO_magazin
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameMain.Navigate(new Pages.АuthorizationPage());
        }
    }
}