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

namespace EquipmentManagement.Client.Pages.Oborudovanie
{
    /// <summary>
    /// Логика взаимодействия для Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(Item),
                new PropertyMetadata(false, OnIsSelectedChanged));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        private readonly Oborudovanie _mainOborudovanie;
        public Models.Oborudovanie Oborudovanie { get; }
        private readonly AuditoriesContext _auditoriesContext = new();
        private readonly UsersContext _usersContext = new();
        private readonly NapravlenieContext _napravlenieContext = new();
        private readonly StatusContext _statusContext = new();
        private readonly ViewModelContext _viewModelContext = new();
        private readonly Models.Users _currentUser;

        public Item(Models.Oborudovanie oborudovanie, Oborudovanie mainOborudovanie)
        {
            InitializeComponent();
            Oborudovanie = oborudovanie;
            _mainOborudovanie = mainOborudovanie;

            _currentUser = MainWindow.init.CurrentUser;
            if (_currentUser != null && _currentUser.Role == "Администратор")
            {
                button1.Visibility = Visibility.Visible;
                button2.Visibility = Visibility.Visible;
            }

            LoadEquipmentData();
            this.MouseLeftButtonDown += Item_MouseLeftButtonDown;
        }

        private void LoadEquipmentData()
        {
            lb_Name.Content = Oborudovanie.Name;
            lb_invNum.Content = Oborudovanie.InventNumber;
            lb_Audience.Content = _auditoriesContext.Auditories.FirstOrDefault(x => x.Id == Oborudovanie.IdClassroom)?.Name;
            lb_User.Content = _usersContext.Users.FirstOrDefault(x => x.Id == Oborudovanie.IdResponUser)?.FIO;
            lb_tempUser.Content = _usersContext.Users.FirstOrDefault(x => x.Id == Oborudovanie.IdTimeResponUser)?.FIO;
            lb_Price.Content = Oborudovanie.PriceObor;
            lb_Direct.Content = _napravlenieContext.Napravlenie.FirstOrDefault(x => x.Id == Oborudovanie.IdNapravObor)?.Name;
            lb_Status.Content = _statusContext.Status.FirstOrDefault(x => x.Id == Oborudovanie.IdStatusObor)?.Name;
            lb_Model.Content = _viewModelContext.ViewModel.FirstOrDefault(x => x.Id == Oborudovanie.IdModelObor)?.Name;
            lb_Comment.Content = Oborudovanie.Comments;
            DisplayImage(Oborudovanie.Photo);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Item item)
            {
                item.RaiseSelectionChanged();
            }
        }

        public event EventHandler SelectionChanged;

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Item_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
            e.Handled = true;
        }

        private void Click_redact(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.Oborudovanie.Add(_mainOborudovanie, Oborudovanie));
            }
            catch (Exception ex)
            {
                LogError("Ошибка редактирования оборудования", ex).ConfigureAwait(false);
            }
        }

        private async void Click_remove(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show(
                    "При удалении оборудования все связанные данные также будут удалены!",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _mainOborudovanie.RemoveEquipmentAsync(Oborudovanie);
                    if (Parent is Panel panel)
                    {
                        panel.Children.Remove(this);
                    }
                }
                else
                {
                    MessageBox.Show("Действие отменено.");
                }
            }
            catch (Exception ex)
            {
                await LogError("Ошибка удаления оборудования", ex);
            }
        }

        private void Click_history(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.HistoryObor.HistoryObor(Oborudovanie.Id));
            }
            catch (Exception ex)
            {
                LogError("Ошибка открытия истории оборудования", ex).ConfigureAwait(false);
            }
        }

        private void DisplayImage(byte[] imageData)
        {
            try
            {
                if (imageData != null && imageData.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        imgObor.Source = bitmap;
                    }
                }
                else
                {
                    SetDefaultImage();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
                SetDefaultImage();
            }
        }

        private void SetDefaultImage()
        {
            try
            {
                var defaultImage = new BitmapImage();
                defaultImage.BeginInit();
                defaultImage.UriSource = new Uri("pack://application:,,,/Images/NoneImage.png", UriKind.Absolute);
                defaultImage.EndInit();
                defaultImage.Freeze();

                imgObor.Source = defaultImage;
            }
            catch (Exception ex)
            {
                LogError($"Ошибка загрузки стандартного изображения: {ex.Message}", ex).ConfigureAwait(false);
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

        private void Click_history_auditori(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.init.OpenPages(new Pages.HistoryAuditory.HistoryAuditory(Oborudovanie.Id));
            }
            catch (Exception ex)
            {
                LogError("Ошибка открытия истории оборудования", ex).ConfigureAwait(false);
            }
        }
    }
}
