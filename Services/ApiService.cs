using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EquipmentManagement.Client.ViewModel.Audiences;

namespace EquipmentManagement.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5001/api"; 

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<AuditoriumViewModel>> GetAudiencesAsync(string? search = null)
        {
            try
            {
                string url = "/Auditorii/list";
                if (!string.IsNullOrEmpty(search))
                {
                    url += $"?search={Uri.EscapeDataString(search)}";
                }

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<AuditoriumViewModel>>(json, options) ?? new List<AuditoriumViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении аудиторий: {ex.Message}");
                return new List<AuditoriumViewModel>();
            }
        }

        public async Task<AuditoriumViewModel?> GetAudienceByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/Auditorii/item/{id}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<AuditoriumViewModel>(json, options);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateAudienceAsync(AuditoriumViewModel audience)
        {
            try
            {
                var json = JsonSerializer.Serialize(new
                {
                    Nazvanie = audience.Name,
                    SokrashennoeNazvanie = audience.ShortName,
                    OtvetstvennyiPolzovatelId = audience.ResponsibleUser?.Id,
                    VremennoOtvetstvennyiPolzovatelId = audience.TempResponsibleUser?.Id
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/Auditorii/create", content);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAudienceAsync(AuditoriumViewModel audience)
        {
            try
            {
                var json = JsonSerializer.Serialize(new
                {
                    Id = audience.Id,
                    Nazvanie = audience.Name,
                    SokrashennoeNazvanie = audience.ShortName,
                    OtvetstvennyiPolzovatelId = audience.ResponsibleUser?.Id,
                    VremennoOtvetstvennyiPolzovatelId = audience.TempResponsibleUser?.Id
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/Auditorii/update/{audience.Id}", content);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAudienceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/Auditorii/delete/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/Polzovateli/list");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<UserViewModel>>(json, options) ?? new List<UserViewModel>();
            }
            catch
            {
                return new List<UserViewModel>();
            }
        }

        public async Task<List<object>> GetEquipmentInAudienceAsync(int audienceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/Auditorii/equipment/{audienceId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<object>>(json, options) ?? new List<object>();
            }
            catch
            {
                return new List<object>();
            }
        }

        public async Task<int> GetEquipmentCountAsync(int audienceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/Auditorii/equipment-count/{audienceId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return int.Parse(content);
            }
            catch
            {
                return 0;
            }
        }
    }
}