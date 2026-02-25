using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Consumables
{
    public partial class ConsumableEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Consumable _currentConsumable;

        public ConsumableEditPage(Frame mainFrame, Consumable consumable = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentConsumable = consumable ?? new Consumable();

            if (consumable != null)
            {
                TitleText.Text = "Редактирование расходного материала";
                NameBox.Text = consumable.Name;
                DescriptionBox.Text = consumable.Description;
                ReceiptDateBox.Text = consumable.ReceiptDate?.ToString("dd.MM.yyyy");
                QuantityBox.Text = consumable.Quantity.ToString();
                ConsumableTypeIdBox.Text = consumable.ConsumableTypeId?.ToString();
                ResponsibleUserIdBox.Text = consumable.ResponsibleUserId?.ToString();
                TempResponsibleUserIdBox.Text = consumable.TempResponsibleUserId?.ToString();
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

            _currentConsumable.Name = NameBox.Text;
            _currentConsumable.Description = DescriptionBox.Text;

            if (DateTime.TryParse(ReceiptDateBox.Text, out DateTime receiptDate))
                _currentConsumable.ReceiptDate = receiptDate;
            else
                _currentConsumable.ReceiptDate = null;

            if (int.TryParse(QuantityBox.Text, out int quantity))
                _currentConsumable.Quantity = quantity;
            else
                _currentConsumable.Quantity = 0;

            if (int.TryParse(ConsumableTypeIdBox.Text, out int typeId))
                _currentConsumable.ConsumableTypeId = typeId;
            else
                _currentConsumable.ConsumableTypeId = null;

            if (int.TryParse(ResponsibleUserIdBox.Text, out int responsibleId))
                _currentConsumable.ResponsibleUserId = responsibleId;
            else
                _currentConsumable.ResponsibleUserId = null;

            if (int.TryParse(TempResponsibleUserIdBox.Text, out int tempId))
                _currentConsumable.TempResponsibleUserId = tempId;
            else
                _currentConsumable.TempResponsibleUserId = null;

            try
            {
                if (_currentConsumable.Id == 0)
                    await _apiService.AddConsumableAsync(_currentConsumable);
                else
                    await _apiService.UpdateConsumableAsync(_currentConsumable);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}