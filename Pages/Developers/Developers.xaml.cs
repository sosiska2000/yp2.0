using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

namespace EquipmentManagement.Client.Pages.Developers
{
    public partial class Developers : Page
    {
        private readonly HttpClient _httpClient;
        private List<Models.Developer> _allDevelopers = new List<Models.Developer>();
        private bool _sortAscending = true;

        public Developers()
        {
            InitializeComponent();

            _httpClient = App.HttpClient;

            Loaded += Developers_Loaded;
        }

        private async void Developers_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDevelopersAsync();
            CheckUserPermissions();
        }

        private async Task LoadDevelopersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Razrabotchiki/list");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _allDevelopers = JsonConvert.DeserializeObject<List<Models.Developer>>(json) ?? new List<Models.Developer>();
                }

                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDataGrid()
        {
            developersGrid.ItemsSource = _allDevelopers;

            // Показываем/скрываем сообщение о пустоте
            if (_allDevelopers.Any())
            {
                developersGrid.Visibility = Visibility.Visible;
                emptyMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                developersGrid.Visibility = Visibility.Collapsed;
                emptyMessage.Visibility = Visibility.Visible;
            }

            // Обновляем статистику
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            var total = _allDevelopers.Count;
            // Можно добавить статистику по программам и т.д.
        }

        private void CheckUserPermissions()
        {
            // Здесь проверяем роль пользователя
            // bool isAdmin = CheckIfCurrentUserIsAdmin();
            // addBtn.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            addBtn.Visibility = Visibility.Visible;
        }

        // ПОИСК
        private async void KeyDown_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await SearchDevelopersAsync();
            }
        }

        private async Task SearchDevelopersAsync()
        {
            var searchText = search.Text.Trim();

            try
            {
                var response = await _httpClient.GetAsync($"/api/Razrabotchiki/list?search={Uri.EscapeDataString(searchText)}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _allDevelopers = JsonConvert.DeserializeObject<List<Models.Developer>>(json) ?? new List<Models.Developer>();
                    UpdateDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // СОРТИРОВКА
        private void SortUp(object sender, RoutedEventArgs e)
        {
            _sortAscending = true;
            SortDevelopers();
        }

        private void SortDown(object sender, RoutedEventArgs e)
        {
            _sortAscending = false;
            SortDevelopers();
        }

        private void SortDevelopers()
        {
            if (_sortAscending)
            {
                _allDevelopers = _allDevelopers.OrderBy(d => d.Nazvanie).ToList();
            }
            else
            {
                _allDevelopers = _allDevelopers.OrderByDescending(d => d.Nazvanie).ToList();
            }

            UpdateDataGrid();
        }

        // ДЕЙСТВИЯ С КНОПКАМИ В ТАБЛИЦЕ
        private void ViewPrograms_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Models.Developer developer)
            {
                // Открываем окно с программами этого разработчика
                var programsWindow = new ProgramsByDeveloperWindow(developer.Id, developer.Nazvanie);
                programsWindow.ShowDialog();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Models.Developer developer)
            {
                NavigationService?.Navigate(new Add(developer));
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Models.Developer developer)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить разработчика \"{developer.Nazvanie}\"?\n" +
                    "При удалении разработчика все связанные программы также будут удалены!",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"/api/Razrabotchiki/delete/{developer.Id}");

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Разработчик успешно удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadDevelopersAsync();
                        }
                        else
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            if (error.Contains("связан"))
                            {
                                MessageBox.Show($"Нельзя удалить разработчика, так как у него есть связанные программы. Сначала удалите все программы этого разработчика.",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show($"Ошибка удаления: {error}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // НАВИГАЦИЯ
        private void Add(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Add());
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
        }

        // Класс для окна с программами разработчика
        private class ProgramsByDeveloperWindow : Window
        {
            public ProgramsByDeveloperWindow(int developerId, string developerName)
            {
                Title = $"Программы разработчика: {developerName}";
                Width = 600;
                Height = 400;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;

                LoadProgramsAsync(developerId).ConfigureAwait(false);

                var grid = new Grid();
                var dataGrid = new DataGrid
                {
                    AutoGenerateColumns = true,
                    Margin = new Thickness(10)
                };

                grid.Children.Add(dataGrid);
                Content = grid;
            }

            private async Task LoadProgramsAsync(int developerId)
            {
                try
                {
                    var httpClient = App.HttpClient;
                    var response = await httpClient.GetAsync($"/api/Programmy/list?razrabotchikId={developerId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var programs = JsonConvert.DeserializeObject<List<dynamic>>(json);

                        if (Content is Grid grid && grid.Children[0] is DataGrid dataGrid)
                        {
                            dataGrid.ItemsSource = programs;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки программ: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}