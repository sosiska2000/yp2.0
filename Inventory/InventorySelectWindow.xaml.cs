using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace EquipmentManagement.Client.Inventory
{
    /// <summary>
    /// Логика взаимодействия для InventorySelectWindow.xaml
    /// </summary>
    public partial class InventorySelectWindow : Window
    {
        public class EquipmentItem
        {
            public bool IsSelected { get; set; }
            public string Name { get; set; }
            public string InventoryNumber { get; set; }
            public string Room { get; set; }
            public string Status { get; set; }
            public int Id { get; set; }
        }
        private ObservableCollection<EquipmentItem> _allEquipment;
        private ObservableCollection<EquipmentItem> _filteredEquipment;

        public List<int> SelectedEquipmentIds { get; private set; }

        public InventorySelectWindow(string inventoryName, DateTime startDate, DateTime endDate, string inventoryType)
        {
            InitializeComponent();
            InventoryInfoText.Text = $"Инвентаризация: {inventoryName}";
            DatesText.Text = $"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";

            // Загружаем тестовые данные
            LoadTestData();

            SelectedEquipmentIds = new List<int>();

            UpdateCounts();
        }
        private void LoadTestData()
        {
            _allEquipment = new ObservableCollection<EquipmentItem>
            {
                new EquipmentItem { Id = 1, Name = "Ноутбук Dell T200", InventoryNumber = "12345", Room = "Аудитория 101", Status = "Используется" },
                new EquipmentItem { Id = 2, Name = "Проектор Epson", InventoryNumber = "12346", Room = "Аудитория 102", Status = "Используется" },
                new EquipmentItem { Id = 3, Name = "Интерактивная доска", InventoryNumber = "12347", Room = "Аудитория 103", Status = "На ремонте" },
                new EquipmentItem { Id = 4, Name = "Системный блок", InventoryNumber = "12348", Room = "Лаборатория 201", Status = "Используется" },
                new EquipmentItem { Id = 5, Name = "Монитор Samsung", InventoryNumber = "12349", Room = "Лаборатория 201", Status = "Используется" },
                new EquipmentItem { Id = 6, Name = "Принтер HP", InventoryNumber = "12350", Room = "Кабинет 301", Status = "Сломано" },
                new EquipmentItem { Id = 7, Name = "Сканер Canon", InventoryNumber = "12351", Room = "Кабинет 302", Status = "Используется" },
                new EquipmentItem { Id = 8, Name = "ИБП APC", InventoryNumber = "12352", Room = "Серверная", Status = "Используется" },
            };

            _filteredEquipment = new ObservableCollection<EquipmentItem>(_allEquipment);
            EquipmentDataGrid.ItemsSource = _filteredEquipment;

            TotalCountText.Text = $"Всего: {_allEquipment.Count}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchTextBox.Text.ToLower();

            _filteredEquipment.Clear();

            var filtered = string.IsNullOrWhiteSpace(searchText)
                ? _allEquipment
                : _allEquipment.Where(eq =>
                    eq.Name.ToLower().Contains(searchText) ||
                    eq.InventoryNumber.ToLower().Contains(searchText) ||
                    eq.Room.ToLower().Contains(searchText));

            foreach (var item in filtered)
            {
                _filteredEquipment.Add(item);
            }
        }

        private void UpdateCounts()
        {
            int selectedCount = _allEquipment.Count(eq => eq.IsSelected);
            SelectedCountText.Text = $"Выбрано: {selectedCount} единиц оборудования";

            // Обновляем список выбранного
            var selectedItems = _allEquipment.Where(eq => eq.IsSelected).ToList();
            SelectedList.ItemsSource = selectedItems;

            // Сохраняем ID выбранного оборудования
            SelectedEquipmentIds = selectedItems.Select(eq => eq.Id).ToList();

            // Активируем кнопку только если что-то выбрано
            StartInventoryButton.IsEnabled = selectedCount > 0;
        }

        private void EquipmentDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            UpdateCounts();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _allEquipment)
            {
                item.IsSelected = true;
            }
            EquipmentDataGrid.Items.Refresh();
            UpdateCounts();
        }

        private void ClearSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _allEquipment)
            {
                item.IsSelected = false;
            }
            EquipmentDataGrid.Items.Refresh();
            UpdateCounts();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void StartInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEquipmentIds.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одно оборудование для инвентаризации",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}

