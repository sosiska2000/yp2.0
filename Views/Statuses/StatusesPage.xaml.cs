using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Statuses
{
    public partial class StatusesPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<Status> _allStatuses;

        public StatusesPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allStatuses = await _apiService.GetStatusesAsync();
                DataGrid.ItemsSource = _allStatuses;
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
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                DataGrid.ItemsSource = _allStatuses;
            }
            else
            {
                var filtered = _allStatuses.Where(x =>
                    x.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase)).ToList();
                DataGrid.ItemsSource = filtered;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new StatusEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Status;
            if (selected != null)
            {
                _mainFrame.Navigate(new StatusEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Status;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить статус '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteStatusAsync(selected.Id);
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