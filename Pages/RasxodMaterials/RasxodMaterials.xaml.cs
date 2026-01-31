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
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace EquipmentManagement.Client.Pages.RasxodMaterials
{
    /// <summary>
    /// Логика взаимодействия для RasxodMaterials.xaml
    /// </summary>
    public partial class RasxodMaterials : Page
    {
        public RasxodMaterialsContext rasxodMaterialsContext = new RasxodMaterialsContext();
        private Models.Users currentUser;
        public UsersContext usContext = new UsersContext();

        private Item _selectedItem;

        public RasxodMaterials()
        {
            InitializeComponent();

            currentUser = MainWindow.init.CurrentUser;
            if (currentUser != null && currentUser.Role == "Администратор")
            {
                addBtn.Visibility = Visibility.Visible;
                exportDoc.Visibility = Visibility.Visible;
            }

            LoadMaterials();
        }

        private void LoadMaterials()
        {
            parent.Children.Clear();
            foreach (Models.RasxodMaterials item in rasxodMaterialsContext.RasxodMaterials)
            {
                var itemControl = new Item(item, this);
                itemControl.SelectionChanged += ItemControl_SelectionChanged;
                parent.Children.Add(itemControl);
            }
        }

        private void ItemControl_SelectionChanged(object sender, EventArgs e)
        {
            var item = (Item)sender;

            if (item.IsSelected)
            {
                if (_selectedItem != null && _selectedItem != item)
                {
                    _selectedItem.IsSelected = false;
                }
                _selectedItem = item;
            }
            else if (_selectedItem == item)
            {
                _selectedItem = null;
            }
        }

        private Models.RasxodMaterials GetSelectedMaterial()
        {
            return _selectedItem?.RasxodMaterials;
        }

        private void KeyDown_Search(object sender, KeyEventArgs e)
        {
            try
            {
                string searchText = search.Text.ToLower();
                var result = rasxodMaterialsContext.RasxodMaterials.Where(x =>
                    x.Name.ToLower().Contains(searchText)
                );
                parent.Children.Clear();
                foreach (var item in result)
                {
                    parent.Children.Add(new Item(item, this));
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

        private void Back(object sender, RoutedEventArgs e)
        {
            try
            {
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

        private void SortUp(object sender, RoutedEventArgs e)
        {
            try
            {
                var sortUp = rasxodMaterialsContext.RasxodMaterials.OrderBy(x => x.Name);
                parent.Children.Clear();

                foreach (var auditories in sortUp)
                {
                    parent.Children.Add(new Item(auditories, this));
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
                var sortDown = rasxodMaterialsContext.RasxodMaterials.OrderByDescending(x => x.Name);
                parent.Children.Clear();

                foreach (var auditories in sortDown)
                {
                    parent.Children.Add(new Item(auditories, this));
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

        private void Add(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.RasxodMaterials.Add(this, null));
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

        private void ExportRasxodMaterials(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedMaterial = GetSelectedMaterial();
                if (selectedMaterial == null)
                {
                    MessageBox.Show("Пожалуйста, выберите расходный материал для генерации отчета.");
                    return;
                }

                string currentDate = DateTime.Now.ToString("dd.MM.yyyy");

                using (var rasxMatContext = new RasxodMaterialsContext())
                {
                    var rasMat = rasxMatContext.RasxodMaterials
                        .FirstOrDefault(x => x.Id == selectedMaterial.Id);

                    if (rasMat == null)
                    {
                        MessageBox.Show("Расходные материалы не найдены в базе данных.");
                        return;
                    }

                    var currentUser = usContext.Users.FirstOrDefault(x => x.Role == "Сотрудник");

                    using (DocX document = DocX.Create("Akt_Priema_Peredachi_Rasxodnyx_Materialov.docx"))
                    {
                        document.InsertParagraph("АКТ\nприема-передачи расходных материалов\n\n")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.center;

                        var locationAndDate = document.InsertParagraph($"г. Пермь")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.left;

                        var date = document.InsertParagraph($"{currentDate}\n")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.right;

                        if (currentUser != null)
                        {
                            var fioParts = currentUser.FIO.Split(' ');
                            string lastName = fioParts[0];
                            string initials = $"{fioParts[1][0]}.{fioParts[2][0]}.";
                            var mainText = document.InsertParagraph($"КГАПОУ Пермский Авиационный техникум им. А.Д. Швецова в целях\nобеспечения необходимым оборудованием для исполнения должностных обязанностей\nпередаёт сотруднику {lastName} {initials}, а сотрудник принимает от учебного учреждения\nследующие расходные материалы:\n\n");
                            mainText.Font("Times New Roman");
                            mainText.FontSize(12);
                            mainText.IndentationFirstLine = 26;
                            mainText.Alignment = Alignment.both;
                        }

                        using (var typeContext = new TypeCharacteristicsContext())
                        using (var valueContext = new ValueCharacteristicsContext())
                        {
                            var typeChar = typeContext.TypeCharacteristics
                                .FirstOrDefault(x => x.Id == selectedMaterial.CharacteristicsType);
                            var valueChar = valueContext.ValueCharacteristics
                                .FirstOrDefault(x => x.Id == selectedMaterial.IdValue);

                            var equipmentInfo = document.InsertParagraph($" {typeChar?.Name} {rasMat.Name}, в количестве {rasMat.Quantity} шт, стоимостью {valueChar?.Znachenie} руб. \n\n\n")
                                .Font("Times New Roman")
                                .FontSize(12)
                                .Alignment = Alignment.center;
                        }

                        if (currentUser != null)
                        {
                            var fioParts = currentUser.FIO.Split(' ');
                            string lastName = fioParts[0];
                            string initials = $"{fioParts[1][0]}.{fioParts[2][0]}.";
                            var paragraph = document.InsertParagraph($"{lastName} {initials}       ____________________     ________________")
                                .Font("Times New Roman")
                                .FontSize(12)
                                .Alignment = Alignment.left;
                        }

                        document.Save();
                    }

                    MessageBox.Show("Документ успешно сгенерирован по пути: Desktop\\YP02\\bin\\Debug\\net6.0-windows");
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
