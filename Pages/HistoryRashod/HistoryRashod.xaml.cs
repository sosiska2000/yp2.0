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

namespace EquipmentManagement.Client.Pages.HistoryRashod
{
    /// <summary>
    /// Логика взаимодействия для HistoryRashod.xaml
    /// </summary>
    public partial class HistoryRashod : Page
    {
        private int _rashodId;
        private Models.Users currentUser;

        public HistoryRashod(int rashodId)
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;

            _rashodId = rashodId;
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                // Загрузка истории для данного оборудования
                var historyList = GetHistoryForRashod(_rashodId);
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

        private List<Models.HistoryRashod> GetHistoryForRashod(int rashodId)
        {
            using (var context = new HistoryRashodContext())
            {
                return context.HistoryRashod.Where(h => h.IdRashod == rashodId).ToList();
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.RasxodMaterials.RasxodMaterials());
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
    }
}
