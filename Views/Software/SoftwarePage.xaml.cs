using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Software
{
    public partial class SoftwarePage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<Models.Software> _allSoftware;
        private List<SoftwareDeveloper> _developers;

        public SoftwarePage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            DeveloperFilterBox.SelectionChanged += DeveloperFilterBox_SelectionChanged;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allSoftware = await _apiService.GetSoftwareListAsync();
                _developers = await _apiService.GetSoftwareDevelopersAsync();

                DeveloperFilterBox.ItemsSource = null;
                DeveloperFilterBox.Items.Clear();
                DeveloperFilterBox.Items.Add(new SoftwareDeveloper { Id = 0, Name = "Все разработчики" });
                foreach (var dev in _developers)
                    DeveloperFilterBox.Items.Add(dev);
                DeveloperFilterBox.SelectedIndex = 0;

                DataGrid.ItemsSource = _allSoftware;
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

        private void DeveloperFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            if (_allSoftware == null) return;

            var filtered = _allSoftware.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                filtered = filtered.Where(x =>
                    x.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (DeveloperFilterBox.SelectedItem is SoftwareDeveloper selectedDev && selectedDev.Id > 0)
            {
                filtered = filtered.Where(x => x.DeveloperId == selectedDev.Id);
            }

            DataGrid.ItemsSource = filtered.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new SoftwareEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Software;
            if (selected != null)
            {
                _mainFrame.Navigate(new SoftwareEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Software;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить программу '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteSoftwareAsync(selected.Id);
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