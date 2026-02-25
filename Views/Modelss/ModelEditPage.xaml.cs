using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Modelss
{
    public partial class ModelEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Model _currentModel;

        public ModelEditPage(Frame mainFrame, Model model = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentModel = model ?? new Model();

            if (model != null)
            {
                TitleText.Text = "Редактирование модели";
                NameBox.Text = model.Name;
                EquipmentTypeIdBox.Text = model.EquipmentTypeId.ToString();
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
                MessageBox.Show("Название модели обязательно для заполнения");
                return;
            }

            if (!int.TryParse(EquipmentTypeIdBox.Text, out int typeId) || typeId <= 0)
            {
                MessageBox.Show("Корректный ID типа оборудования обязателен");
                return;
            }

            _currentModel.Name = NameBox.Text;
            _currentModel.EquipmentTypeId = typeId;

            try
            {
                if (_currentModel.Id == 0)
                    await _apiService.AddModelAsync(_currentModel);
                else
                    await _apiService.UpdateModelAsync(_currentModel);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}