using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EquipmentManagement.Client.Context;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Models.DTO;
using EquipmentManagement.Client.Services;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace EquipmentManagement.Client.Pages.Oborudovanie
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        public Oborudovanie MainOborudovanie;
        public Models.Oborudovanie oborudovanie;
        private readonly ApiService _apiService;

        // Контексты для выпадающих списков
        AuditoriesContext auditoriesContext = new();
        UsersContext usersContext = new();
        NapravlenieContext napravlenieContext = new();
        StatusContext statusContext = new();
        ViewModelContext viewModelContext = new();

        private byte[] tempPhoto = null;

        public Add(Oborudovanie MainOborudovanie, Models.Oborudovanie oborudovanie = null)
        {
            InitializeComponent();
            this.MainOborudovanie = MainOborudovanie;
            this.oborudovanie = oborudovanie;
            _apiService = App.Api;

            // Загружаем данные для выпадающих списков
            LoadComboBoxes();

            if (oborudovanie != null)
            {
                text1.Content = "Изменение оборудования";
                text2.Content = "Изменить";

                // Заполняем поля данными выбранного оборудования
                tb_Name.Text = oborudovanie.Name;
                tb_invNum.Text = oborudovanie.InventNumber;
                tb_Price.Text = oborudovanie.PriceObor;
                tb_Comment.Text = oborudovanie.Comments;

                // Устанавливаем выбранные значения в ComboBox
                SetSelectedValues();
            }
        }

        private void LoadComboBoxes()
        {
            // Аудитории
            foreach (var item in auditoriesContext.Auditories)
                tb_Audience.Items.Add(item.Name);

            // Пользователи (для ответственного и временно ответственного)
            foreach (var item in usersContext.Users)
            {
                tb_User.Items.Add(item.FIO);
                tb_tempUser.Items.Add(item.FIO);
            }

            // Направления
            foreach (var item in napravlenieContext.Napravlenie)
                tb_Direction.Items.Add(item.Name);

            // Статусы
            foreach (var item in statusContext.Status)
                tb_Status.Items.Add(item.Name);

            // Модели
            foreach (var item in viewModelContext.ViewModel)
                tb_Model.Items.Add(item.Name);
        }

        private void SetSelectedValues()
        {
            // Аудитория
            var audience = auditoriesContext.Auditories.FirstOrDefault(x => x.Id == oborudovanie.IdClassroom);
            if (audience != null)
                tb_Audience.SelectedItem = audience.Name;

            // Ответственный пользователь
            var user = usersContext.Users.FirstOrDefault(x => x.Id == oborudovanie.IdResponUser);
            if (user != null)
                tb_User.SelectedItem = user.FIO;

            // Временно ответственный пользователь
            var tempUser = usersContext.Users.FirstOrDefault(x => x.Id == oborudovanie.IdTimeResponUser);
            if (tempUser != null)
                tb_tempUser.SelectedItem = tempUser.FIO;

            // Направление
            var direction = napravlenieContext.Napravlenie.FirstOrDefault(x => x.Id == oborudovanie.IdNapravObor);
            if (direction != null)
                tb_Direction.SelectedItem = direction.Name;

            // Статус
            var status = statusContext.Status.FirstOrDefault(x => x.Id == oborudovanie.IdStatusObor);
            if (status != null)
                tb_Status.SelectedItem = status.Name;

            // Модель
            var model = viewModelContext.ViewModel.FirstOrDefault(x => x.Id == oborudovanie.IdModelObor);
            if (model != null)
                tb_Model.SelectedItem = model.Name;
        }

        private async void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация (как была ранее)
                if (string.IsNullOrEmpty(tb_Name.Text))
                {
                    MessageBox.Show("Введите наименование оборудования");
                    return;
                }
                if (string.IsNullOrEmpty(tb_invNum.Text))
                {
                    MessageBox.Show("Введите инвентарный номер оборудования");
                    return;
                }
                if (!Regex.IsMatch(tb_invNum.Text, @"^\d*$"))
                {
                    MessageBox.Show("Инвентарный номер должен содержать только цифры");
                    return;
                }
                if (tb_Audience.SelectedItem == null)
                {
                    MessageBox.Show("Выберите аудиторию");
                    return;
                }
                if (tb_User.SelectedItem == null)
                {
                    MessageBox.Show("Выберите ответственного пользователя");
                    return;
                }
                if (tb_tempUser.SelectedItem == null)
                {
                    MessageBox.Show("Выберите временно-ответственного пользователя");
                    return;
                }
                if (string.IsNullOrEmpty(tb_Price.Text))
                {
                    MessageBox.Show("Введите стоимость оборудования");
                    return;
                }
                if (!Regex.IsMatch(tb_Price.Text, @"^[\d]+([,.]?[\d]{0,2})?$"))
                {
                    MessageBox.Show("Стоимость должна быть числом и может содержать до двух десятичных знаков");
                    return;
                }
                if (tb_Direction.SelectedItem == null)
                {
                    MessageBox.Show("Выберите направление");
                    return;
                }
                if (tb_Status.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус");
                    return;
                }
                if (tb_Model.SelectedItem == null)
                {
                    MessageBox.Show("Выберите модель");
                    return;
                }

                // Сохраняем старые значения для истории
                int oldIdResponUser = oborudovanie?.IdResponUser ?? 0;
                int oldIdClassroom = oborudovanie?.IdClassroom ?? 0;

                if (oborudovanie == null)
                {
                    oborudovanie = new Models.Oborudovanie();
                }

                // Заполняем модель данными
                oborudovanie.Name = tb_Name.Text;
                oborudovanie.InventNumber = tb_invNum.Text;
                oborudovanie.IdClassroom = auditoriesContext.Auditories.First(x => x.Name == tb_Audience.SelectedItem).Id;
                oborudovanie.IdResponUser = usersContext.Users.First(x => x.FIO == tb_User.SelectedItem).Id;
                oborudovanie.IdTimeResponUser = usersContext.Users.First(x => x.FIO == tb_tempUser.SelectedItem).Id;
                oborudovanie.PriceObor = tb_Price.Text;
                oborudovanie.IdNapravObor = napravlenieContext.Napravlenie.First(x => x.Name == tb_Direction.SelectedItem).Id;
                oborudovanie.IdStatusObor = statusContext.Status.First(x => x.Name == tb_Status.SelectedItem).Id;
                oborudovanie.IdModelObor = viewModelContext.ViewModel.First(x => x.Name == tb_Model.SelectedItem).Id;
                oborudovanie.Comments = tb_Comment.Text;

                if (tempPhoto != null)
                {
                    oborudovanie.Photo = tempPhoto;
                }

                // Сохраняем через API
                if (oborudovanie.Id == 0)
                {
                    var created = await _apiService.Oborudovanie.CreateOborudovanie(oborudovanie);
                    oborudovanie.Id = created.Id;
                    MessageBox.Show("Оборудование успешно создано!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await _apiService.Oborudovanie.UpdateOborudovanie(oborudovanie.Id, oborudovanie);
                    MessageBox.Show("Оборудование успешно обновлено!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                MainWindow.init.OpenPages(new Pages.Oborudovanie.Oborudovanie());
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при сохранении", ex);
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Click_Cancel_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Oborudovanie.Oborudovanie());
            }
            catch (Exception ex)
            {
                LogError("Ошибка", ex).ConfigureAwait(false);
            }
        }

        private void OpenPhoto(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog
                {
                    Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
                    Title = "Выберите фотографию оборудования"
                };

                if (ofd.ShowDialog() == true)
                {
                    try
                    {
                        using (var fileStream = File.OpenRead(ofd.FileName))
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            fileStream.CopyTo(memoryStream);
                            tempPhoto = memoryStream.ToArray();
                        }
                        photobut.Content = "Фото выбрано";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки фотографии: \n{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Ошибка", ex).ConfigureAwait(false);
            }
        }

        private async Task LogError(string message, Exception ex)
        {
            Debug.WriteLine($"{message}: {ex.Message}");

            try
            {
                // Сохраняем в базу ошибок
                await using (var errorsContext = new ErrorsContext())
                {
                    errorsContext.Errors.Add(new Models.Errors { Message = ex.Message });
                    await errorsContext.SaveChangesAsync();
                }

                // Сохраняем в файл
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "error.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath) ?? string.Empty);

                await File.AppendAllTextAsync(logPath, $"{DateTime.Now}: {message} - {ex.Message}\n{ex.StackTrace}\n\n");
            }
            catch (Exception logEx)
            {
                Debug.WriteLine($"Ошибка при записи в лог-файл: {logEx.Message}");
            }
        }
    }
}