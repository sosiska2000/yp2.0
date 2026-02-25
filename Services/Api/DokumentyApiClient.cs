using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Services.Api
{
    public class DokumentyApiClient : ApiClientBase
    {
        public DokumentyApiClient(string baseUrl) : base(baseUrl) { }

        // Получить все акты
        public async Task<List<object>> GetAkty(string? tipAkta = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var url = "/api/Dokumenty/list";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(tipAkta))
                queryParams.Add($"tipAkta={tipAkta}");
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            return await GetAsync<List<object>>(url);
        }

        // Создать акт приема-передачи оборудования
        public async Task<CreateAktResponse> CreateAktOborudovaniya(CreateAktOborudovaniyaRequest request)
        {
            return await PostAsync<CreateAktOborudovaniyaRequest, CreateAktResponse>("/api/Dokumenty/create-akt-oborudovaniya", request);
        }

        // Получить текст акта для печати
        public async Task<string> GenerateAktText(int id)
        {
            return await GetAsync<string>($"/api/Dokumenty/generate-text/{id}");
        }
    }

    public class CreateAktOborudovaniyaRequest
    {
        public int PoluchatelId { get; set; }
        public int SostavilId { get; set; }
        public List<int> OborudovanieIds { get; set; } = new();
        public string? Kommentarii { get; set; }
        public DateTime? DataVozvrata { get; set; }
    }

    public class CreateAktResponse
    {
        public string message { get; set; } = string.Empty;
        public int aktId { get; set; }
        public string text { get; set; } = string.Empty;
    }
}