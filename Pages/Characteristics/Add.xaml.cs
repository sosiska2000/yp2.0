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

namespace EquipmentManagement.Client.Pages.Characteristics
{
    /// <summary>
    /// Логика взаимодействия для Add.xaml
    /// </summary>
    public partial class Add : Page
    {
        public Characteristics MainCharacteristics;
        public Models.Characteristics characteristics;
        TypeCharacteristicsContext typeCharacteristicsContext = new TypeCharacteristicsContext();

        public Add(Characteristics MainCharacteristics, Models.Characteristics characteristics = null)
        {
            InitializeComponent();
            this.MainCharacteristics = MainCharacteristics;
            this.characteristics = characteristics;
            if (characteristics != null)
            {
                text1.Content = "Изменение характеристики";
                text2.Content = "Изменить";
                tb_Name.Text = characteristics.Name;
                cb_TypeCharac.SelectedItem = typeCharacteristicsContext.TypeCharacteristics.Where(x => x.Id == characteristics.TypeCharacter).FirstOrDefault().Name;
            }
            foreach (var item in typeCharacteristicsContext.TypeCharacteristics)
            {
                cb_TypeCharac.Items.Add(item.Name);
            }
        }

        private void Click_Redact(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tb_Name.Text))
                {
                    MessageBox.Show("Введите наименование характеристики");
                    return;
                }
                if (cb_TypeCharac.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип характеристики");
                    return;
                }
                if (characteristics == null)
                {
                    characteristics = new Models.Characteristics();
                    characteristics.Name = tb_Name.Text;
                    characteristics.TypeCharacter = typeCharacteristicsContext.TypeCharacteristics.Where(x => x.Name == cb_TypeCharac.SelectedItem).First().Id;
                    MainCharacteristics.characteristicsContext.Characteristics.Add(characteristics);
                }
                else
                {
                    characteristics.Name = tb_Name.Text;
                    characteristics.TypeCharacter = typeCharacteristicsContext.TypeCharacteristics.Where(x => x.Name == cb_TypeCharac.SelectedItem).First().Id;
                }
                MainCharacteristics.characteristicsContext.SaveChanges();
                MainWindow.init.OpenPages(new Pages.Characteristics.Characteristics());
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
                MainWindow.init.OpenPages(new Pages.Characteristics.Characteristics());
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
