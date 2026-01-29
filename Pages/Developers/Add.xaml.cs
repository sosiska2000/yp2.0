using System;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace EquipmentManagement.Client.Pages.Developers
{
    public partial class Add : Page
    {
        private readonly HttpClient _httpClient;
        private Models.Developer? _editingDeveloper;

        public Add(Models.Developer? developerToEdit = null)
        {
            InitializeComponent();

            // Используем глобальный HttpClient из App.xaml.cs
            _httpClient = App.HttpClient;

            _editingDeveloper = developerToEdit;

            if (_editingDeveloper != null)
            {
                lb_title.Content = "Редактирование разработчика";
                bt_click.Content = "Сохранить";
                tb_Name.Text = _editingDeveloper.Nazvanie;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(tb_Name.Text))
            {
                MessageBox.Show("Введите название разработчика", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_Name.Focus();
                return false;
            }

            return true;
        }

        private async void Click_Redact(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                if (_editingDeveloper == null)
                {
                    // СОЗДАНИЕ нового разработчика
                    var newDeveloper = new
                    {
                        Nazvanie = tb_Name.Text.Trim()
                    };

                    var json = JsonConvert.SerializeObject(newDeveloper);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("/api/Razrabotchiki/create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Разработчик успешно создан", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Очищаем форму
                        tb_Name.Clear();
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
                    // ОБНОВЛЕНИЕ существующего разработчика
                    var updateDeveloper = new
                    {
                        Id = _editingDeveloper.Id,
                        Nazvanie = tb_Name.Text.Trim()
                    };

                    var json = JsonConvert.SerializeObject(updateDeveloper);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"/api/Razrabotchiki/update/{_editingDeveloper.Id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Разработчик успешно обновлен", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Возвращаемся назад
                        NavigationService?.Navigate(new Developers());
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
            NavigationService?.Navigate(new Developers());
        }
    }
}