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
using Newtonsoft.Json.Serialization;
using Path = System.IO.Path;

namespace EquipmentManagement.Client.Pages.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        // Основная модель представления, с которой работает текущая страница
        public ViewModel MainViewModel;

        // Модель представления, которую мы редактируем или добавляем
        public Models.ViewModel viewModel;

        // Контекст для работы с типами оборудования
        OborTypeContext oborTypeContext = new OborTypeContext();

        // Конструктор класса, который принимает основную модель представления и (опционально) модель для редактирования
        public Add(ViewModel MainViewModel, Models.ViewModel viewModel = null)
        {
            InitializeComponent();

            this.MainViewModel = MainViewModel; // Сохранение ссылки на основную модель представления
            this.viewModel = viewModel; // Сохранение модели представления для редактирования (если передана)

            // Если модель представления для редактирования не равна null, заполняем поля данными модели
            if (viewModel != null)
            {
                lb_title.Content = "Изменение вида модели"; // Установка заголовка
                bt_click.Content = "Изменить"; // Изменение текста кнопки
                tb_Name.Text = viewModel.Name; // Заполнение поля имени типа оборудования
                cm_OborType.SelectedItem = oborTypeContext.OborType.Where(x => x.Id == viewModel.OborType).FirstOrDefault()?.Name; // Установка выбранного типа оборудования
            }

            // Заполнение выпадающего списка типов оборудования
            foreach (var item in oborTypeContext.OborType)
            {
                cm_OborType.Items.Add(item.Name); // Добавление каждого типа оборудования в выпадающий список
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
                    MessageBox.Show("Введите название типа оборудования"); // Сообщение об ошибке, если поле пустое
                    return; // Прерывание выполнения метода
                }
                if (cm_OborType.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип оборудования"); // Сообщение об ошибке, если тип не выбран
                    return; // Прерывание выполнения метода
                }

                // Если модель представления не была передана (т.е. мы добавляем новую)
                if (viewModel == null)
                {
                    viewModel = new Models.ViewModel(); // Создание новой модели представления
                    viewModel.Name = tb_Name.Text; // Установка имени типа оборудования
                    viewModel.OborType = oborTypeContext.OborType.Where(x => x.Name == cm_OborType.SelectedItem.ToString()).First().Id; // Получение ID типа оборудования по имени
                    MainViewModel.ViewModelContext.ViewModel.Add(viewModel); // Добавление новой модели в контекст
                }
                else // Если модель уже существует (редактируем)
                {
                    viewModel.Name = tb_Name.Text; // Обновление имени типа оборудования
                    viewModel.OborType = oborTypeContext.OborType.Where(x => x.Name == cm_OborType.SelectedItem.ToString()).First().Id; // Обновление ID типа оборудования
                }

                // Сохранение изменений в базе данных
                MainViewModel.ViewModelContext.SaveChanges();
                // Переход на страницу со списком моделей представления
                MainWindow.init.OpenPages(new Pages.ViewModel.ViewModel());
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
                // Переход на страницу со списком моделей представления без сохранения изменений
                MainWindow.init.OpenPages(new Pages.ViewModel.ViewModel());
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
