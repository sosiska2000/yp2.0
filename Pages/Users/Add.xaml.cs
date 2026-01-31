using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Path = System.IO.Path;

namespace EquipmentManagement.Client.Pages.Users
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        // Основная страница пользователей, с которой работает текущая страница
        public Users MainUsers;

        // Модель пользователя, которую мы редактируем или добавляем
        public Models.Users users;

        // Конструктор класса, который принимает основную страницу пользователей и (опционально) пользователя для редактирования
        public Add(Users MainUsers, Models.Users users = null)
        {
            InitializeComponent(); // Инициализация компонентов страницы
            this.MainUsers = MainUsers; // Сохранение ссылки на основную страницу пользователей
            this.users = users; // Сохранение пользователя для редактирования (если передан)

            // Если пользователь для редактирования не равен null, заполняем поля данными пользователя
            if (users != null)
            {
                text1.Content = "Изменение пользователя"; // Установка заголовка
                text2.Content = "Изменить"; // Изменение текста кнопки
                tb_FIO.Text = users.FIO; // Заполнение поля ФИО
                tb_Login.Text = users.Login; // Заполнение поля логина
                tb_Password.Text = users.Password; // Заполнение поля пароля
                tb_Email.Text = users.Email; // Заполнение поля электронной почты
                tb_Phone.Text = users.Number; // Заполнение поля номера телефона
                tb_Role.SelectedItem = users.Role; // Установка выбранной роли
                tb_Address.Text = users.Address; // Заполнение поля адреса
            }

            // Заполнение выпадающего списка ролей
            tb_Role.Items.Add("Администратор");
            tb_Role.Items.Add("Сотрудник");
            tb_Role.Items.Add("Преподаватель");
        }

        // Обработчик события нажатия кнопки "Сохранить" (или "Изменить")
        private void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на заполненность полей
                if (!Regex.IsMatch(tb_FIO.Text, "([А-ЯЁ][а-яё]+[\\-\\s]?){3,}") || tb_FIO.Text == "")
                {
                    MessageBox.Show("Введите ФИО пользователя"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (!Regex.IsMatch(tb_Login.Text, "[A-Za-z]") || tb_Login.Text == "")
                {
                    MessageBox.Show("Введите логин"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (!Regex.IsMatch(tb_Password.Text, "[a-zA-Z]") || tb_Password.Text == "")
                {
                    MessageBox.Show("Введите пароль"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (!Regex.IsMatch(tb_Phone.Text, "^((8|\\+7)[\\- ]?)?(\\(?\\d{3}\\)?[\\- ]?)?[\\d\\- ]{7,10}$") || tb_Phone.Text == "")
                {
                    MessageBox.Show("Введите номер телефона"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (!Regex.IsMatch(tb_Email.Text, @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$") || tb_Email.Text == "")
                {
                    MessageBox.Show("Введите эл. почту в формате test@test.test"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (string.IsNullOrEmpty(tb_Address.Text))
                {
                    MessageBox.Show("Введите адрес"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (tb_Role.SelectedItem == null)
                {
                    MessageBox.Show("Выберите роль"); // Сообщение об ошибке, если роль не выбрана
                    return; // Прерывание выполнения метода
                }

                // Если пользователь не был передан (т.е. мы добавляем нового)
                if (users == null)
                {
                    users = new Models.Users // Создание новой модели пользователя
                    {
                        FIO = tb_FIO.Text, // Установка ФИО
                        Login = tb_Login.Text, // Установка логина
                        Password = tb_Password.Text, // Установка пароля
                        Number = tb_Phone.Text, // Установка номера телефона
                        Email = tb_Email.Text, // Установка электронной почты
                        Address = tb_Address.Text, // Установка адреса
                        Role = tb_Role.Text // Установка роли
                    };
                    MainUsers.UsersContext.Users.Add(users); // Добавление нового пользователя в контекст
                }
                else // Если пользователь уже существует (редактируем)
                {
                    // Обновление данных существующего пользователя
                    users.FIO = tb_FIO.Text; // Обновление ФИО
                    users.Login = tb_Login.Text; // Обновление логина
                    users.Password = tb_Password.Text; // Обновление пароля
                    users.Email = tb_Email.Text; // Обновление электронной почты
                    users.Address = tb_Address.Text; // Обновление адреса
                    users.Role = tb_Role.Text; // Обновление роли
                    users.Number = tb_Phone.Text; // Обновление номера телефона
                }

                MainUsers.UsersContext.SaveChanges(); // Сохранение изменений в контексте
                MainWindow.init.OpenPages(new Pages.Users.Users()); // Переход на страницу пользователей
            }
            catch (Exception ex)
            {
                LogError("Ошибка", ex).ConfigureAwait(false);
            }
        }

        // Обработчик события нажатия кнопки "Отмена"
        private void Click_Cancel_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Users.Users()); // Переход на страницу пользователей без сохранения изменений
            }
            catch (Exception ex)
            {
                LogError("Ошибка", ex).ConfigureAwait(false);
            }
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
