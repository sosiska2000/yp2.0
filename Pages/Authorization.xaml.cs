using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EquipmentManagement.Client.Context;
using Microsoft.EntityFrameworkCore;
using Path = System.IO.Path;

namespace EquipmentManagement.Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        private readonly UsersContext _usersContext;

        public Authorization()
        {
            InitializeComponent();
            _usersContext = new UsersContext();

            // Опционально: предварительная инициализация контекста
            InitializeDbContextAsync().ConfigureAwait(false);
        }

        private async Task InitializeDbContextAsync()
        {
            // Предварительная "прогрев" контекста
            await _usersContext.Users.FirstOrDefaultAsync();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Найти placeholder в визуальном дереве
                var border = VisualTreeHelper.GetChild(passwordBox, 0) as Border;
                if (border != null && VisualTreeHelper.GetChildrenCount(border) > 0)
                {
                    var grid = VisualTreeHelper.GetChild(border, 0) as Grid;
                    if (grid != null && VisualTreeHelper.GetChildrenCount(grid) > 1)
                    {
                        var placeholder = VisualTreeHelper.GetChild(grid, 1) as TextBlock;
                        if (placeholder != null)
                        {
                            // Скрыть placeholder если есть текст
                            placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                                ? Visibility.Visible
                                : Visibility.Collapsed;
                        }
                    }
                }
            }
        }
        private async void AuthorizationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string loginText = login.Text.Trim();
                string passwordText = password.Password;

                if (string.IsNullOrWhiteSpace(loginText))
                {
                    MessageBox.Show("Введите логин.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(passwordText))
                {
                    MessageBox.Show("Введите пароль.");
                    return;
                }

                // Асинхронный поиск пользователя с оптимизированным запросом
                var user = await FindUserAsync(loginText, passwordText);

                if (user != null)
                {
                    MainWindow.init.SetCurrentUser(user);
                    MainWindow.init.OpenPages(new Menu());
                }
                else
                {
                    MessageBox.Show("Некорректный ввод логина или пароля.");
                }
            }
            catch (Exception ex)
            {
                await LogError("Ошибка авторизации", ex);
                MessageBox.Show("Произошла ошибка при авторизации. Попробуйте позже.");
            }
        }

        private async Task<Models.Users?> FindUserAsync(string login, string password)
        {
            // Оптимизированный запрос - выбираем только необходимые поля
            return await _usersContext.Users
                .AsNoTracking() // Не отслеживать изменения
                .Where(u => u.Login == login && u.Password == password) // Проверяем и логин, и пароль
                .Select(u => new Models.Users
                {
                    Id = u.Id,
                    Login = u.Login,
                    Password = u.Password,
                    FIO = u.FIO,
                    Role = u.Role
                })
                .FirstOrDefaultAsync();
        }

        private async Task LogError(string message, Exception ex)
        {
            Debug.WriteLine($"{message}: {ex.Message}");

            try
            {
                await using (var errorsContext = new ErrorsContext())
                {
                    errorsContext.Errors.Add(new Models.Errors { Message = ex.Message });
                    await errorsContext.SaveChangesAsync();
                }

                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "log.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath) ?? string.Empty);
                await File.AppendAllTextAsync(logPath, $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n\n");
            }
            catch (Exception logEx)
            {
                Debug.WriteLine($"Ошибка при записи в лог-файл: {logEx.Message}");
            }
        }
    }
}
