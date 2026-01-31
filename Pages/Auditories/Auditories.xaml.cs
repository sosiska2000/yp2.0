using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Path = System.IO.Path;

namespace EquipmentManagement.Client.Pages.Auditories
{
    /// <summary>
    /// Логика взаимодействия для Auditories.xaml
    /// </summary>
    public partial class Auditories : Page
    {
        private readonly AuditoriesContext _auditoriesContext;
        private readonly Models.Users _currentUser;
        private List<Models.Auditories> _allAuditories;

        public AuditoriesContext AuditoriesContext => _auditoriesContext;

        public Auditories()
        {
            InitializeComponent();

            _auditoriesContext = new AuditoriesContext();
            _currentUser = MainWindow.init.CurrentUser;

            if (_currentUser != null && _currentUser.Role == "Администратор")
            {
                addBtn.Visibility = Visibility.Visible;
            }

            LoadAuditoriesAsync().ConfigureAwait(false);
        }

        private async Task LoadAuditoriesAsync()
        {
            try
            {
                _allAuditories = await Task.Run(() => _auditoriesContext.Auditories.ToList());

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var item in _allAuditories)
                    {
                        parent.Children.Add(new Item(item, this));
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при загрузке аудиторий: ", ex);
            }
        }

        public async Task RemoveAuditoryAsync(Models.Auditories auditory)
        {
            try
            {
                _auditoriesContext.Auditories.Remove(auditory);
                await _auditoriesContext.SaveChangesAsync();
                _allAuditories.Remove(auditory);
            }
            catch (Exception ex)
            {
                await LogError("Общая ошибка при удалении аудитории", ex);
                throw;
            }
        }

        private async void KeyDown_Search(object sender, KeyEventArgs e)
        {
            try
            {
                string searchText = search.Text.ToLower();

                var filteredItems = string.IsNullOrWhiteSpace(searchText)
                    ? _allAuditories
                    : _allAuditories.Where(x => x.Name.ToLower().Contains(searchText)).ToList();

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var item in filteredItems)
                    {
                        parent.Children.Add(new Item(item, this));
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при поиске: ", ex);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Menu());
            }
            catch (Exception ex)
            {
                LogError("Ошибка при возврате в меню", ex).ConfigureAwait(false);
            }
        }

        private async void SortUp(object sender, RoutedEventArgs e)
        {
            try
            {
                var sorted = await Task.Run(() => _allAuditories.OrderBy(x => x.Name).ToList());

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var auditory in sorted)
                    {
                        parent.Children.Add(new Item(auditory, this));
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при сортировке: ", ex);
            }
        }

        private async void SortDown(object sender, RoutedEventArgs e)
        {
            try
            {
                var sorted = await Task.Run(() => _allAuditories.OrderByDescending(x => x.Name).ToList());

                await Dispatcher.InvokeAsync(() =>
                {
                    parent.Children.Clear();
                    foreach (var auditory in sorted)
                    {
                        parent.Children.Add(new Item(auditory, this));
                    }
                });
            }
            catch (Exception ex)
            {
                await LogError("Ошибка при сортировке: ", ex);
            }
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Auditories.Add(this, null));
            }
            catch (Exception ex)
            {
                LogError("Ошибка: ", ex).ConfigureAwait(false);
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
