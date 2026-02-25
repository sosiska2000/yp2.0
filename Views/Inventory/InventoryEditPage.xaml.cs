using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Inventory
{
    public partial class InventoryEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Models.Inventory _currentInventory;

        public InventoryEditPage(Frame mainFrame, Models.Inventory inventory = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentInventory = inventory ?? new Models.Inventory();

            if (inventory != null)
            {
                TitleText.Text = "Редактирование инвентаризации";
                NameBox.Text = inventory.Name;
                StartDateBox.Text = inventory.StartDate.ToString("dd.MM.yyyy");
                EndDateBox.Text = inventory.EndDate?.ToString("dd.MM.yyyy");
                CreatedByUserIdBox.Text = inventory.CreatedByUserId?.ToString();
            }

            CancelButton.Click += CancelButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.GoBack();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Название обязательно для заполнения");
                return;
            }

            if (!DateTime.TryParse(StartDateBox.Text, out DateTime startDate))
            {
                MessageBox.Show("Некорректная дата начала");
                return;
            }

            _currentInventory.Name = NameBox.Text;
            _currentInventory.StartDate = startDate;

            if (DateTime.TryParse(EndDateBox.Text, out DateTime endDate))
                _currentInventory.EndDate = endDate;
            else
                _currentInventory.EndDate = null;

            if (int.TryParse(CreatedByUserIdBox.Text, out int userId))
                _currentInventory.CreatedByUserId = userId;
            else
                _currentInventory.CreatedByUserId = null;

            try
            {
                if (_currentInventory.Id == 0)
                {
                    // Для создания нужно передать список оборудования
                    MessageBox.Show("Для создания инвентаризации нужно выбрать оборудование");
                    return;
                }
                else
                {
                    await _apiService.UpdateInventoryAsync(_currentInventory);
                }

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}