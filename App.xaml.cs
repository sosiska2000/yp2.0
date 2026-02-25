using EquipmentManagement.Client.Services;
using EquipmentManagement.Client.ViewModels;
using System.Windows;

namespace EquipmentManagement.Client
{
    public partial class App : Application
    {
            public static ApiService Api { get; private set; }

            protected override void OnStartup(StartupEventArgs e)
            {
                base.OnStartup(e);

                // Укажите правильный URL вашего API
                Api = new ApiService("https://localhost:7000");
            }
        }
    }