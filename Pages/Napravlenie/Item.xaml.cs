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

namespace EquipmentManagement.Client.Pages.Napravlenie
{
    /// <summary>
    /// Логика взаимодействия для Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        // Основная страница направлений, с которой работает текущий элемент
        Napravlenie MainNapravlenie;

        // Модель направления, которую представляет данный элемент
        Models.Napravlenie Napravlenie;
        private Models.Users currentUser;

        // Конструктор класса, который принимает модель направления и основную страницу направлений
        public Item(Models.Napravlenie Napravlenie, Napravlenie MainNapravlenie)
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;
            if (currentUser != null && currentUser.Role == "Администратор")
            {
                buttons.Visibility = Visibility.Visible;
            }

            this.MainNapravlenie = MainNapravlenie; // Сохранение ссылки на основную страницу направлений
            this.Napravlenie = Napravlenie; // Сохранение ссылки на модель направления

            // Заполнение элемента управления данными направления
            lb_Name.Content = Napravlenie.Name; // Установка имени направления
        }

        // Обработчик события нажатия кнопки "Редактировать"
        private void Click_redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переход на страницу редактирования направления, передавая основную страницу и текущее направление
                MainWindow.init.OpenPages(new Pages.Napravlenie.Add(MainNapravlenie, Napravlenie));
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
                    // Удаление направления из контекста
                    MainNapravlenie.NapravlenieContext.Napravlenie.Remove(Napravlenie);
                    MainNapravlenie.NapravlenieContext.SaveChanges(); // Сохранение изменений в базе данных

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
