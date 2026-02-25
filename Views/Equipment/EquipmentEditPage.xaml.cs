using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models; // Для моделей
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Equipment
{
    public partial class EquipmentEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Models.Equipment _currentEquipment; // Явно указываем Models.Equipment

        public EquipmentEditPage(Frame mainFrame, Models.Equipment equipment = null) // Явно указываем Models.Equipment
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentEquipment = equipment ?? new Models.Equipment(); // Явно указываем Models.Equipment

            if (equipment != null)
            {
                TitleText.Text = "Редактирование оборудования";
                NameBox.Text = equipment.Name;
                InventoryNumberBox.Text = equipment.InventoryNumber;
                ClassroomIdBox.Text = equipment.ClassroomId?.ToString();
                ResponsibleUserIdBox.Text = equipment.ResponsibleUserId?.ToString();
                TempResponsibleUserIdBox.Text = equipment.TempResponsibleUserId?.ToString();
                CostBox.Text = equipment.Cost?.ToString();
                DirectionIdBox.Text = equipment.DirectionId?.ToString();
                StatusIdBox.Text = equipment.StatusId?.ToString();
                ModelIdBox.Text = equipment.ModelId?.ToString();
                PhotoPathBox.Text = equipment.PhotoPath;
                CommentBox.Text = equipment.Comment;
            }

            CancelButton.Click += CancelButton_Click;
            SaveButton.Click += SaveButton_Click;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.GoBack();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Название обязательно для заполнения");
                return;
            }

            if (string.IsNullOrWhiteSpace(InventoryNumberBox.Text))
            {
                MessageBox.Show("Инвентарный номер обязателен для заполнения");
                return;
            }

            _currentEquipment.Name = NameBox.Text;
            _currentEquipment.InventoryNumber = InventoryNumberBox.Text;
            _currentEquipment.PhotoPath = PhotoPathBox.Text;
            _currentEquipment.Comment = CommentBox.Text;

            if (int.TryParse(ClassroomIdBox.Text, out int classroomId))
                _currentEquipment.ClassroomId = classroomId;
            else
                _currentEquipment.ClassroomId = null;

            if (int.TryParse(ResponsibleUserIdBox.Text, out int responsibleId))
                _currentEquipment.ResponsibleUserId = responsibleId;
            else
                _currentEquipment.ResponsibleUserId = null;

            if (int.TryParse(TempResponsibleUserIdBox.Text, out int tempId))
                _currentEquipment.TempResponsibleUserId = tempId;
            else
                _currentEquipment.TempResponsibleUserId = null;

            if (decimal.TryParse(CostBox.Text, out decimal cost))
                _currentEquipment.Cost = cost;
            else
                _currentEquipment.Cost = null;

            if (int.TryParse(DirectionIdBox.Text, out int directionId))
                _currentEquipment.DirectionId = directionId;
            else
                _currentEquipment.DirectionId = null;

            if (int.TryParse(StatusIdBox.Text, out int statusId))
                _currentEquipment.StatusId = statusId;
            else
                _currentEquipment.StatusId = null;

            if (int.TryParse(ModelIdBox.Text, out int modelId))
                _currentEquipment.ModelId = modelId;
            else
                _currentEquipment.ModelId = null;

            try
            {
                if (_currentEquipment.Id == 0)
                    await _apiService.AddEquipmentAsync(_currentEquipment);
                else
                    await _apiService.UpdateEquipmentAsync(_currentEquipment);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}