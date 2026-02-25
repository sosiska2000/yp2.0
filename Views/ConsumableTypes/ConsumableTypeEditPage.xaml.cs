using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.ConsumableTypes
{
    public partial class ConsumableTypeEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly ConsumableType _currentType;

        public ConsumableTypeEditPage(Frame mainFrame, ConsumableType type = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentType = type ?? new ConsumableType();

            if (type != null)
            {
                TitleText.Text = "Редактирование типа";
                NameBox.Text = type.Name;
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

            _currentType.Name = NameBox.Text;

            try
            {
                if (_currentType.Id == 0)
                    await _apiService.AddConsumableTypeAsync(_currentType);
                else
                    await _apiService.UpdateConsumableTypeAsync(_currentType);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}