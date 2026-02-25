using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Views;

namespace EquipmentManagement.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage(MainFrame));
        }
    }
}