using EquipmentManagement.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Services.Api
{
    public class SetevyeNastroikiApiClient : ApiClientBase
    {
        public SetevyeNastroikiApiClient(string baseUrl) : base(baseUrl) { }

        // Получить все сетевые настройки
        public async Task<List<NetworkSettings>> GetSetevyeNastroiki(string? search = null, string? sortBy = null)
        {
            var url = "/api/SetevyeNastroiki/list";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(search))
                queryParams.Add($"search={search}");
            if (!string.IsNullOrEmpty(sortBy))
                queryParams.Add($"sortBy={sortBy}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            return await GetAsync<List<NetworkSettings>>(url);
        }

        // Проверить доступность устройства по сети
        public async Task<NetworkCheckResult> CheckNetworkDevice(string ip)
        {
            return await PostAsync<NetworkCheckResult>("/api/SetevyeNastroiki/check-network", ip);
        }

        // Проверить все устройства
        public async Task<List<NetworkCheckResult>> CheckAllNetworkDevices()
        {
            return await PostAsync<List<NetworkCheckResult>>("/api/SetevyeNastroiki/check-all-network");
        }
    }

    public class NetworkCheckResult
    {
        public string Ip { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public long? ResponseTime { get; set; }
        public DateTime CheckedAt { get; set; }
    }
}   