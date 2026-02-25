using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Software
{
    public partial class SoftwareEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Models.Software _currentSoftware;

        public SoftwareEditPage(Frame mainFrame, Models.Software software = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentSoftware = software ?? new Models.Software();

            if (software != null)
            {
                TitleText.Text = "Редактирование программы";
                NameBox.Text = software.Name;
                VersionBox.Text = software.Version;
                DeveloperIdBox.Text = software.DeveloperId?.ToString();
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

            _currentSoftware.Name = NameBox.Text;
            _currentSoftware.Version = VersionBox.Text;

            if (int.TryParse(DeveloperIdBox.Text, out int devId))
                _currentSoftware.DeveloperId = devId;
            else
                _currentSoftware.DeveloperId = null;

            try
            {
                if (_currentSoftware.Id == 0)
                    await _apiService.AddSoftwareAsync(_currentSoftware);
                else
                    await _apiService.UpdateSoftwareAsync(_currentSoftware);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}