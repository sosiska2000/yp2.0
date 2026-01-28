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
using EquipmentManagement.Client.Context.Database;
using Microsoft.Win32;

namespace EquipmentManagement.Client.Pages.Equipment
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        public Equipment MainEquipment;
        public Models.Equipment equipment;
        AudiencesContext audiencesContext = new();
        UsersContext usersContext = new();
        DirectionContext DirectionContext = new();
        StatusContext statusContext = new();
        ViewModelContext viewModelContext = new();

        private byte[] tempPhoto = null;
        public Models.HistoryEquipment historyEquipment;
        public Models.HistoryAudiences historyAudiences;
        public Add(Equipment MainEquipment, Models.Equipment equipment = null)
        {
            InitializeComponent(); 
            this.MainEquipment = MainEquipment;
            this.equipment = equipment;

            if (equipment != null)
            {
                text1.Content = "Изменение оборудования";
                text2.Content = "Изменить";
                tb_Name.Text = equipment.Name;
                tb_invNum.Text = equipment.InventNumber;
                tb_Audience.SelectedItem = audiencesContext.Audiences.FirstOrDefault(x => x.Id == equipment.IdClassroom)?.Name;
                tb_User.SelectedItem = usersContext.Users.FirstOrDefault(x => x.Id == equipment.IdResponUser)?.FIO;
                tb_tempUser.SelectedItem = usersContext.Users.FirstOrDefault(x => x.Id == equipment.IdTimeResponUser)?.FIO;
                tb_Price.Text = equipment.PriceObor;
                tb_Direction.SelectedItem = DirectionContext.Direction.FirstOrDefault(x => x.Id == equipment.IdNapravObor)?.Name;
                tb_Status.SelectedItem = statusContext.Status.FirstOrDefault(x => x.Id == equipment.IdStatusObor)?.Name;
                tb_Model.SelectedItem = viewModelContext.ViewModel.FirstOrDefault(x => x.Id == equipment.IdModelObor)?.Name;
                tb_Comment.Text = equipment.Comments;

            }

            foreach (var item in audiencesContext.Audiences) tb_Audience.Items.Add(item.Name);
            foreach (var item in usersContext.Users)
            {
                tb_User.Items.Add(item.FIO);
                tb_tempUser.Items.Add(item.FIO);
            }
            foreach (var item in DirectionContext.Direction) tb_Direction.Items.Add(item.Name);
            foreach (var item in statusContext.Status) tb_Status.Items.Add(item.Name);
            foreach (var item in viewModelContext.ViewModel) tb_Model.Items.Add(item.Name);
        }
        private void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
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
                // Валидация инвентарного номера (только цифры)
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
                // Валидация стоимости (только цифры и возможно десятичная точка)
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

                if (equipment == null)
                {
                    equipment = new Models.Equipment();
                }

                int oldIdResponUser = equipment.IdResponUser;
                int oldIdClassroom = equipment.IdClassroom;

                equipment.Name = tb_Name.Text;
                equipment.InventNumber = tb_invNum.Text;
                equipment.IdClassroom = audiencesContext.Audiences.First(x => x.Name == tb_Audience.SelectedItem).Id;
                equipment.IdResponUser = usersContext.Users.First(x => x.FIO == tb_User.SelectedItem).Id;
                equipment.IdTimeResponUser = usersContext.Users.First(x => x.FIO == tb_tempUser.SelectedItem).Id;
                equipment.PriceObor = tb_Price.Text;
                equipment.IdNapravObor = DirectionContext.Direction.First(x => x.Name == tb_Direction.SelectedItem).Id;
                equipment.IdStatusObor = statusContext.Status.First(x => x.Name == tb_Status.SelectedItem).Id;
                equipment.IdModelObor = viewModelContext.ViewModel.First(x => x.Name == tb_Model.SelectedItem).Id;
                equipment.Comments = tb_Comment.Text;

                // Если фотография не была загружена, оставляем старую
                if (tempPhoto != null)
                {
                    equipment.Photo = tempPhoto;
                }

                if (equipment.Id == 0)
                {
                    MainEquipment.EquipmentContext.Equipment.Add(equipment);
                }

                MainEquipment.EquipmentContext.SaveChanges();

                // Проверяем, изменился ли IdTimeResponUser
                if (oldIdResponUser != equipment.IdResponUser)
                {
                    // Создаем запись в истории
                    var historyEquipment = new Models.HistoryEquipment
                    {
                        IdUserr = usersContext.Users.First(x => x.FIO == tb_User.SelectedItem).Id,
                        IdObor = equipment.Id, // Используем Id оборудования, который был сгенерирован при сохранении
                        Date = DateTime.Now,
                        Comment = tb_Comment.Text
                    };

                    // Используем HistoryOborContext для сохранения истории
                    using (var historyContext = new HistoryEquipment())
                    {
                        historyContext.HistoryEquipment.Add(historyEquipment);
                        historyContext.SaveChanges();
                    }
                }

                // Проверяем, изменился ли IdClassroom
                if (oldIdClassroom != equipment.IdClassroom)
                {
                    // Создаем запись в истории
                    var historyAudiences = new Models.HistoryAudiences
                    {
                        IdClassroom = audiencesContext.Audiences.First(x => x.Name == tb_Audience.SelectedItem).Id,
                        IdObor = equipment.Id, // Используем Id оборудования, который был сгенерирован при сохранении
                        Date = DateTime.Now,
                    };

                    // Используем HistoryAuditoryContext для сохранения истории
                    using (var HistoryAudiencesContext = new HistoryAudiencesContext())
                    {
                        HistoryAudiencesContext.HistoryAudiences.Add(historyAudiences);
                        HistoryAudiencesContext.SaveChanges();
                    }
                }

                MainWindow.init.OpenPages(new Pages.Equipment.Equipment());
            }
            catch (Exception ex)
            {
                LogError("Ошибка", ex).ConfigureAwait(false);
            }
        }

        private void Click_Cancel_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Equipment.Equipment());
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
                    Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif"
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
                await using (var errorsContext = new ErrorsContext())
                {
                    errorsContext.Errors.Add(new Models.Errors { Message = ex.Message });
                    await errorsContext.SaveChangesAsync();
                }
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "log.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath) ?? string.Empty);

                await File.AppendAllTextAsync(logPath, $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n\n");
            }
            catch (Exception logEx)
            {
                Debug.WriteLine($"Ошибка при записи в лог-файл: {logEx.Message}");
            }
        }
    }
}
