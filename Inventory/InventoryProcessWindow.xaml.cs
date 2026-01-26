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
    /// Логика взаимодействия для InventoryProcessWindow.xaml
    /// </summary>
    public partial class InventoryProcessWindow : Window
    {
        public class InventoryItem
        {
            public bool IsFound { get; set; }
            public string Name { get; set; }
            public string InventoryNumber { get; set; }
            public string Room { get; set; }
            public string Status { get; set; }
            public string Comment { get; set; }
            public int Id { get; set; }
            public string ImagePath { get; set; }
            public string Responsible { get; set; }
        }

        private ObservableCollection<InventoryItem> _inventoryItems;
        private DateTime _lastUpdateTime;

        public InventoryProcessWindow(string inventoryName, List<int> equipmentIds)
        {
            InitializeComponent();
            InventoryTitleText.Text = $"Инвентаризация: {inventoryName}";
            UserInfoText.Text = $"Проверяющий: {LoginWindow.CurrentUser}";

            // Загружаем данные оборудования
            LoadInventoryData(equipmentIds);


            // Обновляем статистику
            UpdateStatistics();
            UpdateProgress();
        }
        private void LoadInventoryData(List<int> equipmentIds)
        {
            // Тестовые данные (потом заменим на загрузку из API)
            _inventoryItems = new ObservableCollection<InventoryItem>();

            var testData = new[]
            {
                new InventoryItem { Id = 1, Name = "Ноутбук Dell T200", InventoryNumber = "12345",
                    Room = "Аудитория 101", Status = "Используется", Responsible = "Иванов И.И.",
                    ImagePath = null, IsFound = false, Comment = "" },
                new InventoryItem { Id = 2, Name = "Проектор Epson", InventoryNumber = "12346",
                    Room = "Аудитория 102", Status = "Используется", Responsible = "Петров П.П.",
                    ImagePath = null, IsFound = false, Comment = "" },
                new InventoryItem { Id = 3, Name = "Интерактивная доска", InventoryNumber = "12347",
                    Room = "Аудитория 103", Status = "На ремонте", Responsible = "Сидоров С.С.",
                    ImagePath = null, IsFound = false, Comment = "" },
                new InventoryItem { Id = 4, Name = "Системный блок", InventoryNumber = "12348",
                    Room = "Лаборатория 201", Status = "Используется", Responsible = "Кузнецов К.К.",
                    ImagePath = null, IsFound = false, Comment = "" },
            };

            // Фильтруем по выбранным ID (для демо берём первые 4)
            foreach (var item in testData)
            {
                _inventoryItems.Add(item);
            }

            InventoryDataGrid.ItemsSource = _inventoryItems;
        }

        private void InventoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is InventoryItem selectedItem)
            {
                ShowEquipmentDetails(selectedItem);
                NoSelectionText.Visibility = Visibility.Collapsed;
                DetailsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoSelectionText.Visibility = Visibility.Visible;
                DetailsPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowEquipmentDetails(InventoryItem item)
        {
            EquipmentNameText.Text = item.Name;
            InventoryNumberText.Text = item.InventoryNumber;
            RoomText.Text = item.Room;
            ResponsibleText.Text = item.Responsible;
            StatusText.Text = item.Status;

            // Здесь можно загрузить фото, если оно есть
            // if (!string.IsNullOrEmpty(item.ImagePath))
            //     EquipmentImage.Source = new BitmapImage(new Uri(item.ImagePath));
        }

        private void UpdateStatistics()
        {
            int found = _inventoryItems.Count(i => i.IsFound && string.IsNullOrEmpty(i.Comment));
            int notFound = _inventoryItems.Count(i => !i.IsFound);
            int withComments = _inventoryItems.Count(i => !string.IsNullOrEmpty(i.Comment));


        }

        private void UpdateProgress()
        {
            int total = _inventoryItems.Count;
            int checkedCount = _inventoryItems.Count(i => i.IsFound || !string.IsNullOrEmpty(i.Comment));

            ProgressText.Text = $"Проверено: {checkedCount}/{total}";
            ProgressBar.Maximum = total;
            ProgressBar.Value = checkedCount;
        }

        
        private void MarkFoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is InventoryItem selectedItem)
            {
                selectedItem.IsFound = true;
                selectedItem.Comment = "";
                InventoryDataGrid.Items.Refresh();

                _lastUpdateTime = DateTime.Now;
                UpdateStatistics();
                UpdateProgress();
            }
        }

        private void MarkWithCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is InventoryItem selectedItem)
            {
                selectedItem.IsFound = true;

                // Показываем диалог для ввода комментария
                var commentDialog = new InputDialog("Введите комментарий:",
                    "Комментарий к оборудованию", selectedItem.Comment ?? "");

                if (commentDialog.ShowDialog() == true)
                {
                    selectedItem.Comment = commentDialog.InputText;
                }

                InventoryDataGrid.Items.Refresh();

                _lastUpdateTime = DateTime.Now;
                UpdateStatistics();
                UpdateProgress();
            }
        }

        private void MarkNotFoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryDataGrid.SelectedItem is InventoryItem selectedItem)
            {
                selectedItem.IsFound = false;

                var commentDialog = new InputDialog("Укажите причину:",
                    "Оборудование не найдено", selectedItem.Comment ?? "Не найдено в указанной аудитории");

                if (commentDialog.ShowDialog() == true)
                {
                    selectedItem.Comment = commentDialog.InputText;
                }

                InventoryDataGrid.Items.Refresh();

                _lastUpdateTime = DateTime.Now;
                UpdateStatistics();
                UpdateProgress();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь будет сохранение черновика в БД
            MessageBox.Show("Черновик инвентаризации сохранён", "Сохранение",
                MessageBoxButton.OK, MessageBoxImage.Information);

            _lastUpdateTime = DateTime.Now;
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            int uncheckedCount = _inventoryItems.Count(i => !i.IsFound && string.IsNullOrEmpty(i.Comment));

            if (uncheckedCount > 0)
            {
                var result = MessageBox.Show($"Осталось непроверенного оборудования: {uncheckedCount}.\n" +
                    "Всё равно завершить инвентаризацию?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            // Здесь будет сохранение результатов и закрытие инвентаризации

            MessageBox.Show("Инвентаризация успешно завершена!", "Завершение",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }
    }
}

