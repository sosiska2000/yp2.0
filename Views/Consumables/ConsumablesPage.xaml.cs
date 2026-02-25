using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Consumables
{
    public partial class ConsumablesPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<Consumable> _allConsumables;
        private List<ConsumableType> _consumableTypes;

        public ConsumablesPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            AttributesButton.Click += AttributesButton_Click;
            TypeFilterBox.SelectionChanged += TypeFilterBox_SelectionChanged;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allConsumables = await _apiService.GetConsumablesListAsync();
                _consumableTypes = await _apiService.GetConsumableTypesAsync();

                TypeFilterBox.ItemsSource = null;
                TypeFilterBox.Items.Clear();
                TypeFilterBox.Items.Add(new ConsumableType { Id = 0, Name = "Все типы" });
                foreach (var type in _consumableTypes)
                    TypeFilterBox.Items.Add(type);
                TypeFilterBox.SelectedIndex = 0;

                DataGrid.ItemsSource = _allConsumables;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.GoBack();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterData();
        }

        private void TypeFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            if (_allConsumables == null) return;

            var filtered = _allConsumables.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                filtered = filtered.Where(x => x.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (TypeFilterBox.SelectedItem is ConsumableType selectedType && selectedType.Id > 0)
            {
                filtered = filtered.Where(x => x.ConsumableTypeId == selectedType.Id);
            }

            DataGrid.ItemsSource = filtered.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new ConsumableEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Consumable;
            if (selected != null)
            {
                _mainFrame.Navigate(new ConsumableEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Consumable;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить расходный материал '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteConsumableAsync(selected.Id);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void AttributesButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Consumable;
            if (selected != null)
            {
                _mainFrame.Navigate(new ConsumableAttributesPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите расходный материал для просмотра характеристик");
            }
        }
    }
}