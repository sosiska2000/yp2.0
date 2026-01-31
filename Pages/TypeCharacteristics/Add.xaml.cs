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

namespace EquipmentManagement.Client.Pages.TypeCharacteristics
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        public TypeCharacteristics MainTypeCharacteristics;
        public Models.TypeCharacteristics typeCharacteristics;

        public Add(TypeCharacteristics MainTypeCharacteristics, Models.TypeCharacteristics typeCharacteristics = null)
        {
            InitializeComponent();
            this.MainTypeCharacteristics = MainTypeCharacteristics;
            this.typeCharacteristics = typeCharacteristics;
            if (typeCharacteristics != null)
            {
                text1.Content = "Изменение типа характеристики";
                text2.Content = "Изменить";
                tb_Name.Text = typeCharacteristics.Name;
            }
        }

        private void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tb_Name.Text))
                {
                    MessageBox.Show("Введите наименование типа характеристики");
                    return;
                }
                if (typeCharacteristics == null)
                {
                    typeCharacteristics = new Models.TypeCharacteristics();
                    typeCharacteristics.Name = tb_Name.Text;
                    MainTypeCharacteristics.typeCharacteristicsContext.TypeCharacteristics.Add(typeCharacteristics);
                }
                else
                {
                    typeCharacteristics.Name = tb_Name.Text;
                }
                MainTypeCharacteristics.typeCharacteristicsContext.SaveChanges();
                MainWindow.init.OpenPages(new Pages.TypeCharacteristics.TypeCharacteristics());
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
                MainWindow.init.OpenPages(new Pages.TypeCharacteristics.TypeCharacteristics());
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
