using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models; // Для моделей
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Equipment
{
    public partial class EquipmentPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<Models.Equipment> _allEquipment; // Явно указываем Models.Equipment
        private List<Status> _statuses;
        private List<Classroom> _classrooms;

        public EquipmentPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            NetworkButton.Click += NetworkButton_Click;
            SoftwareButton.Click += SoftwareButton_Click;
            StatusFilterBox.SelectionChanged += FilterBox_SelectionChanged;
            ClassroomFilterBox.SelectionChanged += FilterBox_SelectionChanged;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allEquipment = await _apiService.GetEquipmentListAsync();
                _statuses = await _apiService.GetStatusesAsync();
                _classrooms = await _apiService.GetClassroomsAsync();
                
                StatusFilterBox.ItemsSource = null;
                StatusFilterBox.Items.Clear();
                StatusFilterBox.Items.Add(new Status { Id = 0, Name = "Все статусы" });
                foreach (var status in _statuses)
                    StatusFilterBox.Items.Add(status);
                StatusFilterBox.SelectedIndex = 0;

                ClassroomFilterBox.ItemsSource = null;
                ClassroomFilterBox.Items.Clear();
                ClassroomFilterBox.Items.Add(new Classroom { Id = 0, Name = "Все аудитории" });
                foreach (var classroom in _classrooms)
                    ClassroomFilterBox.Items.Add(classroom);
                ClassroomFilterBox.SelectedIndex = 0;
                
                DataGrid.ItemsSource = _allEquipment;
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

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            if (_allEquipment == null) return;

            var filtered = _allEquipment.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                filtered = filtered.Where(x => x.Name.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (StatusFilterBox.SelectedItem is Status selectedStatus && selectedStatus.Id > 0)
            {
                filtered = filtered.Where(x => x.StatusId == selectedStatus.Id);
            }

            if (ClassroomFilterBox.SelectedItem is Classroom selectedClassroom && selectedClassroom.Id > 0)
            {
                filtered = filtered.Where(x => x.ClassroomId == selectedClassroom.Id);
            }

            DataGrid.ItemsSource = filtered.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new EquipmentEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Equipment; // Явно указываем Models.Equipment
            if (selected != null)
            {
                _mainFrame.Navigate(new EquipmentEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Equipment; // Явно указываем Models.Equipment
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить оборудование '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteEquipmentAsync(selected.Id);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void NetworkButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Equipment; // Явно указываем Models.Equipment
            if (selected != null)
            {
                // _mainFrame.Navigate(new NetworkSettings.NetworkSettingsPage(_mainFrame, selected));
                MessageBox.Show("Перейти к сетевым настройкам");
            }
            else
            {
                MessageBox.Show("Выберите оборудование");
            }
        }

        private void SoftwareButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as Models.Equipment; // Явно указываем Models.Equipment
            if (selected != null)
            {
                MessageBox.Show("Перейти к ПО");
            }
            else
            {
                MessageBox.Show("Выберите оборудование");
            }
        }
    }
}