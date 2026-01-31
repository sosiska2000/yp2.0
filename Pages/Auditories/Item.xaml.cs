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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EquipmentManagement.Client.Context;

namespace EquipmentManagement.Client.Pages.Auditories
{
    /// <summary>
    /// Логика взаимодействия для Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        private readonly Auditories _mainAuditories;
        public Models.Auditories Auditory { get; }
        private readonly UsersContext _usersContext = new();
        private readonly Models.Users _currentUser;

        public Item(Models.Auditories auditory, Auditories mainAuditories)
        {
            InitializeComponent();
            Auditory = auditory;
            _mainAuditories = mainAuditories;

            _currentUser = MainWindow.init.CurrentUser;
            if (_currentUser != null && _currentUser.Role == "Администратор")
            {
                buttons.Visibility = Visibility.Visible;
            }

            LoadAuditoriesData();
        }

        private void LoadAuditoriesData()
        {
            try
            {
                lb_Name.Content = Auditory.Name;
                lb_sokrName.Content = Auditory.ShortName;
                lb_User.Content = "Ответственный: " + _usersContext.Users
                    .FirstOrDefault(x => x.Id == Auditory.ResponUser)?.FIO;
                lb_tempUser.Content = "Временно-ответственный: " + _usersContext.Users
                    .FirstOrDefault(x => x.Id == Auditory.TimeResponUser)?.FIO;
            }
            catch (Exception ex)
            {
                LogError("Ошибка загрузки данных аудитории", ex).ConfigureAwait(false);
            }
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Auditories.Add(_mainAuditories, Auditory));
            }
            catch (Exception ex)
            {
                LogError("Ошибка при открытии редактирования аудитории", ex).ConfigureAwait(false);
            }
        }

        private async void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(
                    "При удалении аудитории все связанные данные также будут удалены!",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _mainAuditories.RemoveAuditoryAsync(Auditory);
                    if (Parent is Panel panel)
                    {
                        panel.Children.Remove(this);
                    }
                }
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при удалении аудитории", ex);
            }
        }

        private async Task LogError(string message, Exception ex)
        {
            try
            {
                // Логирование в файл
                string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "log.txt");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));
                await System.IO.File.AppendAllTextAsync(logPath, $"{DateTime.Now}: {message}\n{ex.Message}\n{ex.StackTrace}\n\n");

                // Логирование в базу данных
                await using (var errorsContext = new ErrorsContext())
                {
                    errorsContext.Errors.Add(new Models.Errors { Message = $"{message}: {ex.Message}" });
                    await errorsContext.SaveChangesAsync();
                }
            }
            catch (Exception logEx)
            {
                MessageBox.Show($"Ошибка при логировании: {logEx.Message}", "Ошибка логирования", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
