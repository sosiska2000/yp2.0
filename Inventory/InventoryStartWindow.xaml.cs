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

namespace EquipmentManagement.Client.Inventory
{
    /// <summary>
    /// Логика взаимодействия для InventoryStartWindow.xaml
    /// </summary>
    public partial class InventoryStartWindow : Window
    {
        public string InventoryName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string InventoryType { get; private set; }

        public InventoryStartWindow()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(7);

            Loaded += (s, e) => InventoryNameTextBox.Focus();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(InventoryNameTextBox.Text))
            {
                MessageBox.Show("Введите название инвентаризации", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                InventoryNameTextBox.Focus();
                return;
            }

            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты начала и окончания", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохраняем данные
            InventoryName = InventoryNameTextBox.Text.Trim();
            StartDate = StartDatePicker.SelectedDate.Value;
            EndDate = EndDatePicker.SelectedDate.Value;

            // Определяем тип инвентаризации
            if (AllEquipmentRadio.IsChecked == true)
                InventoryType = "all";
            else if (SelectedEquipmentRadio.IsChecked == true)
                InventoryType = "selected";
            else if (ByRoomRadio.IsChecked == true)
                InventoryType = "by_room";

            this.DialogResult = true;
            this.Close();
        }
    }
}
