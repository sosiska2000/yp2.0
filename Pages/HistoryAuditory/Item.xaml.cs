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
    /// Логика взаимодействия для Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        HistoryAuditory MainHistoryAuditory;
        Models.HistoryAuditory HistoryAuditory;
        AuditoriesContext auditoriesContext = new AuditoriesContext();
        private Models.Users currentUser;
        public Item(Models.HistoryAuditory HistoryAuditory, HistoryAuditory MainHistoryAuditory)
        {
            InitializeComponent();
            try
            {
                currentUser = MainWindow.init.CurrentUser;
                this.HistoryAuditory = HistoryAuditory;
                this.MainHistoryAuditory = MainHistoryAuditory;
                lb_Classroom.Content = auditoriesContext.Auditories.Where(x => x.Id == HistoryAuditory.IdClassroom).First().Name;
                lb_Date.Content = HistoryAuditory.Date.ToString();
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
