using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace EquipmentManagement.Client.Pages.Developers
{
    public partial class Item : UserControl
    {
        private readonly HttpClient _httpClient;
        private Models.Developer _developer;

        // События для взаимодействия с родительской страницей
        public event EventHandler? EditClicked;
        public event EventHandler? DeleteClicked;
        public event EventHandler? ViewProgramsClicked;

        public Item(Models.Developer developer)
        {
            InitializeComponent();

            _httpClient = App.HttpClient;
            _developer = developer;

            LoadDeveloperData();
            CheckUserPermissions();
        }

        private void LoadDeveloperData()
        {
            lb_Name.Content = _developer.Nazvanie;
        }

        private void CheckUserPermissions()
        {
            // Здесь проверяем роль текущего пользователя
            // bool isAdmin = CheckIfCurrentUserIsAdmin();
            // buttons.Visibility = isAdmin ? Visibility.Visible : Visibility.Hidden;

            buttons.Visibility = Visibility.Visible;
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, EventArgs.Empty);
        }

        private async void Click_remove(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить разработчика \"{_developer.Nazvanie}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DeleteClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        // Свойства для доступа к данным
        public int DeveloperId => _developer.Id;
        public string DeveloperName => _developer.Nazvanie;
    }
}