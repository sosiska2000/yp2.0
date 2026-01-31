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

namespace EquipmentManagement.Client.Pages.Programs
{
    /// <summary>
    /// Логика взаимодействия для Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        // Основная программа, с которой работает текущий элемент
        Programs MainPrograms;

        // Модель программы, которую представляет данный элемент
        Models.Programs Programs;

        // Контексты для работы с базами данных разработчиков и оборудования
        DevelopersContext developersContext = new DevelopersContext();
        OborudovanieContext oborudovanieContext = new OborudovanieContext();
        private Models.Users currentUser;

        // Конструктор класса, который принимает модель программы и основную программу
        public Item(Models.Programs Programs, Programs MainPrograms)
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;
            if (currentUser != null && currentUser.Role == "Администратор")
            {
                buttons.Visibility = Visibility.Visible;
            }

            this.Programs = Programs; // Сохранение ссылки на модель программы
            this.MainPrograms = MainPrograms; // Сохранение ссылки на основную программу

            // Заполнение элементов управления данными программы
            lb_Name.Content = Programs.Name; // Установка имени программы
            lb_VersionPO.Content = Programs.VersionPO; // Установка версии ПО

            // Получение имени разработчика по ID программы и установка в элемент управления
            lb_Developer.Content = developersContext.Developers.Where(x => x.Id == Programs.DeveloperId).FirstOrDefault()?.Name;

            // Получение имени оборудования по ID программы и установка в элемент управления
            lb_Obor.Content = oborudovanieContext.Oborudovanie.Where(x => x.Id == Programs.OborrId).FirstOrDefault()?.Name;
        }

        // Обработчик события нажатия кнопки "Редактировать"
        private void Click_redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переход на страницу редактирования программы, передавая основную программу и текущую программу
                MainWindow.init.OpenPages(new Pages.Programs.Add(MainPrograms, Programs));
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

        // Обработчик события нажатия кнопки "Удалить"
        private void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                // Запрос подтверждения на удаление
                MessageBoxResult result = MessageBox.Show("При удалении все связанные данные также будут удалены!", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Если пользователь подтвердил удаление
                if (result == MessageBoxResult.Yes)
                {
                    // Удаление программы из контекста
                    MainPrograms.ProgramsContext.Programs.Remove(Programs);
                    MainPrograms.ProgramsContext.SaveChanges(); // Сохранение изменений в базе данных

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
