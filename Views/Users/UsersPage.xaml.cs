using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Users
{
    public partial class UsersPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private List<User> _allUsers;

        public UsersPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();

            BackButton.Click += BackButton_Click;
            SearchButton.Click += SearchButton_Click;
            AddButton.Click += AddButton_Click;
            EditButton.Click += EditButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            RoleFilterBox.SelectionChanged += RoleFilterBox_SelectionChanged;

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _allUsers = await _apiService.GetUsersAsync();

                RoleFilterBox.ItemsSource = null;
                RoleFilterBox.Items.Clear();
                RoleFilterBox.Items.Add("Все роли");
                RoleFilterBox.Items.Add("admin");
                RoleFilterBox.Items.Add("teacher");
                RoleFilterBox.Items.Add("employee");
                RoleFilterBox.SelectedIndex = 0;

                DataGrid.ItemsSource = _allUsers;
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

        private void RoleFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            if (_allUsers == null) return;

            var filtered = _allUsers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                filtered = filtered.Where(x =>
                    (x.LastName?.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.FirstName?.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Login?.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (RoleFilterBox.SelectedItem is string selectedRole && selectedRole != "Все роли")
            {
                filtered = filtered.Where(x => x.Role == selectedRole);
            }

            DataGrid.ItemsSource = filtered.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.Navigate(new UserEditPage(_mainFrame, null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as User;
            if (selected != null)
            {
                _mainFrame.Navigate(new UserEditPage(_mainFrame, selected));
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGrid.SelectedItem as User;
            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить пользователя '{selected.Login}'?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _apiService.DeleteUserAsync(selected.Id);
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