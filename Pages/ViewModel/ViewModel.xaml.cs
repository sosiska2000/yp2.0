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
using Newtonsoft.Json.Serialization;

namespace EquipmentManagement.Client.Pages.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для ViewModel.xaml
    /// </summary>
    public partial class ViewModel : Page
    {
        // Контекст для работы с моделями представления
        public ViewModelContext ViewModelContext = new ViewModelContext();
        private Models.Users currentUser;

        public ViewModel()
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;
            if (currentUser != null && currentUser.Role == "Администратор")
            {
                addBtn.Visibility = Visibility.Visible;
            }

            parent.Children.Clear(); // Очистка родительского контейнера перед добавлением новых элементов

            // Заполнение родительского контейнера элементами Item для каждой модели представления из контекста
            foreach (Models.ViewModel item in ViewModelContext.ViewModel)
            {
                parent.Children.Add(new Item(item, this)); // Добавление нового элемента Item для каждой модели представления
            }
        }

        // Обработчик события нажатия клавиши в поле поиска
        private void KeyDown_Search(object sender, KeyEventArgs e)
        {
            try
            {
                string searchText = search.Text.ToLower(); // Получение текста поиска в нижнем регистре

                // Поиск моделей представления, имя которых содержит текст поиска
                var result = ViewModelContext.ViewModel.Where(x =>
                    x.Name.ToLower().Contains(searchText)
                );

                parent.Children.Clear(); // Очистка родительского контейнера перед добавлением результатов поиска

                // Добавление найденных моделей представления в родительский контейнер
                foreach (var item in result)
                {
                    parent.Children.Add(new Item(item, this)); // Создание элемента Item для каждой найденной модели представления
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

        // Обработчик события нажатия кнопки "Назад"
        private void Back(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переход на страницу меню
                MainWindow.init.OpenPages(new Menu());
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

        // Обработчик события нажатия кнопки "Добавить"
        private void Add(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переход на страницу добавления новой модели представления
                MainWindow.init.OpenPages(new Pages.ViewModel.Add(this, null));
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

        private void SortUp(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сортировка пользователей по ФИО в порядке возрастания
                var sortUp = ViewModelContext.ViewModel.OrderBy(x => x.Name);
                parent.Children.Clear(); // Очистка родительского контейнера перед добавлением отсортированных пользователей

                // Добавление отсортированных пользователей в родительский контейнер
                foreach (var viewModel in sortUp)
                {
                    parent.Children.Add(new Item(viewModel, this)); // Создание элемента Item для каждого отсортированного пользователя
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

        private void SortDown(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сортировка пользователей по ФИО в порядке убывания
                var sortDown = ViewModelContext.ViewModel.OrderByDescending(x => x.Name);
                parent.Children.Clear(); // Очистка родительского контейнера перед добавлением отсортированных пользователей

                // Добавление отсортированных пользователей в родительский контейнер
                foreach (var viewModel in sortDown)
                {
                    parent.Children.Add(new Item(viewModel, this)); // Создание элемента Item для каждого отсортированного пользователя
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
