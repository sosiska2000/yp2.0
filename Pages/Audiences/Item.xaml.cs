using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace EquipmentManagement.Client.Pages.Audiences
{
    public partial class Item : UserControl
    {
        private readonly HttpClient _httpClient;
        private Models.Auditorium _auditorium;
        private Models.User? _responsibleUser;
        private Models.User? _tempResponsibleUser;

        // События для взаимодействия с родительской страницей
        public event EventHandler? EditClicked;
        public event EventHandler? DeleteClicked;
        public event EventHandler? ViewEquipmentClicked;

        public Item(Models.Auditorium auditorium)
        {
            InitializeComponent();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:5001");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            _auditorium = auditorium;

            Loaded += Item_Loaded;
        }

        private async void Item_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAuditoriumData();

            // Проверяем права пользователя (скрываем кнопки если не админ)
            CheckUserPermissions();
        }

        private async Task LoadAuditoriumData()
        {
            try
            {
                // Загружаем информацию об аудитории с пользователями
                var response = await _httpClient.GetAsync($"/api/Auditorii/item/{_auditorium.Id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var auditoriumData = JsonConvert.DeserializeObject<dynamic>(json);

                    if (auditoriumData != null)
                    {
                        // Обновляем интерфейс
                        lb_Name.Content = auditoriumData.Nazvanie;
                        lb_sokrName.Content = auditoriumData.SokrashennoeNazvanie ?? "Не указано";

                        // Загружаем информацию о пользователях если нужно
                        if (auditoriumData.OtvetstvennyiPolzovatelId != null)
                        {
                            var respUserId = (int?)auditoriumData.OtvetstvennyiPolzovatelId;
                            if (respUserId.HasValue && respUserId.Value > 0)
                            {
                                var userResp = await GetUserByIdAsync(respUserId.Value);
                                if (userResp != null)
                                {
                                    _responsibleUser = userResp;
                                    lb_User.Content = $"Ответственный: {userResp.FullName}";
                                }
                            }
                            else
                            {
                                lb_User.Content = "Ответственный: Не назначен";
                            }
                        }

                        if (auditoriumData.VremennoOtvetstvennyiPolzovatelId != null)
                        {
                            var tempRespUserId = (int?)auditoriumData.VremennoOtvetstvennyiPolzovatelId;
                            if (tempRespUserId.HasValue && tempRespUserId.Value > 0)
                            {
                                var userTempResp = await GetUserByIdAsync(tempRespUserId.Value);
                                if (userTempResp != null)
                                {
                                    _tempResponsibleUser = userTempResp;
                                    lb_tempUser.Content = $"Временно-ответственный: {userTempResp.FullName}";
                                }
                            }
                            else
                            {
                                lb_tempUser.Content = "Временно-ответственный: Не назначен";
                            }
                        }
                    }
                }
                else
                {
                    // Если не удалось загрузить подробные данные, используем базовые
                    lb_Name.Content = _auditorium.Nazvanie;
                    lb_sokrName.Content = _auditorium.SokrashennoeNazvanie ?? "Не указано";
                    lb_User.Content = "Ответственный: Загрузка...";
                    lb_tempUser.Content = "Временно-ответственный: Загрузка...";
                }
            }
            catch (Exception ex)
            {
                lb_Name.Content = _auditorium.Nazvanie;
                lb_sokrName.Content = _auditorium.SokrashennoeNazvanie ?? "Не указано";
                lb_User.Content = "Ошибка загрузки данных";
                lb_tempUser.Content = "Ошибка загрузки данных";

                // Логируем ошибку
                Console.WriteLine($"Ошибка загрузки данных аудитории: {ex.Message}");
            }
        }

        private async Task<Models.User?> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Polzovateli/item/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Models.User>(json);
                }
            }
            catch
            {
                // Игнорируем ошибки загрузки пользователя
            }

            return null;
        }

        private void CheckUserPermissions()
        {
            // Здесь проверяем роль текущего пользователя
            // bool isAdmin = CheckIfCurrentUserIsAdmin();
            // buttons.Visibility = isAdmin ? Visibility.Visible : Visibility.Hidden;

            // Пока показываем кнопки всем
            buttons.Visibility = Visibility.Visible;
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            EditClicked?.Invoke(this, EventArgs.Empty);
        }

        private async void Click_remove(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить аудиторию \"{_auditorium.Nazvanie}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DeleteClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        // Метод для обновления данных аудитории
        public void UpdateAuditorium(Models.Auditorium updatedAuditorium)
        {
            _auditorium = updatedAuditorium;
            LoadAuditoriumData().ConfigureAwait(false);
        }

        // Свойства для доступа к данным
        public int AuditoriumId => _auditorium.Id;
        public string AuditoriumName => _auditorium.Nazvanie;
    }
}