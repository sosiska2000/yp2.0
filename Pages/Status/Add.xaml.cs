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

namespace EquipmentManagement.Client.Pages.Status
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        // Основная страница статусов, с которой работает текущая страница
        public Status MainStatus;

        // Модель статуса, которую мы редактируем или добавляем
        public Models.Status status;

        // Конструктор класса, который принимает основную страницу статусов и (опционально) статус для редактирования
        public Add(Status MainStatus, Models.Status status)
        {
            InitializeComponent();
            this.MainStatus = MainStatus; // Сохранение ссылки на основную страницу статусов
            this.status = status; // Сохранение статуса для редактирования (если передан)

            // Если статус для редактирования не равен null, заполняем поля данными статуса
            if (status != null)
            {
                text1.Content = "Изменение статуса"; // Установка заголовка
                text2.Content = "Изменить"; // Изменение текста кнопки
                tb_Name.Text = status.Name; // Заполнение поля имени статуса
            }
        }

        // Обработчик события нажатия кнопки "Сохранить" (или "Изменить")
        private void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на заполненность поля имени статуса
                if (string.IsNullOrEmpty(tb_Name.Text))
                {
                    MessageBox.Show("Введите наименование статуса"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }

                // Если статус не был передан (т.е. мы добавляем новый)
                if (status == null)
                {
                    status = new Models.Status // Создание новой модели статуса
                    {
                        Name = tb_Name.Text // Установка имени статуса
                    };
                    MainStatus.StatusContext.Status.Add(status); // Добавление новой модели в контекст
                }
                else // Если статус уже существует (редактируем)
                {
                    status.Name = tb_Name.Text; // Обновление имени статуса
                }

                // Сохранение изменений в базе данных
                MainStatus.StatusContext.SaveChanges();
                // Переход на страницу со списком статусов
                MainWindow.init.OpenPages(new Pages.Status.Status());
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
                // Переход на страницу со списком статусов без сохранения изменений
                MainWindow.init.OpenPages(new Pages.Status.Status());
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
