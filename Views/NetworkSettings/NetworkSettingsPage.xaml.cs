using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.NetworkSettings
{
    public partial class NetworkSettingsPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<NetworkSetting> _allSettings;
        private List<Models.Equipment> _equipmentList; // Явно указываем Models.Equipment

        public NetworkSettingsPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            CheckNetworkButton.Click += CheckNetworkButton_Click;
            EquipmentFilterBox.SelectionChanged += EquipmentFilterBox_SelectionChanged;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allSettings = await _apiService.GetNetworkSettingsListAsync();
                _equipmentList = await _apiService.GetEquipmentListAsync();

                EquipmentFilterBox.ItemsSource = null;
                EquipmentFilterBox.Items.Clear();
                EquipmentFilterBox.Items.Add(new Models.Equipment { Id = 0, Name = "Все оборудование" });
                foreach (var eq in _equipmentList)
                    EquipmentFilterBox.Items.Add(eq);
                EquipmentFilterBox.SelectedIndex = 0;

                DataGrid.ItemsSource = _allSettings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.GoBack();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterData();
        }

        private void EquipmentFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            if (_allSettings == null) return;

            var filtered = _allSettings.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                filtered = filtered.Where(x =>
                    x.IpAddress.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (EquipmentFilterBox.SelectedItem is Models.Equipment selectedEq && selectedEq.Id > 0)
            {
                filtered = filtered.Where(x => x.EquipmentId == selectedEq.Id);
            }

            DataGrid.ItemsSource = filtered.ToList();
        }

        private async void CheckNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = await _apiService.CheckNetworkAsync();
                string message = "Результаты проверки сети:\n\n";
                foreach (var item in result)
                {
                    message += $"{item.Key}: {(item.Value ? "Доступен" : "Недоступен")}\n";
                }
                MessageBox.Show(message, "Проверка сети");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки сети: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new NetworkSettingEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as NetworkSetting;
            if (selected != null)
            {
                _mainFrame.Navigate(new NetworkSettingEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as NetworkSetting;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить сетевые настройки для IP {selected.IpAddress}?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteNetworkSettingAsync(selected.Id);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }
    }
}