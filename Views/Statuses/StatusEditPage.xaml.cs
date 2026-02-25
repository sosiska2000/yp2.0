using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Statuses
{
    public partial class StatusEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Status _currentStatus;

        public StatusEditPage(Frame mainFrame, Status status = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentStatus = status ?? new Status();

            if (status != null)
            {
                TitleText.Text = "Редактирование статуса";
                NameBox.Text = status.Name;
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

            _currentStatus.Name = NameBox.Text;

            try
            {
                if (_currentStatus.Id == 0)
                    await _apiService.AddStatusAsync(_currentStatus);
                else
                    await _apiService.UpdateStatusAsync(_currentStatus);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}