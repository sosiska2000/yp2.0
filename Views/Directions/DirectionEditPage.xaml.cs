using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Directions
{
    public partial class DirectionEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Direction _currentDirection;

        public DirectionEditPage(Frame mainFrame, Direction direction = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentDirection = direction ?? new Direction();

            if (direction != null)
            {
                TitleText.Text = "Редактирование направления";
                NameBox.Text = direction.Name;
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

            _currentDirection.Name = NameBox.Text;

            try
            {
                if (_currentDirection.Id == 0)
                    await _apiService.AddDirectionAsync(_currentDirection);
                else
                    await _apiService.UpdateDirectionAsync(_currentDirection);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}