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
using System.Xml.Linq;

namespace EquipmentManagement.Client.Pages.ValueCharacteristics
{
    /// <summary>
    /// Логика взаимодействия для Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        ValueCharacteristics MainValueCharacteristics;
        Models.ValueCharacteristics valueCharacteristics;

        RasxodMaterialsContext rasxodMaterialsContext = new RasxodMaterialsContext();
        CharacteristicsContext characteristicsContext = new CharacteristicsContext();
        private Models.Users currentUser;

        public Item(Models.ValueCharacteristics valueCharacteristics, ValueCharacteristics MainValueCharacteristics)
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;
            if (currentUser != null && currentUser.Role == "Администратор")
            {
                buttons.Visibility = Visibility.Visible;
            }

            this.valueCharacteristics = valueCharacteristics;
            this.MainValueCharacteristics = MainValueCharacteristics;

            lb_Value.Content = valueCharacteristics.Znachenie;
            lb_RasxMat.Content = "Расходный материал: " + rasxodMaterialsContext.RasxodMaterials.Where(x => x.Id == valueCharacteristics.IdRasxod).FirstOrDefault()?.Name;
            lb_Charact.Content = "Характеристика: " + characteristicsContext.Characteristics.Where(x => x.Id == valueCharacteristics.IdCharacter).FirstOrDefault()?.Name;
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.ValueCharacteristics.Add(MainValueCharacteristics, valueCharacteristics));
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

        private void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("При удалении значения характеристики все связанные данные также будут удалены!", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Если пользователь подтвердил удаление
                if (result == MessageBoxResult.Yes)
                {
                    MainValueCharacteristics.ValueCharacteristicsContext.ValueCharacteristics.Remove(valueCharacteristics);
                    MainValueCharacteristics.ValueCharacteristicsContext.SaveChanges(); // Сохранение изменений в базе данных

                    // Удаление текущего элемента из родительского контейнера
                    (this.Parent as Panel).Children.Remove(this);
                }
                else
                {
                    // Сообщение о том, что действие отменено
                    MessageBox.Show("Действие отменено.");
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
    }
}
