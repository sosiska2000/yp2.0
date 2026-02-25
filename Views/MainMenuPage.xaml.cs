using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EquipmentManagement.Client.Views
{
    public partial class MainMenuPage : Page
    {
        private readonly Frame _mainFrame;

        public MainMenuPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;

            foreach (var button in FindVisualChildren<Button>(this))
            {
                button.Click += Button_Click;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tag = button.Tag as string;

            switch (tag)
            {
                case "Classrooms": _mainFrame.Navigate(new Classrooms.ClassroomsPage(_mainFrame)); break;
                case "Directions": _mainFrame.Navigate(new Directions.DirectionsPage(_mainFrame)); break;
                case "Statuses": _mainFrame.Navigate(new Statuses.StatusesPage(_mainFrame)); break;
                case "EquipmentTypes": _mainFrame.Navigate(new EquipmentTypes.EquipmentTypesPage(_mainFrame)); break;
                case "Models": _mainFrame.Navigate(new Modelss.ModelsPage(_mainFrame)); break;
                case "SoftwareDevelopers": _mainFrame.Navigate(new SoftwareDevelopers.SoftwareDevelopersPage(_mainFrame)); break;
                case "Software": _mainFrame.Navigate(new Software.SoftwarePage(_mainFrame)); break;
                case "ConsumableTypes": _mainFrame.Navigate(new ConsumableTypes.ConsumableTypesPage(_mainFrame)); break;
                case "Users": _mainFrame.Navigate(new Users.UsersPage(_mainFrame)); break;
                case "Equipment": _mainFrame.Navigate(new Equipment.EquipmentPage(_mainFrame)); break;
                case "Consumables": _mainFrame.Navigate(new Consumables.ConsumablesPage(_mainFrame)); break;
                case "Inventories": _mainFrame.Navigate(new Inventory.InventoriesPage(_mainFrame)); break;
                case "NetworkSettings": _mainFrame.Navigate(new NetworkSettings.NetworkSettingsPage(_mainFrame)); break;
                case "Logout": _mainFrame.Navigate(new LoginPage(_mainFrame)); break;
            }
        }

        private System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}