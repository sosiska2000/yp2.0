using System;
using System.Windows;
using System.Windows.Controls;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Services;

namespace EquipmentManagement.Client.Views.Classrooms
{
    public partial class ClassroomEditPage : Page
    {
        private readonly Frame _mainFrame;
        private readonly ApiService _apiService;
        private readonly Classroom _currentClassroom;

        public ClassroomEditPage(Frame mainFrame, Classroom classroom = null)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
            _apiService = new ApiService();
            _currentClassroom = classroom ?? new Classroom();

            if (classroom != null)
            {
                TitleText.Text = "Редактирование аудитории";
                NameBox.Text = classroom.Name;
                ShortNameBox.Text = classroom.ShortName;
                ResponsibleUserIdBox.Text = classroom.ResponsibleUserId?.ToString();
                TempResponsibleUserIdBox.Text = classroom.TempResponsibleUserId?.ToString();
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

            _currentClassroom.Name = NameBox.Text;
            _currentClassroom.ShortName = ShortNameBox.Text;

            if (int.TryParse(ResponsibleUserIdBox.Text, out int responsibleId))
                _currentClassroom.ResponsibleUserId = responsibleId;
            else
                _currentClassroom.ResponsibleUserId = null;

            if (int.TryParse(TempResponsibleUserIdBox.Text, out int tempId))
                _currentClassroom.TempResponsibleUserId = tempId;
            else
                _currentClassroom.TempResponsibleUserId = null;

            try
            {
                if (_currentClassroom.Id == 0)
                    await _apiService.AddClassroomAsync(_currentClassroom);
                else
                    await _apiService.UpdateClassroomAsync(_currentClassroom);

                _mainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}