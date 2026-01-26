using Microsoft.Win32;
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
    /// Логика взаимодействия для EquipmentEditWindow.xaml
    /// </summary>
    public partial class EquipmentEditWindow : Window
    {
        private EquipmentListWindow.EquipmentItem _editingItem;
        private bool _isEditMode = false;
        public EquipmentEditWindow()
        {
            InitializeComponent();
            WindowTitle.Text = "Добавление нового оборудования";
            _isEditMode = false;
        }
        public EquipmentEditWindow(EquipmentListWindow.EquipmentItem item)
        {
            InitializeComponent();
            WindowTitle.Text = "Редактирование оборудования";
            _isEditMode = true;
            _editingItem = item;

            // Заполняем поля данными
            LoadEquipmentData();
        }

        private void LoadEquipmentData()
        {
            if (_editingItem == null) return;

            NameTextBox.Text = _editingItem.Name;
            InventoryNumberTextBox.Text = _editingItem.InventoryNumber;
            CostTextBox.Text = _editingItem.Cost.ToString();
            ModelTextBox.Text = "Dell T200"; // Пример

            // Выбираем соответствующие значения в ComboBox
            SelectComboBoxItem(RoomComboBox, _editingItem.Room);
            SelectComboBoxItem(StatusComboBox, _editingItem.Status);
            SelectComboBoxItem(ResponsibleComboBox, _editingItem.Responsible);
        }

        private void SelectComboBoxItem(System.Windows.Controls.ComboBox comboBox, string value)
        {
            foreach (var item in comboBox.Items)
            {
                if (item is System.Windows.Controls.ComboBoxItem comboBoxItem)
                {
                    if (comboBoxItem.Content.ToString() == value)
                    {
                        comboBox.SelectedItem = comboBoxItem;
                        break;
                    }
                }
            }
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
                Title = "Выберите фото оборудования"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var imagePath = openFileDialog.FileName;
                    // Здесь загружаем изображение
                    // PhotoImage.Source = new BitmapImage(new Uri(imagePath));

                    MessageBox.Show($"Выбрано фото: {System.IO.Path.GetFileName(imagePath)}",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки фото: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // PhotoImage.Source = null;
            MessageBox.Show("Фото удалено", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CheckNetworkButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка доступности сети (заглушка)
            MessageBox.Show("Проверка доступности сети...\n(реализовать без команды PING)",
                "Проверка сети", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValidateInput()
        {
            ErrorText.Text = "";

            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ErrorText.Text = "Введите название оборудования";
                NameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(InventoryNumberTextBox.Text))
            {
                ErrorText.Text = "Введите инвентарный номер";
                InventoryNumberTextBox.Focus();
                return false;
            }

            // Проверка, что инвентарный номер содержит только цифры
            if (!System.Text.RegularExpressions.Regex.IsMatch(InventoryNumberTextBox.Text, @"^\d+$"))
            {
                ErrorText.Text = "Инвентарный номер должен содержать только цифры";
                InventoryNumberTextBox.Focus();
                InventoryNumberTextBox.SelectAll();
                return false;
            }

            // Проверка стоимости
            if (!string.IsNullOrWhiteSpace(CostTextBox.Text))
            {
                if (!decimal.TryParse(CostTextBox.Text, out decimal cost))
                {
                    ErrorText.Text = "Стоимость должна быть числом";
                    CostTextBox.Focus();
                    CostTextBox.SelectAll();
                    return false;
                }

                if (cost < 0)
                {
                    ErrorText.Text = "Стоимость не может быть отрицательной";
                    CostTextBox.Focus();
                    CostTextBox.SelectAll();
                    return false;
                }
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                // Здесь будет сохранение в БД через API

                string message = _isEditMode
                    ? "Оборудование успешно обновлено!"
                    : "Новое оборудование успешно добавлено!";

                MessageBox.Show(message, "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
