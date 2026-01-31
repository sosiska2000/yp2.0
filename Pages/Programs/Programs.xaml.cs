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

namespace EquipmentManagement.Client.Pages.Programs
{
    /// <summary>
    /// Логика взаимодействия для Programs.xaml
    /// </summary>
    public partial class Programs : Page
    {
        // Контекст для работы с программами
        public ProgramsContext ProgramsContext = new ProgramsContext();
        private Models.Users currentUser;

        public Programs()
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;
            if (currentUser != null && currentUser.Role == "Администратор")
            {
                addBtn.Visibility = Visibility.Visible;
            }

            parent.Children.Clear(); // Очистка родительского контейнера перед добавлением новых элементов

            // Заполнение родительского контейнера элементами Item для каждой программы из контекста
            foreach (Models.Programs item in ProgramsContext.Programs)
            {
                parent.Children.Add(new Item(item, this)); // Добавление нового элемента Item для каждой программы
            }
        }

        // Обработчик события нажатия клавиши в поле поиска
        private void KeyDown_Search(object sender, KeyEventArgs e)
        {
            try
            {
                string searchText = search.Text.ToLower(); // Получение текста поиска в нижнем регистре

                // Поиск программ, имя которых содержит текст поиска
                var result = ProgramsContext.Programs.Where(x =>
                    x.Name.ToLower().Contains(searchText)
                );

                parent.Children.Clear(); // Очистка родительского контейнера перед добавлением результатов поиска

                // Добавление найденных программ в родительский контейнер
                foreach (var item in result)
                {
                    parent.Children.Add(new Item(item, this)); // Создание элемента Item для каждой найденной программы
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

        // Обработчик события нажатия кнопки "Сортировать по возрастанию"
        private void SortUp(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сортировка программ по имени в порядке возрастания
                var sortUp = ProgramsContext.Programs.OrderBy(x => x.Name);
                parent.Children.Clear(); // Очистка родительского контейнера перед добавлением отсортированных программ

                // Добавление отсортированных программ в родительский контейнер
                foreach (var program in sortUp)
                {
                    parent.Children.Add(new Item(program, this)); // Создание элемента Item для каждой отсортированной программы
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

        // Обработчик события нажатия кнопки "Сортировать по убыванию"
        private void SortDown(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сортировка программ по имени в порядке убывания
                var sortDown = ProgramsContext.Programs.OrderByDescending(x => x.Name);
                parent.Children.Clear(); // Очистка родительского контейнера перед добавлением отсортированных программ

                // Добавление отсортированных программ в родительский контейнер
                foreach (var program in sortDown)
                {
                    parent.Children.Add(new Item(program, this)); // Создание элемента Item для каждой отсортированной программы
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

        // Обработчик события нажатия кнопки "Добавить"
        private void Add(object sender, RoutedEventArgs e)
        {
            try
            {
                // Переход на страницу добавления новой программы
                MainWindow.init.OpenPages(new Pages.Programs.Add(this, null));
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
