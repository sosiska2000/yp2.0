using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.EquipmentTypes
{
    public partial class EquipmentTypeEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly EquipmentType _currentType;

        public EquipmentTypeEditPage(Frame mainFrame, EquipmentType type = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentType = type ?? new EquipmentType();

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
                    await _apiService.AddEquipmentTypeAsync(_currentType);
                else
                    await _apiService.UpdateEquipmentTypeAsync(_currentType);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}