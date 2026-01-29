using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
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
using EquipmentManagement.Client.Models;

namespace EquipmentManagement.Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Models.Users currentUser;
        public MainPage()
        {
            InitializeComponent();
            currentUser = MainWindow.init.CurrentUser;
            if (currentUser != null && currentUser.Role == "Администратор")
            {
                users.Visibility = Visibility.Visible;
            }
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new MainPage());
        }

        private void OpenEquip(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Equipment.Equipment());
        }

        private void OpenEqType(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new EquipmentType.EquipmentType());
        }

        private void OpenAudien(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Audiences.Audiences());
        }

        private void OpenProgr(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Programs.Programs());
        }

        private void OpenDevel(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Developers.Developers());
        }

        private void OpenRMat(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Materials.Materials());
        }

        private void OpenCharRMat(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Сharacteristic.Characteristic());
        }

        private void OpenModType(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new ViewModel.ViewModel());
        }

        private void OpenInvent(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Inventory.Inventory());
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new NetworkSettings.NetworkSettings());
        }

        private void OpenUsers(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Users.Users());
        }

        private void OpenDirection(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Direction.Direction());
        }

        private void OpenStatus(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new Status.Status());
        }

        private void OpenTypeChar(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new СharacteristicType.СharacteristicType());
        }

        private void OpenValueChar(object sender, RoutedEventArgs e)
        {
            MainWindow.init.OpenPages(new СharacteristicValue.СharacteristicValue());
        }
    }
}
