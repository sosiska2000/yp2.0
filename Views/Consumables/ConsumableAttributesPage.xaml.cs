using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Consumables
{
    public partial class ConsumableAttributesPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Consumable _consumable;
        private List<ConsumableAttribute> _attributes;

        public ConsumableAttributesPage(Frame mainFrame, Consumable consumable)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _consumable = consumable;

            ConsumableNameText.Text = $"Характеристики: {consumable.Name}";

            BackButton.Click += BackButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _attributes = await _apiService.GetConsumableAttributesAsync(_consumable.Id);
                DataGrid.ItemsSource = _attributes;
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь будет окно добавления характеристики
            MessageBox.Show("Функция добавления характеристики будет позже");
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as ConsumableAttribute;
            if (selected != null)
            {
                // Здесь будет окно редактирования характеристики
                MessageBox.Show($"Редактирование характеристики: {selected.AttributeName}");
            }
            else
            {
                MessageBox.Show("Выберите характеристику для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as ConsumableAttribute;
            if (selected == null)
            {
                MessageBox.Show("Выберите характеристику для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить характеристику '{selected.AttributeName}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteConsumableAttributeAsync(selected.Id);
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