using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace EquipmentManagement.Client.Pages.Audiences
{
    public partial class Add : Page
    {
        private readonly HttpClient _httpClient;
        private List<Models.User> _users = new List<Models.User>();
        private Models.Auditorium? _editingAuditorium;

        public Add(Models.Auditorium? auditoriumToEdit = null)
        {
            InitializeComponent();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:5001"); // Ваш URL API
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            _editingAuditorium = auditoriumToEdit;

            Loaded += Add_Loaded;
        }

        private async void Add_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();

            if (_editingAuditorium != null)
            {
                LoadAuditoriumData();
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Polzovateli/list");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _users = JsonConvert.DeserializeObject<List<Models.User>>(json) ?? new List<Models.User>();

                    // Настраиваем ComboBox для ответственного
                    tb_User.Items.Clear();
                    tb_User.Items.Add(new Models.User { Id = 0, Familiia = "Не назначен" });
                    foreach (var user in _users)
                    {
                        tb_User.Items.Add(user);
                    }

                    // Настраиваем ComboBox для временно ответственного
                    tb_tempUser.Items.Clear();
                    tb_tempUser.Items.Add(new Models.User { Id = 0, Familiia = "Не назначен" });
                    foreach (var user in _users)
                    {
                        tb_tempUser.Items.Add(user);
                    }

                    tb_User.DisplayMemberPath = "FullName";
                    tb_tempUser.DisplayMemberPath = "FullName";
                }
                else
                {
                    MessageBox.Show($"Ошибка загрузки пользователей: {response.StatusCode}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAuditoriumData()
        {
            if (_editingAuditorium == null) return;

            text1.Content = "Редактирование аудитории";
            text2.Content = "Сохранить";

            tb_Name.Text = _editingAuditorium.Nazvanie;
            tb_shortName.Text = _editingAuditorium.SokrashennoeNazvanie ?? "";

            // Устанавливаем выбранного ответственного пользователя
            if (_editingAuditorium.OtvetstvennyiPolzovatelId.HasValue && _editingAuditorium.OtvetstvennyiPolzovatelId.Value > 0)
            {
                var respUser = _users.FirstOrDefault(u => u.Id == _editingAuditorium.OtvetstvennyiPolzovatelId.Value);
                if (respUser != null)
                {
                    tb_User.SelectedItem = respUser;
                }
            }
            else
            {
                tb_User.SelectedIndex = 0;
            }

            // Устанавливаем выбранного временно ответственного пользователя
            if (_editingAuditorium.VremennoOtvetstvennyiPolzovatelId.HasValue && _editingAuditorium.VremennoOtvetstvennyiPolzovatelId.Value > 0)
            {
                var tempUser = _users.FirstOrDefault(u => u.Id == _editingAuditorium.VremennoOtvetstvennyiPolzovatelId.Value);
                if (tempUser != null)
                {
                    tb_tempUser.SelectedItem = tempUser;
                }
            }
            else
            {
                tb_tempUser.SelectedIndex = 0;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(tb_Name.Text))
            {
                MessageBox.Show("Поле 'Наименование' обязательно для заполнения", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_Name.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tb_shortName.Text))
            {
                MessageBox.Show("Поле 'Сокращенное наименование' обязательно для заполнения", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_shortName.Focus();
                return false;
            }

            return true;
        }

        private async void Click_Redact(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                // Определяем ID выбранных пользователей
                int? respUserId = null;
                int? tempRespUserId = null;

                if (tb_User.SelectedItem is Models.User selectedRespUser && selectedRespUser.Id > 0)
                {
                    respUserId = selectedRespUser.Id;
                }

                if (tb_tempUser.SelectedItem is Models.User selectedTempUser && selectedTempUser.Id > 0)
                {
                    tempRespUserId = selectedTempUser.Id;
                }

                if (_editingAuditorium == null)
                {
                    // СОЗДАНИЕ новой аудитории
                    var newAuditorium = new
                    {
                        Nazvanie = tb_Name.Text.Trim(),
                        SokrashennoeNazvanie = tb_shortName.Text.Trim(),
                        OtvetstvennyiPolzovatelId = respUserId,
                        VremennoOtvetstvennyiPolzovatelId = tempRespUserId
                    };

                    var json = JsonConvert.SerializeObject(newAuditorium);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("/api/Auditorii/create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Аудитория успешно создана", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Очищаем форму
                        tb_Name.Clear();
                        tb_shortName.Clear();
                        tb_User.SelectedIndex = 0;
                        tb_tempUser.SelectedIndex = 0;
                        tb_Name.Focus();
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка создания: {error}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // ОБНОВЛЕНИЕ существующей аудитории
                    var updateAuditorium = new
                    {
                        Id = _editingAuditorium.Id,
                        Nazvanie = tb_Name.Text.Trim(),
                        SokrashennoeNazvanie = tb_shortName.Text.Trim(),
                        OtvetstvennyiPolzovatelId = respUserId,
                        VremennoOtvetstvennyiPolzovatelId = tempRespUserId
                    };

                    var json = JsonConvert.SerializeObject(updateAuditorium);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"/api/Auditorii/update/{_editingAuditorium.Id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Аудитория успешно обновлена", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Возвращаемся назад
                        NavigationService?.Navigate(new Audiences());
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка обновления: {error}", "Ошибка",
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

        private void Click_Cancel_Redact(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Audiences());
        }
    }
}