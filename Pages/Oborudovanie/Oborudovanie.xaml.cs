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
using ExcelDataReader;
using Microsoft.Win32;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Path = System.IO.Path;

namespace EquipmentManagement.Client.Pages.Oborudovanie
{
    /// <summary>
    /// Логика взаимодействия для Oborudovanie.xaml
    /// </summary>
    public partial class Oborudovanie : Page
    {
        private readonly OborudovanieContext _oborudovanieContext;
        private readonly UsersContext _usersContext;
        private readonly ViewModelContext _viewModelContext;
        private Models.Users _currentUser;
        private Item _selectedItem;
        private List<Models.Oborudovanie> _allEquipment;

        public OborudovanieContext OborudovanieContext => _oborudovanieContext;

        public Oborudovanie()
        {
            InitializeComponent();

            _oborudovanieContext = new OborudovanieContext();
            _usersContext = new UsersContext();
            _viewModelContext = new ViewModelContext();

            _currentUser = MainWindow.init.CurrentUser;
            if (_currentUser != null && _currentUser.Role == "Администратор")
            {
                addBtn.Visibility = Visibility.Visible;
                exportDoc.Visibility = Visibility.Visible;
                exportDoc1.Visibility = Visibility.Visible;
                import.Visibility = Visibility.Visible;
            }

            LoadEquipmentAsync().ConfigureAwait(false);
        }

        private async Task LoadEquipmentAsync()
        {
            try
            {
                _allEquipment = await Task.Run(() => _oborudovanieContext.Oborudovanie.ToList());

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var item in _allEquipment)
                    {
                        var itemControl = new Item(item, this);
                        itemControl.SelectionChanged += ItemControl_SelectionChanged;
                        parent.Children.Add(itemControl);
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при загрузке оборудования: ", ex);
            }
        }

        public async Task RemoveEquipmentAsync(Models.Oborudovanie equipment)
        {
            try
            {
                _oborudovanieContext.Oborudovanie.Remove(equipment);
                await _oborudovanieContext.SaveChangesAsync();
                _allEquipment.Remove(equipment);
            }
            catch (Exception ex)
            {
                await LogError("Ошибка удаления", ex);
                throw;
            }
        }

        private void ItemControl_SelectionChanged(object sender, EventArgs e)
        {
            var clickedItem = (Item)sender;

            // Если кликнули на уже выделенный элемент - снимаем выделение
            if (_selectedItem == clickedItem)
            {
                _selectedItem.IsSelected = false;
                _selectedItem = null;
                return;
            }

            // Снимаем выделение с предыдущего элемента
            if (_selectedItem != null)
            {
                _selectedItem.IsSelected = false;
            }

            // Устанавливаем новое выделение
            _selectedItem = clickedItem;
            _selectedItem.IsSelected = true;
        }

        private Models.Oborudovanie GetSelectedEquipment()
        {
            return _selectedItem?.Oborudovanie;
        }

        private async void KeyDown_Search(object sender, KeyEventArgs e)
        {
            try
            {
                string searchText = search.Text.ToLower();

                var filteredItems = string.IsNullOrWhiteSpace(searchText)
                    ? _allEquipment
                    : _allEquipment.Where(x => x.Name.ToLower().Contains(searchText)).ToList();

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var item in filteredItems)
                    {
                        var itemControl = new Item(item, this);
                        itemControl.SelectionChanged += ItemControl_SelectionChanged;

                        // Восстанавливаем выделение после поиска
                        if (_selectedItem != null && item.Id == _selectedItem.Oborudovanie.Id)
                        {
                            itemControl.IsSelected = true;
                            _selectedItem = itemControl;
                        }

                        parent.Children.Add(itemControl);
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при поиске: ", ex);
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
                LogError("Ошибка при возврате в меню", ex).ConfigureAwait(false);
            }
        }

        private async void SortUp(object sender, RoutedEventArgs e)
        {
            try
            {
                var sorted = await Task.Run(() => _allEquipment.OrderBy(x => x.Name).ToList()).ConfigureAwait(false);

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var oborudovanie in sorted)
                    {
                        parent.Children.Add(new Item(oborudovanie, this));
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при сортировке: ", ex);
            }
        }

