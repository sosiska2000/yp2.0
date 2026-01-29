using System.Windows;
using System.Windows.Controls;

namespace EquipmentManagement.Client
{
    public partial class MainWindow : Window
    {
        public static MainWindow init;
        public static Pages.MainPage mainPage;
        public Models.Users CurrentUser { get; private set; }

        public MainWindow() : this("Default") { }
        public MainWindow(string role)
        {
            InitializeComponent();
            init = this;
            OpenPages(new Pages.MainPage());
        }
        public void OpenPages(Page page)
        {
            frame.Navigate(page);
        }
        public void SetCurrentUser(Models.Users user)
        {
            CurrentUser = user;
        }
    }
}