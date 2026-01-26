using EquipmentManagement.Client.Equipment;
using EquipmentManagement.Client.Inventory;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EquipmentManagement.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            // Показываем приветствие
            ShowWelcomeScreen();
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void EquipmentListButton_Click(object sender, RoutedEventArgs e)
        {
            // ОТКРЫВАЕМ ОКНО СПИСКА ОБОРУДОВАНИЯ
            var equipmentWindow = new EquipmentListWindow();
            equipmentWindow.Show();  // ⚠️ НЕ ShowDialog()!
        }

        private void AddEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно добавления оборудования
            var addWindow = new EquipmentEditWindow();
            addWindow.ShowDialog(); // Это диалоговое окно
        }

        private void RoomsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRoomsList();
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            ShowInventoryScreen();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowReportsScreen();
        }

        private void ReferencesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowReferencesScreen();
        }

        // ========== МЕТОДЫ ДЛЯ ПОКАЗА КОНТЕНТА ==========

        private void ShowWelcomeScreen()
        {
            ChangeTitle("Главная");
            ContentGrid.Children.Clear();

            var welcomePanel = CreateWelcomePanel();
            ContentGrid.Children.Add(welcomePanel);
        }

        private void ShowRoomsList()
        {
            ChangeTitle("Список аудиторий");
            ContentGrid.Children.Clear();

            // Создаём простой список аудиторий
            var roomsPanel = CreateSimpleRoomsPanel();
            ContentGrid.Children.Add(roomsPanel);
        }

        private void ShowInventoryScreen()
        {
            ChangeTitle("Инвентаризация");
            ContentGrid.Children.Clear();

            // Создаём панель для запуска инвентаризации
            var inventoryPanel = CreateInventoryStartPanel();
            ContentGrid.Children.Add(inventoryPanel);
        }

        private void ShowReportsScreen()
        {
            ChangeTitle("Отчёты");
            ContentGrid.Children.Clear();

            var reportsPanel = CreatePlaceholder("📊", "Формирование отчётов",
                "Здесь будут отчёты по оборудованию, аудиториям и инвентаризациям");
            ContentGrid.Children.Add(reportsPanel);
        }

        private void ShowReferencesScreen()
        {
            ChangeTitle("Справочники");
            ContentGrid.Children.Clear();

            var referencesPanel = CreatePlaceholder("📚", "Управление справочниками",
                "Направления, статусы, типы оборудования и другие справочные данные");
            ContentGrid.Children.Add(referencesPanel);
        }

        // ========== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ==========

        private void ChangeTitle(string newTitle)
        {
            // Просто меняем заголовок окна
            this.Title = $"Учёт оборудования - {newTitle}";
        }

        private StackPanel CreateWelcomePanel()
        {
            var panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = "🏢",
                FontSize = 80,
                Foreground = Brushes.LightGray,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Добро пожаловать в систему учёта оборудования!",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Выберите раздел в меню слева для начала работы.",
                FontSize = 16,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 400
            });

            // Создаём и добавляем TextBlock для имени пользователя
            var userTextBlock = new TextBlock
            {
                Name = "WelcomeUserText",
                Text = $"Вы вошли как: {LoginWindow.CurrentUser}",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 96, 172)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 30, 0, 0)
            };

            panel.Children.Add(userTextBlock);

            return panel;
        }

        private StackPanel CreateSimpleRoomsPanel()
        {
            var panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = "🏫",
                FontSize = 80,
                Foreground = Brushes.LightGray,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Список аудиторий",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Здесь будет таблица со всеми аудиториями учебного заведения.",
                FontSize = 16,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 400,
                Margin = new Thickness(0, 0, 0, 20)
            });

            return panel;
        }

        private StackPanel CreateInventoryStartPanel()
        {
            var panel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = "📋",
                FontSize = 80,
                Foreground = Brushes.LightGray,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Инвентаризация оборудования",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new TextBlock
            {
                Text = "Проверьте наличие и состояние оборудования в аудиториях",
                FontSize = 16,
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = 400,
                Margin = new Thickness(0, 0, 0, 30)
            });

            var startButton = new Button
            {
                Content = "Начать новую инвентаризацию",
                Width = 250,
                Height = 45,
                Background = new SolidColorBrush(Color.FromRgb(0, 96, 172)),
                Foreground = Brushes.White,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Cursor = Cursors.Hand,
                Margin = new Thickness(0, 10, 0, 0)
            };

            startButton.Click += (s, args) =>
            {
                StartInventoryProcess();
            };

            panel.Children.Add(startButton);

            return panel;
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

        private void StartInventoryProcess()
        {
            try
            {
                // Шаг 1: Основные данные
                var step1 = new InventoryStartWindow();
                if (step1.ShowDialog() != true)
                    return;

                // Шаг 2: Выбор оборудования
                var step2 = new InventorySelectWindow(
                    step1.InventoryName,
                    step1.StartDate,
                    step1.EndDate,
                    step1.InventoryType);

                if (step2.ShowDialog() != true)
                    return;

                // Шаг 3: Проведение инвентаризации
                var step3 = new InventoryProcessWindow(
                    step1.InventoryName,
                    step2.SelectedEquipmentIds);

                step3.ShowDialog();

                MessageBox.Show("Инвентаризация завершена!", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}