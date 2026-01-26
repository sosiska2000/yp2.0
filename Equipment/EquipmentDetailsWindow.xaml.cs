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
    /// Логика взаимодействия для EquipmentDetailsWindow.xaml
    /// </summary>
    public partial class EquipmentDetailsWindow : Window
    {
        private EquipmentListWindow.EquipmentItem _equipment;

        public EquipmentDetailsWindow(EquipmentListWindow.EquipmentItem equipment)
        {
            InitializeComponent();
            _equipment = equipment;
            LoadEquipmentDetails();
        }
        private void LoadEquipmentDetails()
        {
            TitleText.Text = $"Детали: {_equipment.Name}";
            EquipmentName.Text = _equipment.Name;
            InventoryNumberText.Text = _equipment.InventoryNumber;
            RoomText.Text = _equipment.Room;
            ResponsibleText.Text = _equipment.Responsible;
            StatusText.Text = _equipment.Status;
            CostText.Text = $"{_equipment.Cost:N0} руб.";

            // Примерные дополнительные данные
            DirectionText.Text = "Мехатроника";
            CommentText.Text = "Оборудование используется для проведения лабораторных работ. " +
                             "Установлено специализированное ПО для программирования ПЛК.";

            // Устанавливаем цвет статуса
            var statusBorder = (Border)StatusText.Parent;
            statusBorder.Background = _equipment.StatusColor;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EquipmentEditWindow(_equipment);
            if (editWindow.ShowDialog() == true)
            {
                // Обновляем данные после редактирования
                LoadEquipmentDetails();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
