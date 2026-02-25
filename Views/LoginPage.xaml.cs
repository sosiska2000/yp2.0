using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views
{
    public partial class LoginPage : Page
    {
        private readonly ApiService _apiService;
        private readonly Frame _mainFrame;

        public LoginPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            LoginButton.Click += LoginButton_Click;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "Введите логин и пароль";
                return;
            }

            try
            {
                var users = await _apiService.GetUsersAsync();
                var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    if (user.Role == "admin")
                    {
                        _mainFrame.Navigate(new MainMenuPage(_mainFrame));
                    }
                    else
                    {
                        ErrorText.Text = "Доступ только для администраторов";
                    }
                }
                else
                {
                    ErrorText.Text = "Неверный логин или пароль";
                }
            }
            catch
            {
                ErrorText.Text = "Ошибка подключения к серверу";
            }
        }
    }
}