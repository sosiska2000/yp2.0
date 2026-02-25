using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.SoftwareDevelopers
{
    public partial class SoftwareDevelopersPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<SoftwareDeveloper> _allDevelopers;

        public SoftwareDevelopersPage(Frame mainFrame)
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
                _allDevelopers = await _apiService.GetSoftwareDevelopersAsync();
                DataGrid.ItemsSource = _allDevelopers;
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
                DataGrid.ItemsSource = _allDevelopers;
            }
            else
            {
                var filtered = _allDevelopers.Where(x =>
                    x.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase)).ToList();
                DataGrid.ItemsSource = filtered;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new SoftwareDeveloperEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as SoftwareDeveloper;
            if (selected != null)
            {
                _mainFrame.Navigate(new SoftwareDeveloperEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as SoftwareDeveloper;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить разработчика '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteSoftwareDeveloperAsync(selected.Id);
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