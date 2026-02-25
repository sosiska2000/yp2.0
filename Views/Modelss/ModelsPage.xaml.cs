using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Modelss
{
    public partial class ModelsPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<Model> _allModels;
        private List<EquipmentType> _equipmentTypes;

        public ModelsPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            TypeFilterBox.SelectionChanged += TypeFilterBox_SelectionChanged;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allModels = await _apiService.GetModelsAsync();
                _equipmentTypes = await _apiService.GetEquipmentTypesAsync();

                TypeFilterBox.ItemsSource = null;
                TypeFilterBox.Items.Clear();
                TypeFilterBox.Items.Add(new EquipmentType { Id = 0, Name = "Все типы" });
                foreach (var type in _equipmentTypes)
                    TypeFilterBox.Items.Add(type);
                TypeFilterBox.SelectedIndex = 0;

                DataGrid.ItemsSource = _allModels;
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
            if (_allModels == null) return;

            var filtered = _allModels.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                filtered = filtered.Where(x => x.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (TypeFilterBox.SelectedItem is EquipmentType selectedType && selectedType.Id > 0)
            {
                filtered = filtered.Where(x => x.EquipmentTypeId == selectedType.Id);
            }

            DataGrid.ItemsSource = filtered.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new ModelEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Model;
            if (selected != null)
            {
                _mainFrame.Navigate(new ModelEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Model;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить модель '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteModelAsync(selected.Id);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }
    }
}