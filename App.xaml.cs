using System;
using System.Net.Http;
using System.Windows;

namespace EquipmentManagement.Client
{
    public partial class App : Application
    {
        public static HttpClient HttpClient { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем глобальный HttpClient
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001"), // Ваш URL API
                Timeout = TimeSpan.FromSeconds(30)
            };

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            HttpClient?.Dispose();
        }
    }
}