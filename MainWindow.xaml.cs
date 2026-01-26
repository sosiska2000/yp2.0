using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EquipmentManagement.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Показываем имя текущего пользователя
            CurrentUserText.Text = LoginWindow.CurrentUser;
            WelcomeUserText.Text = $"Вы вошли как: {LoginWindow.CurrentUser}";

            // Можно добавить логику в зависимости от роли пользователя
            SetUserPermissions(LoginWindow.CurrentUser);
        }

        private void SetUserPermissions(string username)
        {
            // Пример: ограничения для разных пользователей
            if (username == "teacher" || username == "user")
            {
                // Для преподавателей и сотрудников скрываем некоторые кнопки
                ReferencesButton.Visibility = Visibility.Collapsed;
                // AddEquipmentButton.IsEnabled = false; // или так
            }
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаём новое окно входа
            var loginWindow = new LoginWindow();

            // Закрываем главное окно
            this.Close();

            // Показываем окно входа
            loginWindow.Show();
        }

        private void EquipmentListButton_Click(object sender, RoutedEventArgs e)
        {
            // Меняем заголовок
            ChangeTitle("Список оборудования");

            // Очищаем контент
            ContentGrid.Children.Clear();

            // Создаём и добавляем список оборудования
            //var equipmentList = new EquipmentListView();
            //ContentGrid.Children.Add(equipmentList);
        }

        private void AddEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTitle("Добавление оборудования");

            ContentGrid.Children.Clear();

            // Создаём форму добавления оборудования
            //var addForm = new EquipmentAddForm();
            //ContentGrid.Children.Add(addForm);
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTitle("Список аудиторий");

            ContentGrid.Children.Clear();

            // Заглушка для аудиторий
            var placeholder = CreatePlaceholder("", "Управление аудиториями",
                "Здесь будет список аудиторий и управление ими");
            ContentGrid.Children.Add(placeholder);
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTitle("Инвентаризация");

            ContentGrid.Children.Clear();

            var placeholder = CreatePlaceholder("", "Проведение инвентаризации",
                "Выберите оборудование для проверки и отметьте его состояние");
            ContentGrid.Children.Add(placeholder);
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTitle("Отчёты");

            ContentGrid.Children.Clear();

            var placeholder = CreatePlaceholder("", "Формирование отчётов",
                "Генерация актов приёма-передачи и других документов");
            ContentGrid.Children.Add(placeholder);
        }

        private void ReferencesButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeTitle("Справочники");

            ContentGrid.Children.Clear();

            var placeholder = CreatePlaceholder("", "Управление справочниками",
                "Направления, статусы, типы оборудования и другие справочные данные");
            ContentGrid.Children.Add(placeholder);
        }

        private void ChangeTitle(string newTitle)
        {
            // Находим TextBlock с заголовком (второй TextBlock в шапке)
            var headerGrid = (Grid)((Border)((DockPanel)Content).Children[0]).Child;
            var titleTextBlock = (TextBlock)((Grid)headerGrid).Children[1];
            titleTextBlock.Text = newTitle;
        }

        private StackPanel CreatePlaceholder(string emoji, string title, string description)
        {
            var panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = emoji,
                FontSize = 80,
                Foreground = Brushes.LightGray,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = description,
                FontSize = 16,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 400
            });

            return panel;
        }
    }
}