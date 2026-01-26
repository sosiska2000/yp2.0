using System.Windows;

namespace EquipmentManagement.Client
{
    public partial class LoginWindow : Window
    {
        // Храним текущего пользователя в переменной
        private static string _currentUser = "";

        public static string CurrentUser => _currentUser;

        public LoginWindow()
        {
            InitializeComponent();

            // Фокус на поле логина при открытии
            Loaded += (s, e) => UsernameTextBox.Focus();

            // Enter для входа
            KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    LoginButton_Click(null, null);
                }
            };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем предыдущую ошибку
            ErrorText.Visibility = Visibility.Collapsed;

            // Получаем данные
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Простая валидация
            if (string.IsNullOrEmpty(username))
            {
                ShowError("Введите логин");
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль");
                PasswordBox.Focus();
                return;
            }

            // ПРОСТАЯ проверка (потом заменим на вызов API)
            if (username == "admin" && password == "admin123")
            {
                _currentUser = username;
                this.DialogResult = true;
                this.Close();
            }
            else if (username == "teacher" && password == "teacher123")
            {
                _currentUser = username;
                this.DialogResult = true;
                this.Close();
            }
            else if (username == "user" && password == "user123")
            {
                _currentUser = username;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                ShowError("Неверный логин или пароль");
                PasswordBox.Clear();
                PasswordBox.Focus();
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}