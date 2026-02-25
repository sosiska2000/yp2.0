using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.SoftwareDevelopers
{
    public partial class SoftwareDeveloperEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly SoftwareDeveloper _currentDeveloper;

        public SoftwareDeveloperEditPage(Frame mainFrame, SoftwareDeveloper developer = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentDeveloper = developer ?? new SoftwareDeveloper();

            if (developer != null)
            {
                TitleText.Text = "Редактирование разработчика";
                NameBox.Text = developer.Name;
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

            _currentDeveloper.Name = NameBox.Text;

            try
            {
                if (_currentDeveloper.Id == 0)
                    await _apiService.AddSoftwareDeveloperAsync(_currentDeveloper);
                else
                    await _apiService.UpdateSoftwareDeveloperAsync(_currentDeveloper);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}