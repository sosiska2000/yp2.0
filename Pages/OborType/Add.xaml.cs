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

namespace EquipmentManagement.Client.Pages.OborType
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        // Поле для хранения ссылки на основной объект типа оборудования
        public OborType MainOborType;

        // Поле для хранения информации о конкретном типе оборудования
        public Models.OborType oborType;

        public Add(OborType MainOborType, Models.OborType oborType = null)
        {
            InitializeComponent();

            // Присваивание переданных параметров полям класса
            this.MainOborType = MainOborType;
            this.oborType = oborType;

            // Если объект типа оборудования не равен null, заполняем текстовое поле его именем
            if (oborType != null)
            {
                lb_title.Content = "Изменение типа оборудования";
                bt_click.Content = "Изменить";
                tb_Name.Text = oborType.Name;
            }
        }

        // Обработчик события нажатия кнопки "Редактировать"
        private void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка, введено ли название типа оборудования
                if (string.IsNullOrEmpty(tb_Name.Text))
                {
                    // Если нет, выводим сообщение об ошибке
                    MessageBox.Show("Введите название типа оборудования");
                    return; // Прерываем выполнение метода
                }

                // Если объект типа оборудования равен null, создаем новый объект
                if (oborType == null)
                {
                    oborType = new Models.OborType();
                    oborType.Name = tb_Name.Text; // Устанавливаем имя типа оборудования
                    MainOborType.OborTypeContext.OborType.Add(oborType); // Добавляем новый тип оборудования в контекст
                }
                else
                {
                    // Если объект типа оборудования существует, обновляем его имя
                    oborType.Name = tb_Name.Text;
                }

                // Сохраняем изменения в контексте типов оборудования
                MainOborType.OborTypeContext.SaveChanges();

                // Открываем страницу со списком типов оборудования
                MainWindow.init.OpenPages(new Pages.OborType.OborType());
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
                // Открываем страницу со списком типов оборудования без сохранения изменений
                MainWindow.init.OpenPages(new Pages.OborType.OborType());
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
