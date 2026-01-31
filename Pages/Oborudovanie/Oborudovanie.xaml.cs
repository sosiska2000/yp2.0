using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EquipmentManagement.Client.Context;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;
using Microsoft.Win32;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace EquipmentManagement.Client.Pages.Oborudovanie
{
    public partial class Oborudovanie : Page
    {
        private readonly ApiClient _apiClient;
        private Models.Users _currentUser;
        private Item _selectedItem;
        private List<Models.Oborudovanie> _allEquipment;
        private readonly UsersContext _usersContext;

        public Oborudovanie()
        {
            InitializeComponent();

            _apiClient = new ApiClient();
            _usersContext = new UsersContext();
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
                // Получаем оборудование через API
                var apiEquipment = await _apiClient.GetOborudovanieAsync();

                // Конвертируем API модели в наши локальные модели
                _allEquipment = apiEquipment.Select(apiModel => new Models.Oborudovanie
                {
                    Id = apiModel.Id,
                    Name = apiModel.Nazvanie,
                    InventNumber = apiModel.InventarnyiNomer,
                    PriceObor = apiModel.Stoimost.ToString("F2"),
                    IdResponUser = apiModel.OtvetstvennyiPolzovatelId,
                    IdTimeResponUser = apiModel.VremennoOtvetstvennyiPolzovatelId,
                    IdClassroom = apiModel.AuditoriaId,
                    IdNapravObor = apiModel.NapravlenieId,
                    IdStatusObor = apiModel.StatusId,
                    IdModelObor = apiModel.VidModeliId,
                    Comments = apiModel.Kommentarii
                }).ToList();

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
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task RemoveEquipmentAsync(Models.Oborudovanie equipment)
        {
            try
            {
                var success = await _apiClient.DeleteOborudovanieAsync(equipment.Id);

                if (success)
                {
                    _allEquipment.Remove(equipment);
                    MessageBox.Show("Оборудование успешно удалено!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить оборудование", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                await LogError("Ошибка удаления", ex);
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void ItemControl_SelectionChanged(object sender, EventArgs e)
        {
            var clickedItem = (Item)sender;

            if (_selectedItem == clickedItem)
            {
                _selectedItem.IsSelected = false;
                _selectedItem = null;
                return;
            }

            if (_selectedItem != null)
            {
                _selectedItem.IsSelected = false;
            }

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
                string searchText = search.Text;

                var apiEquipment = await _apiClient.GetOborudovanieAsync(searchText);

                var filteredItems = apiEquipment.Select(apiModel => new Models.Oborudovanie
                {
                    Id = apiModel.Id,
                    Name = apiModel.Nazvanie,
                    InventNumber = apiModel.InventarnyiNomer,
                    PriceObor = apiModel.Stoimost.ToString("F2"),
                    IdResponUser = apiModel.OtvetstvennyiPolzovatelId,
                    IdTimeResponUser = apiModel.VremennoOtvetstvennyiPolzovatelId,
                    IdClassroom = apiModel.AuditoriaId,
                    IdNapravObor = apiModel.NapravlenieId,
                    IdStatusObor = apiModel.StatusId,
                    IdModelObor = apiModel.VidModeliId,
                    Comments = apiModel.Kommentarii
                }).ToList();

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var item in filteredItems)
                    {
                        var itemControl = new Item(item, this);
                        itemControl.SelectionChanged += ItemControl_SelectionChanged;

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
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                var apiEquipment = await _apiClient.GetOborudovanieAsync(sort: "nazvanie");

                var sorted = apiEquipment.Select(apiModel => new Models.Oborudovanie
                {
                    Id = apiModel.Id,
                    Name = apiModel.Nazvanie,
                    InventNumber = apiModel.InventarnyiNomer,
                    PriceObor = apiModel.Stoimost.ToString("F2"),
                    IdResponUser = apiModel.OtvetstvennyiPolzovatelId,
                    IdTimeResponUser = apiModel.VremennoOtvetstvennyiPolzovatelId,
                    IdClassroom = apiModel.AuditoriaId,
                    IdNapravObor = apiModel.NapravlenieId,
                    IdStatusObor = apiModel.StatusId,
                    IdModelObor = apiModel.VidModeliId,
                    Comments = apiModel.Kommentarii
                }).ToList();

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
                MessageBox.Show($"Ошибка сортировки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SortDown(object sender, RoutedEventArgs e)
        {
            try
            {
                var apiEquipment = await _apiClient.GetOborudovanieAsync();
                var sorted = apiEquipment
                    .OrderByDescending(x => x.Nazvanie)
                    .Select(apiModel => new Models.Oborudovanie
                    {
                        Id = apiModel.Id,
                        Name = apiModel.Nazvanie,
                        InventNumber = apiModel.InventarnyiNomer,
                        PriceObor = apiModel.Stoimost.ToString("F2"),
                        IdResponUser = apiModel.OtvetstvennyiPolzovatelId,
                        IdTimeResponUser = apiModel.VremennoOtvetstvennyiPolzovatelId,
                        IdClassroom = apiModel.AuditoriaId,
                        IdNapravObor = apiModel.NapravlenieId,
                        IdStatusObor = apiModel.StatusId,
                        IdModelObor = apiModel.VidModeliId,
                        Comments = apiModel.Kommentarii
                    }).ToList();

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
                MessageBox.Show($"Ошибка сортировки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Пожалуйста, выберите оборудование для генерации отчета.",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string currentDate = DateTime.Now.ToString("dd.MM.yyyy");

                await Task.Run(() =>
                {
                    var currentUser = _usersContext.Users.FirstOrDefault(x => x.Role == "Сотрудник");

                    using (DocX document = DocX.Create("Akt_Priema_Peredachi.docx"))
                    {
                        document.InsertParagraph("АКТ\nприема-передачи оборудования\n\n")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.center;

                        document.InsertParagraph($"г. Пермь")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.left;

                        document.InsertParagraph($"{currentDate}\n")
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

                        var equipmentInfo = document.InsertParagraph($" {selectedEquipment.Name}, инвентарный номер {selectedEquipment.InventNumber}, стоимостью {selectedEquipment.PriceObor} руб. \n\n\n")
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

                MessageBox.Show("Документ успешно сгенерирован!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await LogError("Ошибка: ", ex);
                MessageBox.Show($"Ошибка генерации документа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExportObor1(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEquipment = GetSelectedEquipment();
                if (selectedEquipment == null)
                {
                    MessageBox.Show("Пожалуйста, выберите оборудование для генерации отчета.",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await Task.Run(() =>
                {
                    string currentDate = DateTime.Now.ToString("dd.MM.yyyy");
                    var currentUser = _usersContext.Users.FirstOrDefault(x => x.Role == "Сотрудник");

                    using (DocX document = DocX.Create("Akt_Priema_Peredachi_Vrem_Polz.docx"))
                    {
                        document.InsertParagraph("АКТ\nприема-передачи оборудования на временное пользование\n\n")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.center;

                        document.InsertParagraph($"г. Пермь")
                            .Font("Times New Roman")
                            .FontSize(12)
                            .Alignment = Alignment.left;

                        document.InsertParagraph($"{currentDate}\n")
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

                        var equipmentInfo = document.InsertParagraph($" {selectedEquipment.Name}, инвентарный номер {selectedEquipment.InventNumber}, стоимостью {selectedEquipment.PriceObor} руб. \n\n")
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

                MessageBox.Show("Документ успешно сгенерирован!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await LogError("Ошибка: ", ex);
                MessageBox.Show($"Ошибка генерации документа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GoImport(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel Files|*.xls;*.xlsx|CSV Files|*.csv|Text Files|*.txt",
                    Title = "Выберите файл для импорта"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    // Показываем прогресс
                    MessageBox.Show("Импорт начат. Пожалуйста, подождите...", "Импорт",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // TODO: Нужно реализовать метод импорта в ApiClient
                    // var result = await _apiClient.ImportEquipmentFromFileAsync(filePath);

                    MessageBox.Show("Функция импорта через API пока не реализована. Используйте локальную базу или добавьте метод в ApiClient.",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем список после импорта
                    await LoadEquipmentAsync();
                }
            }
            catch (Exception ex)
            {
                await LogError("Ошибка: ", ex);
                MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LogError(string message, Exception ex)
        {
            Debug.WriteLine($"{message}: {ex.Message}");

            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "error.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath) ?? string.Empty);
                await File.AppendAllTextAsync(logPath, $"{DateTime.Now}: {message} - {ex.Message}\n{ex.StackTrace}\n\n");
            }
            catch (Exception logEx)
            {
                Debug.WriteLine($"Ошибка при записи в лог-файл: {logEx.Message}");
            }
        }
    }
}