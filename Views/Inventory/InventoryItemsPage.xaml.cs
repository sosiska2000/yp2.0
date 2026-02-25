using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Inventory
{
    public partial class InventoryItemsPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Models.Inventory _inventory; // Явно указываем Models.Inventory
        private List<InventoryItem> _items;

        public InventoryItemsPage(Frame mainFrame, Models.Inventory inventory) // Явно указываем Models.Inventory
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _inventory = inventory;

            InventoryNameText.Text = $"Состав инвентаризации: {inventory.Name}";

            BackButton.Click += BackButton_Click;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _items = await _apiService.GetInventoryItemsAsync(_inventory.Id);
                DataGrid.ItemsSource = _items;
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
    }
}