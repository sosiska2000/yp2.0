using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Users
{
    public partial class UserEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly User _currentUser;

        public UserEditPage(Frame mainFrame, User user = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentUser = user ?? new User();

            if (user != null)
            {
                TitleText.Text = "Редактирование пользователя";
                LoginBox.Text = user.Login;
                PasswordBox.Password = user.Password;

                if (user.Role == "admin") RoleBox.SelectedIndex = 0;
                else if (user.Role == "teacher") RoleBox.SelectedIndex = 1;
                else if (user.Role == "employee") RoleBox.SelectedIndex = 2;

                LastNameBox.Text = user.LastName;
                FirstNameBox.Text = user.FirstName;
                MiddleNameBox.Text = user.MiddleName;
                EmailBox.Text = user.Email;
                PhoneBox.Text = user.Phone;
                AddressBox.Text = user.Address;
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
            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                MessageBox.Show("Логин обязателен для заполнения");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password) && _currentUser.Id == 0)
            {
                MessageBox.Show("Пароль обязателен для заполнения");
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Фамилия обязательна для заполнения");
                return;
            }

            _currentUser.Login = LoginBox.Text;

            if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
                _currentUser.Password = PasswordBox.Password;

            if (RoleBox.SelectedItem is ComboBoxItem selectedRole)
                _currentUser.Role = selectedRole.Content.ToString();

            _currentUser.LastName = LastNameBox.Text;
            _currentUser.FirstName = FirstNameBox.Text;
            _currentUser.MiddleName = MiddleNameBox.Text;
            _currentUser.Email = EmailBox.Text;
            _currentUser.Phone = PhoneBox.Text;
            _currentUser.Address = AddressBox.Text;

            try
            {
                if (_currentUser.Id == 0)
                    await _apiService.AddUserAsync(_currentUser);
                else
                    await _apiService.UpdateUserAsync(_currentUser);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}