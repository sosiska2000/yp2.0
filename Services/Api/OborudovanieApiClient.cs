using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Models.DTO;
using EquipmentManagement.Client.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Services.Api
{
    public class OborudovanieApiClient : ApiClientBase
    {
        public OborudovanieApiClient(string baseUrl) : base(baseUrl) { }

        public async Task<List<OborudovanieDto>> GetOborudovanie(string? search = null, string? sort = null)
        {
            var url = "/api/Oborudovanie/list";
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(search))
                queryParams.Add($"search={search}");
            if (!string.IsNullOrEmpty(sort))
                queryParams.Add($"sort={sort}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            return await GetAsync<List<OborudovanieDto>>(url) ?? new List<OborudovanieDto>();
        }

        public async Task<OborudovanieDto?> GetOborudovanieById(int id)
        {
            return await GetAsync<OborudovanieDto>($"/api/Oborudovanie/item/{id}");
        }

        // Используем OborudovanieMapper
        public async Task<OborudovanieDto> CreateOborudovanie(Models.Oborudovanie equipment)
        {
            var request = equipment.ToCreateRequest();  // Из OborudovanieMapper
            return await PostAsync<CreateOborudovanieRequest, OborudovanieDto>("/api/Oborudovanie/create", request);
        }

        public async Task<OborudovanieDto> UpdateOborudovanie(int id, Models.Oborudovanie equipment)
        {
            var request = equipment.ToUpdateRequest();  // Из OborudovanieMapper
            return await PutAsync<UpdateOborudovanieRequest, OborudovanieDto>($"/api/Oborudovanie/update/{id}", request);
        }

        public async Task<string> DeleteOborudovanie(int id)
        {
            return await DeleteAsync<string>($"/api/Oborudovanie/delete/{id}") ?? string.Empty;
        }
    }
}