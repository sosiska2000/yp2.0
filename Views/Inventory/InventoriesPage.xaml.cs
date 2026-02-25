using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Inventory
{
    public partial class InventoriesPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<Models.Inventory> _allInventories;

        public InventoriesPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            ItemsButton.Click += ItemsButton_Click;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allInventories = await _apiService.GetInventoriesListAsync();
                DataGrid.ItemsSource = _allInventories;
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
            if (_allInventories == null) return;

            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                DataGrid.ItemsSource = _allInventories;
            }
            else
            {
                var filtered = _allInventories.Where(x =>
                    x.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase)).ToList();
                DataGrid.ItemsSource = filtered;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new InventoryEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Inventory;
            if (selected != null)
            {
                _mainFrame.Navigate(new InventoryEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Inventory;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить инвентаризацию '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteInventoryAsync(selected.Id);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void ItemsButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Inventory;
            if (selected != null)
            {
                _mainFrame.Navigate(new InventoryItemsPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите инвентаризацию");
            }
        }
    }
}