using System;
using System.Collections.Generic;
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

namespace EquipmentManagement.Client.Pages.HistoryAuditory
{
    /// <summary>
    /// Логика взаимодействия для HistoryAuditory.xaml
    /// </summary>
    public partial class HistoryAuditory : Page
    {
        private int _oborudovanieId;
        private Models.Users currentUser;

        public HistoryAuditory(int oborudovanieId)
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;

            _oborudovanieId = oborudovanieId;
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                // Загрузка истории для данного оборудования
                var historyList = GetHistoryForAuditories(_oborudovanieId);
                foreach (var history in historyList)
                {
                    // Создание пользовательского элемента для каждой записи истории
                    var item = new Item(history, this);
                    // Добавление элемента на страницу
                    parent.Children.Add(item);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    using (var errorsContext = new ErrorsContext())
                    {
                        var error = new Models.Errors
                        {
                            Message = ex.Message
                        };
                        errorsContext.Errors.Add(error);
                        errorsContext.SaveChanges(); // Сохраняем ошибку в базе данных
                    }

                    // Логирование ошибки в файл log.txt
                    string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "log.txt");
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath)); // Создаем папку bin, если ее нет
                    System.IO.File.AppendAllText(logPath, $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n\n");
                }
                catch (Exception logEx)
                {
                    MessageBox.Show("Ошибка при записи в лог-файл: " + logEx.Message);
                }

                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private List<Models.HistoryAuditory> GetHistoryForAuditories(int oborudovanieId)
        {
            using (var context = new HistoryAuditoryContex())
            {
                return context.HistoryAuditory.Where(h => h.IdObor == oborudovanieId).ToList();
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Oborudovanie.Oborudovanie());
            }
            catch (Exception ex)
            {
                try
                {
                    using (var errorsContext = new ErrorsContext())
                    {
                        var error = new Models.Errors
                        {
                            Message = ex.Message
                        };
                        errorsContext.Errors.Add(error);
                        errorsContext.SaveChanges(); // Сохраняем ошибку в базе данных
                    }

                    // Логирование ошибки в файл log.txt
                    string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "log.txt");
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath)); // Создаем папку bin, если ее нет
                    System.IO.File.AppendAllText(logPath, $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n\n");
                }
                catch (Exception logEx)
                {
                    MessageBox.Show("Ошибка при записи в лог-файл: " + logEx.Message);
                }

                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {

        }

        private void ClearFilters(object sender, RoutedEventArgs e)
        {

        }

        private void ExportToExcel(object sender, RoutedEventArgs e)
        {

        }

        private void ExportToPDF(object sender, RoutedEventArgs e)
        {

        }

        private void KeyDown_Search(object sender, KeyEventArgs e)
        {

        }

        private void ShowFilters(object sender, RoutedEventArgs e)
        {

        }
    }
}