        private async void SortDown(object sender, RoutedEventArgs e)
        {
            try
            {
                var sorted = await Task.Run(() => _allEquipment.OrderByDescending(x => x.Name).ToList()).ConfigureAwait(false);

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var oborudovanie in sorted)
                    {
                        parent.Children.Add(new Item(oborudovanie, this));
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при сортировке: ", ex);
            }
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Oborudovanie.Add(this, null));
            }
            catch (Exception ex)
            {
                LogError("Ошибка: ", ex).ConfigureAwait(false);
            }
        }

        private async void ExportObor(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEquipment = GetSelectedEquipment();
                if (selectedEquipment == null)
                {
                    MessageBox.Show("Пожалуйста, выберите оборудование для генерации отчета.");
                    return;
                }

                string currentDate = DateTime.Now.ToString("dd.MM.yyyy");

                await Task.Run(() =>
                {
                    var oborudovanie = _oborudovanieContext.Oborudovanie
                        .FirstOrDefault(x => x.Id == selectedEquipment.Id);

                    if (oborudovanie == null)
                    {
                        MessageBox.Show("Оборудование не найдено в базе данных.");
                        return;
                    }

                    var currentUser = _usersContext.Users.FirstOrDefault(x => x.Role == "Сотрудник");

                    using (DocX document = DocX.Create("Akt_Priema_Peredachi.docx"))
                    {
                        // Добавляем заголовок
                        document.InsertParagraph("АКТ\nприема-передачи оборудования\n\n")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.center;

                        // Добавляем информацию о месте и дате
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
                            var mainText = document.InsertParagraph($"КГАПОУ Пермский Авиационный техникум им. А.Д. Швецова в целях\nобеспечения необходимым оборудованием для исполнения должностных обязанностей\nпередаёт сотруднику {lastName} {initials}, а сотрудник принимает от учебного учреждения\nследующее оборудование:\n\n");
                            mainText.Font("Times New Roman");
                            mainText.FontSize(12);
                            mainText.IndentationFirstLine = 26;
                            mainText.Alignment = Alignment.both;
                        }

                        var model = _viewModelContext.ViewModel
                            .FirstOrDefault(x => x.Id == selectedEquipment.IdModelObor);

                        var equipmentInfo = document.InsertParagraph($" {oborudovanie.Name} {model?.Name}, серийный номер {oborudovanie.InventNumber}, стоимостью {oborudovanie.PriceObor} руб. \n\n\n")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.center;

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
                });

                MessageBox.Show("Документ успешно сгенерирован по пути: Desktop\\YP02\\bin\\Debug\\net6.0-windows");
            }
            catch (Exception ex)
            {
                await LogError("Ошибка: ", ex);
            }
        }

        private async void ExportObor1(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEquipment = GetSelectedEquipment();
                if (selectedEquipment == null)
                {
                    MessageBox.Show("Пожалуйста, выберите оборудование для генерации отчета.");
                    return;
                }

                await Task.Run(() =>
                {
                    string currentDate = DateTime.Now.ToString("dd.MM.yyyy");

                    var oborudovanie = _oborudovanieContext.Oborudovanie
                        .FirstOrDefault(x => x.Id == selectedEquipment.Id);

                    if (oborudovanie == null)
                    {
                        MessageBox.Show("Оборудование не найдено в базе данных.");
                        return;
                    }

                    var currentUser = _usersContext.Users.FirstOrDefault(x => x.Role == "Сотрудник");

                    using (DocX document = DocX.Create("Akt_Priema_Peredachi_Vrem_Polz.docx"))
                    {
                        document.InsertParagraph("АКТ\nприема-передачи оборудования на временное пользование\n\n")
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
                            var mainText = document.InsertParagraph($"КГАПОУ Пермский Авиационный техникум им. А.Д. Швецова в целях\nобеспечения необходимым оборудованием для исполнения должностных обязанностей\nпередаёт сотруднику {lastName} {initials}, а сотрудник принимает от учебного учреждения\nследующее оборудование:\n\n");
                            mainText.Font("Times New Roman");
                            mainText.FontSize(12);
                            mainText.IndentationFirstLine = 26;
                            mainText.Alignment = Alignment.both;
                        }

                        var model = _viewModelContext.ViewModel
                            .FirstOrDefault(x => x.Id == selectedEquipment.IdModelObor);

                        var equipmentInfo = document.InsertParagraph($" {oborudovanie.Name} {model?.Name}, серийный номер {oborudovanie.InventNumber}, стоимостью {oborudovanie.PriceObor} руб. \n\n")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.center;

                        var lastText = document.InsertParagraph($"По окончанию должностных работ  «__»  ____________  20___  года, работник\nобязуется вернуть полученное оборудование.\n\n");
                        lastText.Font("Times New Roman");
                        lastText.FontSize(12);
                        lastText.IndentationFirstLine = 26;
                        lastText.Alignment = Alignment.both;

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
                });

                MessageBox.Show("Документ успешно сгенерирован по пути: Desktop\\YP02\\bin\\Debug\\net6.0-windows");
            }
            catch (Exception ex)
            {
                await LogError("Ошибка: ", ex);
            }
        }

        private List<Models.Oborudovanie> ReadExcelFile(string filePath)
        {
            List<Models.Oborudovanie> equipmentList = new List<Models.Oborudovanie>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Используем конфигурацию чтения
                    var configuration = new ExcelReaderConfiguration
                    {
                        // Указываем настройки чтения
                    };

                    // Читаем данные вручную
                    do
                    {
                        // Пропускаем заголовок
                        if (!reader.Read())
                            continue;

                        while (reader.Read())
                        {
                            try
                            {
                                // Получаем значения ячеек
                                string userFIO = reader.GetValue(0)?.ToString()?.Trim();

                                if (string.IsNullOrEmpty(userFIO))
                                    continue;

                                var user = _usersContext.Users.FirstOrDefault(u => u.FIO == userFIO);

                                if (user == null)
                                {
                                    MessageBox.Show($"Ошибка: Пользователь '{userFIO}' не найден в базе!",
                                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    continue;
                                }

                                string name = reader.GetValue(1)?.ToString() ?? "Не указано";
                                string inventNumber = reader.GetValue(2)?.ToString() ?? "Не указано";

                                equipmentList.Add(new Models.Oborudovanie
                                {
                                    Name = name,
                                    InventNumber = inventNumber,
                                    PriceObor = "Не указано",
                                    IdResponUser = user.Id,
                                    IdTimeResponUser = user.Id,
                                    IdClassroom = 8,
                                    IdNapravObor = 7,
                                    IdStatusObor = 9,
                                    IdModelObor = 6,
                                    Comments = "Импортировано из Excel"
                                });
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Ошибка при чтении строки: {ex.Message}");
                            }
                        }
                    } while (reader.NextResult());
                }
            }
            return equipmentList;
        }

        private async Task SaveToDatabaseAsync(List<Models.Oborudovanie> equipmentList)
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var context = new OborudovanieContext())
                    {
                        context.Oborudovanie.AddRange(equipmentList);
                        context.SaveChanges();
                    }
                });

                // Обновляем кэш после добавления новых записей
                _allEquipment = await Task.Run(() => _oborudovanieContext.Oborudovanie.ToList());
            }
            catch (Exception ex)
            {
                await LogError("Ошибка: ", ex);
            }
        }

        private async void GoImport(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files|*.xls;*.xlsx",
                    Title = "Выберите файл Excel"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    var equipmentList = await Task.Run(() => ReadExcelFile(filePath));

                    // Сохраняем данные в базу
                    await SaveToDatabaseAsync(equipmentList);

                    MessageBox.Show("Импорт завершён успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    await Dispatcher.InvokeAsync(() =>
                    {
                        parent.Children.Clear();
                        foreach (var item in _allEquipment)
                        {
                            parent.Children.Add(new Item(item, this));
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await LogError("Ошибка: ", ex);
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
