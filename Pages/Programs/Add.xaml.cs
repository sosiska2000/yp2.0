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

namespace EquipmentManagement.Client.Pages.Programs
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        // Основная программа, с которой работает текущая страница
        public Programs MainPrograms;

        // Модель программы, которую мы редактируем или добавляем
        public Models.Programs programs;

        // Контексты для работы с базами данных разработчиков и оборудования
        DevelopersContext developersContext = new DevelopersContext();
        OborudovanieContext oborudovanieContext = new OborudovanieContext();

        // Конструктор класса, который принимает основную программу и (опционально) программу для редактирования
        public Add(Programs MainPrograms, Models.Programs programs = null)
        {
            InitializeComponent();

            this.MainPrograms = MainPrograms; // Сохранение ссылки на основную программу
            this.programs = programs; // Сохранение программы для редактирования (если передана)

            // Если программа для редактирования не равна null, заполняем поля данными программы
            if (programs != null)
            {
                lb_title.Content = "Изменение программ"; // Установка заголовка
                bt_click.Content = "Изменить"; // Изменение текста кнопки
                tb_Name.Text = programs.Name; // Заполнение поля имени программы
                tb_VersionPO.Text = programs.VersionPO; // Заполнение поля версии ПО

                // Установка выбранного разработчика и оборудования на основе текущей программы
                cm_DeveloperId.SelectedItem = developersContext.Developers.Where(x => x.Id == programs.DeveloperId).FirstOrDefault()?.Name;
                cm_OborId.SelectedItem = oborudovanieContext.Oborudovanie.Where(x => x.Id == programs.OborrId).FirstOrDefault()?.Name;
            }

            // Заполнение выпадающего списка разработчиков
            foreach (var item in developersContext.Developers)
            {
                cm_DeveloperId.Items.Add(item.Name);
            }

            // Заполнение выпадающего списка оборудования
            foreach (var item in oborudovanieContext.Oborudovanie)
            {
                cm_OborId.Items.Add(item.Name);
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
                    MessageBox.Show("Введите название программы");
                    return; // Прерывание выполнения метода, если поле пустое
                }
                if (string.IsNullOrEmpty(tb_VersionPO.Text))
                {
                    MessageBox.Show("Введите версию ПО");
                    return; // Прерывание выполнения метода, если поле пустое
                }
                if (cm_DeveloperId.SelectedItem == null)
                {
                    MessageBox.Show("Выберите разработчика");
                    return; // Прерывание выполнения метода, если разработчик не выбран
                }

                // Если программа не была передана (т.е. мы добавляем новую)
                if (programs == null)
                {
                    programs = new Models.Programs();
                    programs.Name = tb_Name.Text;
                    programs.VersionPO = tb_VersionPO.Text;
                    programs.DeveloperId = developersContext.Developers.Where(x => x.Name == cm_DeveloperId.SelectedItem.ToString()).First().Id;
                    programs.OborrId = oborudovanieContext.Oborudovanie.Where(x => x.Name == cm_OborId.SelectedItem.ToString()).First().Id;
                    MainPrograms.ProgramsContext.Programs.Add(programs);
                }
                else // Если программа уже существует (редактируем)
                {
                    // Обновление данных существующей программы
                    programs.Name = tb_Name.Text;
                    programs.VersionPO = tb_VersionPO.Text;
                    programs.DeveloperId = developersContext.Developers.Where(x => x.Name == cm_DeveloperId.SelectedItem.ToString()).First().Id;
                    programs.OborrId = oborudovanieContext.Oborudovanie.Where(x => x.Name == cm_OborId.SelectedItem.ToString()).First().Id;
                }

                // Сохранение изменений в базе данных
                MainPrograms.ProgramsContext.SaveChanges();
                // Переход на страницу со списком программ
                MainWindow.init.OpenPages(new Pages.Programs.Programs());
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
                // Переход на страницу со списком программ без сохранения изменений
                MainWindow.init.OpenPages(new Pages.Programs.Programs());
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
