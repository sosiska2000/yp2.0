using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace EquipmentManagement.Client.Pages.Audiences
{
    public partial class Audiences : Page
    {
        private readonly HttpClient _httpClient;
        private List<Models.Auditorium> _allAudiences = new List<Models.Auditorium>();
        private List<Models.User> _allUsers = new List<Models.User>();
        private bool _sortAscending = true;
        
        public Audiences()
        {
            InitializeComponent();
            
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:5001"); // Ваш URL API
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            Loaded += Audiences_Loaded;
        }

        private async void Audiences_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
            
            // Проверяем права пользователя (показываем кнопку добавления только админам)
            CheckUserPermissions();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Загружаем аудитории
                var audiencesResponse = await _httpClient.GetAsync("/api/Auditorii/list");
                if (audiencesResponse.IsSuccessStatusCode)
                {
                    var json = await audiencesResponse.Content.ReadAsStringAsync();
                    _allAudiences = JsonConvert.DeserializeObject<List<Models.Auditorium>>(json) ?? new List<Models.Auditorium>();
                }
                
                // Загружаем пользователей для отображения ФИО
                var usersResponse = await _httpClient.GetAsync("/api/Polzovateli/list");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var json = await usersResponse.Content.ReadAsStringAsync();
                    _allUsers = JsonConvert.DeserializeObject<List<Models.User>>(json) ?? new List<Models.User>();
                }
                
                // Обновляем DataGrid
                UpdateDataGrid();
                
                // Обновляем статистику
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateDataGrid()
        {
            // Создаем коллекцию для DataGrid с дополнительными полями
            var itemsForGrid = _allAudiences.Select(a => new
            {
                Id = a.Id,
                Name = a.Nazvanie,
                ShortName = a.SokrashennoeNazvanie ?? "",
                ResponsibleUserId = a.OtvetstvennyiPolzovatelId,
                TempResponsibleUserId = a.VremennoOtvetstvennyiPolzovatelId,
                ResponsibleUserName = GetUserName(a.OtvetstvennyiPolzovatelId),
                TempResponsibleUserName = GetUserName(a.VremennoOtvetstvennyiPolzovatelId),
                EquipmentCount = 0 // Пока заглушка, можно получить из API /api/Auditorii/equipment-count/{id}
            }).ToList();
            
            auditoriesGrid.ItemsSource = itemsForGrid;
            
            // Показываем/скрываем сообщение о пустоте
            if (itemsForGrid.Any())
            {
                auditoriesGrid.Visibility = Visibility.Visible;
                emptyMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                auditoriesGrid.Visibility = Visibility.Collapsed;
                emptyMessage.Visibility = Visibility.Visible;
            }
        }

        private string GetUserName(int? userId)
        {
            if (!userId.HasValue || userId.Value == 0) return "Не назначен";
            
            var user = _allUsers.FirstOrDefault(u => u.Id == userId.Value);
            return user != null ? $"{user.Familiia} {user.Imia} {user.Otchestvo}".Trim() : "Не найден";
        }

        private void UpdateStatistics()
        {
            var total = _allAudiences.Count;
            var withEquipment = _allAudiences.Count; // Заглушка, можно получить реальные данные
            
            // Находим TextBlock'и по имени или используем binding
            // Временно просто выводим в заголовок
            this.Title = $"Аудитории ({total} всего)";
        }

        private void CheckUserPermissions()
        {
            // Здесь нужно реализовать проверку роли пользователя
            // Пока показываем кнопку всем
            addBtn.Visibility = Visibility.Visible;
        }

        // ПОИСК
        private async void KeyDown_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await SearchAudiencesAsync();
            }
        }

        private async Task SearchAudiencesAsync()
        {
            var searchText = search.Text.Trim();
            
            try
            {
                var response = await _httpClient.GetAsync($"/api/Auditorii/list?search={Uri.EscapeDataString(searchText)}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _allAudiences = JsonConvert.DeserializeObject<List<Models.Auditorium>>(json) ?? new List<Models.Auditorium>();
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
            SortAudiences();
        }

        private void SortDown(object sender, RoutedEventArgs e)
        {
            _sortAscending = false;
            SortAudiences();
        }

        private void SortAudiences()
        {
            if (_sortAscending)
            {
                _allAudiences = _allAudiences.OrderBy(a => a.Nazvanie).ToList();
            }
            else
            {
                _allAudiences = _allAudiences.OrderByDescending(a => a.Nazvanie).ToList();
            }
            
            UpdateDataGrid();
        }

        // ДЕЙСТВИЯ С КНОПКАМИ В ТАБЛИЦЕ
        private async void ViewEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is dynamic item)
            {
                int audienceId = item.Id;
                
                try
                {
                    var response = await _httpClient.GetAsync($"/api/Auditorii/equipment/{audienceId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var equipment = JsonConvert.DeserializeObject<List<dynamic>>(json);
                        
                        // Показываем диалог с оборудованием
                        var dialog = new EquipmentInAudienceDialog(audienceId, item.Name, equipment);
                        dialog.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки оборудования: {ex.Message}", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is dynamic item)
            {
                var auditorium = _allAudiences.FirstOrDefault(a => a.Id == item.Id);
                if (auditorium != null)
                {
                    NavigationService?.Navigate(new Add(auditorium));
                }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is dynamic item)
            {
                int audienceId = item.Id;
                string audienceName = item.Name;
                
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить аудиторию \"{audienceName}\"?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"/api/Auditorii/delete/{audienceId}");
                        
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Аудитория успешно удалена", "Успех", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            await LoadDataAsync();
                        }
                        else
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Ошибка удаления: {error}", "Ошибка", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
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

        // Вспомогательный класс для диалога оборудования
        private class EquipmentInAudienceDialog : Window
        {
            public EquipmentInAudienceDialog(int audienceId, string audienceName, List<dynamic> equipment)
            {
                Title = $"Оборудование в аудитории: {audienceName}";
                Width = 600;
                Height = 400;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                
                var grid = new Grid();
                
                var dataGrid = new DataGrid
                {
                    AutoGenerateColumns = true,
                    ItemsSource = equipment,
                    Margin = new Thickness(10)
                };
                
                grid.Children.Add(dataGrid);
                Content = grid;
            }
        }
    }
}