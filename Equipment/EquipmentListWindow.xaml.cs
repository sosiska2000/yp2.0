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
using System.Windows.Shapes;

namespace EquipmentManagement.Client.Equipment
{
    /// <summary>
    /// Логика взаимодействия для EquipmentListWindow.xaml
    /// </summary>
    public partial class EquipmentListWindow : Window
    {
        public class EquipmentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string InventoryNumber { get; set; }
            public string Room { get; set; }
            public string Status { get; set; }
            public string Responsible { get; set; }
            public decimal Cost { get; set; }
            public string ImagePath { get; set; }

            public Brush StatusColor
            {
                get
                {
                    return Status switch
                    {
                        "Используется" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                        "На ремонте" => new SolidColorBrush(Color.FromRgb(255, 152, 0)),
                        "Сломано" => new SolidColorBrush(Color.FromRgb(244, 67, 54)),
                        "На выдаче" => new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                        _ => new SolidColorBrush(Color.FromRgb(158, 158, 158))
                    };
                }
            }
        }

        private List<EquipmentItem> _allEquipment;
        private List<EquipmentItem> _filteredEquipment;

        public EquipmentListWindow()
        {
            InitializeComponent();
            LoadTestData();

            EquipmentGrid.ItemsSource = _filteredEquipment;

            UpdateStatus();

            Loaded += (s, e) => SearchTextBox.Focus();
        }
        private void LoadTestData()
        {
            _allEquipment = new List<EquipmentItem>
            {
                new EquipmentItem { Id = 1, Name = "Ноутбук Dell T200", InventoryNumber = "12345",
                    Room = "Аудитория 101", Status = "Используется", Responsible = "Иванов И.И.", Cost = 45000 },
                new EquipmentItem { Id = 2, Name = "Проектор Epson EB-X41", InventoryNumber = "12346",
                    Room = "Аудитория 102", Status = "Используется", Responsible = "Петров П.П.", Cost = 35000 },
                new EquipmentItem { Id = 3, Name = "Интерактивная доска Smart", InventoryNumber = "12347",
                    Room = "Аудитория 103", Status = "На ремонте", Responsible = "Сидоров С.С.", Cost = 120000 },
                new EquipmentItem { Id = 4, Name = "Системный блок HP", InventoryNumber = "12348",
                    Room = "Лаборатория 201", Status = "Используется", Responsible = "Кузнецов К.К.", Cost = 25000 },
                new EquipmentItem { Id = 5, Name = "Монитор Samsung 24\"", InventoryNumber = "12349",
                    Room = "Лаборатория 201", Status = "Используется", Responsible = "Кузнецов К.К.", Cost = 15000 },
                new EquipmentItem { Id = 6, Name = "Принтер HP LaserJet", InventoryNumber = "12350",
                    Room = "Кабинет 301", Status = "Сломано", Responsible = "Смирнов С.С.", Cost = 18000 },
                new EquipmentItem { Id = 7, Name = "Сканер Canon CanoScan", InventoryNumber = "12351",
                    Room = "Кабинет 302", Status = "На выдаче", Responsible = "Фёдоров Ф.Ф.", Cost = 12000 },
                new EquipmentItem { Id = 8, Name = "ИБП APC 1500VA", InventoryNumber = "12352",
                    Room = "Серверная", Status = "Используется", Responsible = "Алексеев А.А.", Cost = 22000 },
            };

            _filteredEquipment = new List<EquipmentItem>(_allEquipment);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredEquipment = new List<EquipmentItem>(_allEquipment);
            }
            else
            {
                _filteredEquipment = _allEquipment
                    .Where(eq =>
                        (eq.Name?.ToLower().Contains(searchText) ?? false) ||
                        (eq.InventoryNumber?.ToLower().Contains(searchText) ?? false) ||
                        (eq.Room?.ToLower().Contains(searchText) ?? false) ||
                        (eq.Responsible?.ToLower().Contains(searchText) ?? false))
                    .ToList();
            }

            EquipmentGrid.ItemsSource = null;
            EquipmentGrid.ItemsSource = _filteredEquipment;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            StatusText.Text = $"Всего: {_filteredEquipment.Count} записей";
        }

        // ========== ОБРАБОТЧИКИ КНОПОК ==========

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно добавления КАК ДИАЛОГ
            var editWindow = new EquipmentEditWindow();
            bool? result = editWindow.ShowDialog();

            // Если пользователь сохранил новое оборудование
            if (result == true)
            {
                // Обновляем список
                RefreshButton_Click(null, null);
                MessageBox.Show("Оборудование добавлено!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            SearchTextBox_TextChanged(null, null);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null && int.TryParse(button.Tag.ToString(), out int id))
            {
                var equipment = _allEquipment.FirstOrDefault(eq => eq.Id == id);
                if (equipment != null)
                {
                    var editWindow = new EquipmentEditWindow(equipment);
                    bool? result = editWindow.ShowDialog();

                    if (result == true)
                    {
                        RefreshButton_Click(null, null);
                        MessageBox.Show("Оборудование обновлено!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null && int.TryParse(button.Tag.ToString(), out int id))
            {
                var equipment = _allEquipment.FirstOrDefault(eq => eq.Id == id);
                if (equipment != null)
                {
                    var detailsWindow = new EquipmentDetailsWindow(equipment);
                    detailsWindow.Show(); // ⚠️ Это просто окно просмотра, не диалог
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null && int.TryParse(button.Tag.ToString(), out int id))
            {
                var equipment = _allEquipment.FirstOrDefault(eq => eq.Id == id);
                if (equipment != null)
                {
                    var result = MessageBox.Show(
                        $"Удалить оборудование: {equipment.Name}?\nИнв. номер: {equipment.InventoryNumber}",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Здесь будет удаление через API
                        _allEquipment.RemoveAll(eq => eq.Id == id);
                        SearchTextBox_TextChanged(null, null);

                        MessageBox.Show("Оборудование удалено", "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void EquipmentGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Пустая реализация, можно добавить логику при выборе строки
        }

        // ⚠️ ВАЖНО: Не используем Close() с DialogResult в этом окне!
        // Это главное окно, закрываем его просто

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // ✅ Просто закрываем окно
        }
    }
}
