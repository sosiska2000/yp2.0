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
using Path = System.IO.Path;

namespace EquipmentManagement.Client.Pages.Auditories
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        // Основная страница аудиторий, с которой работает текущая страница
        public Auditories MainAuditories;

        // Модель аудитории, которую мы редактируем или добавляем
        public Models.Auditories auditories;

        // Контекст для работы с пользователями
        UsersContext usersContext = new();

        // Конструктор класса, который принимает основную страницу аудиторий и (опционально) аудиторию для редактирования
        public Add(Auditories MainAuditories, Models.Auditories auditories = null)
        {
            InitializeComponent();

            this.MainAuditories = MainAuditories; // Сохранение ссылки на основную страницу аудиторий
            this.auditories = auditories; // Сохранение аудитории для редактирования (если передана)

            // Если аудитория для редактирования не равна null, заполняем поля данными аудитории
            if (auditories != null)
            {
                text1.Content = "Изменение аудитории"; // Установка заголовка
                text2.Content = "Изменить"; // Изменение текста кнопки
                tb_Name.Text = auditories.Name; // Заполнение поля имени аудитории
                tb_shortName.Text = auditories.ShortName; // Заполнение поля сокращённого имени аудитории
                                                          // Установка выбранного ответственного пользователя по ID
                tb_User.SelectedItem = usersContext.Users.Where(x => x.Id == auditories.ResponUser).FirstOrDefault()?.FIO;
                // Установка выбранного временно-ответственного пользователя по ID
                tb_tempUser.SelectedItem = usersContext.Users.Where(x => x.Id == auditories.TimeResponUser).FirstOrDefault()?.FIO;
            }

            // Заполнение выпадающих списков пользователей
            foreach (var item in usersContext.Users)
            {
                tb_User.Items.Add(item.FIO); // Добавление каждого пользователя в список ответственных
                tb_tempUser.Items.Add(item.FIO); // Добавление каждого пользователя в список временно-ответственных
            }
        }

        // Обработчик события нажатия кнопки "Сохранить" (или "Изменить")
        private void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на заполненность полей
                if (string.IsNullOrEmpty(tb_Name.Text))
                {
                    MessageBox.Show("Введите наименование аудитории"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (string.IsNullOrEmpty(tb_shortName.Text))
                {
                    MessageBox.Show("Введите сокращённое наименование аудитории"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (tb_User.SelectedItem == null)
                {
                    MessageBox.Show("Выберите ответственного пользователя"); // Сообщение об ошибке, если ответственный не выбран
                    return; // Прерывание выполнения метода
                }
                if (tb_tempUser.SelectedItem == null)
                {
                    MessageBox.Show("Выберите временно-ответственного пользователя"); // Сообщение об ошибке, если временно-ответственный не выбран
                    return; // Прерывание выполнения метода
                }

                // Если аудитория не была передана (т.е. мы добавляем новую)
                if (auditories == null)
                {
                    auditories = new Models.Auditories // Создание новой модели аудитории
                    {
                        Name = tb_Name.Text, // Установка имени аудитории
                        ShortName = tb_shortName.Text, // Установка сокращённого имени аудитории
                        ResponUser = usersContext.Users.Where(x => x.FIO == tb_User.SelectedItem.ToString()).First().Id, // Получение ID ответственного пользователя
                        TimeResponUser = usersContext.Users.Where(x => x.FIO == tb_tempUser.SelectedItem.ToString()).First().Id // Получение ID временно-ответственного пользователя
                    };
                    MainAuditories.AuditoriesContext.Auditories.Add(auditories); // Добавление новой аудитории в контекст
                }
                else // Если аудитория уже существует (редактируем)
                {
                    // Обновление данных существующей аудитории
                    auditories.Name = tb_Name.Text; // Обновление имени аудитории
                    auditories.ShortName = tb_shortName.Text; // Обновление сокращённого имени аудитории
                    auditories.ResponUser = usersContext.Users.Where(x => x.FIO == tb_User.SelectedItem.ToString()).First().Id; // Обновление ID ответственного пользователя
                    auditories.TimeResponUser = usersContext.Users.Where(x => x.FIO == tb_tempUser.SelectedItem.ToString()).First().Id; // Обновление ID временно-ответственного пользователя
                }
                MainAuditories.AuditoriesContext.SaveChanges(); // Сохранение изменений в контексте базы данных
                MainWindow.init.OpenPages(new Pages.Auditories.Auditories()); // Переход на страницу аудиторий после сохранения
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
                MainWindow.init.OpenPages(new Pages.Auditories.Auditories()); // Переход на страницу аудиторий без сохранения изменений
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
