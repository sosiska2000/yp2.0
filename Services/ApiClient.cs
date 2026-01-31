// ApiClient.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5001"; // или ваш URL API

        public ApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Метод для получения списка оборудования
        public async Task<List<RestAPI.Models.Oborudovanie>> GetOborudovanieAsync(string? search = null, string? sort = null)
        {
            try
            {
                string url = "/api/Oborudovanie/list";
                if (!string.IsNullOrEmpty(search) || !string.IsNullOrEmpty(sort))
                {
                    url += "?";
                    if (!string.IsNullOrEmpty(search))
                        url += $"search={Uri.EscapeDataString(search)}";
                    if (!string.IsNullOrEmpty(sort))
                        url += $"&sort={Uri.EscapeDataString(sort)}";
                }

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<RestAPI.Models.Oborudovanie>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<RestAPI.Models.Oborudovanie>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении оборудования: {ex.Message}", ex);
            }
        }

        // Метод для получения оборудования по ID
        public async Task<RestAPI.Models.Oborudovanie> GetOborudovanieByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Oborudovanie/item/{id}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RestAPI.Models.Oborudovanie>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении оборудования с ID {id}: {ex.Message}", ex);
            }
        }

        // Метод для создания оборудования
        public async Task<RestAPI.Models.Oborudovanie> CreateOborudovanieAsync(RestAPI.Models.Oborudovanie equipment)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Oborudovanie/create", equipment);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RestAPI.Models.Oborudovanie>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании оборудования: {ex.Message}", ex);
            }
        }

        // Метод для обновления оборудования
        public async Task<RestAPI.Models.Oborudovanie> UpdateOborudovanieAsync(int id, RestAPI.Models.Oborudovanie equipment)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/Oborudovanie/update/{id}", equipment);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RestAPI.Models.Oborudovanie>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении оборудования с ID {id}: {ex.Message}", ex);
            }
        }

        // Метод для удаления оборудования
        public async Task<bool> DeleteOborudovanieAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Oborudovanie/delete/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении оборудования с ID {id}: {ex.Message}", ex);
            }
        }

        // Дополнительные методы для связанных сущностей
        public async Task<List<RestAPI.Models.Status>> GetStatusyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Statusy/list");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<RestAPI.Models.Status>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<RestAPI.Models.Status>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении статусов: {ex.Message}", ex);
            }
        }

        public async Task<List<RestAPI.Models.TipOborudovania>> GetTipyOborudovaniaAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/TipyOborudovania/list");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<RestAPI.Models.TipOborudovania>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<RestAPI.Models.TipOborudovania>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении типов оборудования: {ex.Message}", ex);
            }
        }

        public async Task<List<RestAPI.Models.Polzovatel>> GetPolzovateliAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Polzovateli/list");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<RestAPI.Models.Polzovatel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new List<RestAPI.Models.Polzovatel>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении пользователей: {ex.Message}", ex);
            }
        }
        // Метод для импорта оборудования через API
        public async Task<ImportResult> ImportEquipmentFromFileAsync(string filePath)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(filePath);
                using var fileContent = new StreamContent(fileStream);

                string fileExtension = Path.GetExtension(filePath).ToLower();
                string contentType = fileExtension switch
                {
                    ".csv" => "text/csv",
                    ".txt" => "text/plain",
                    ".xls" or ".xlsx" => "application/vnd.ms-excel",
                    _ => "application/octet-stream"
                };

                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                var response = await _httpClient.PostAsync("/api/Import/oborudovanie", form);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ImportResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при импорте оборудования: {ex.Message}", ex);
            }
        }

        public class ImportResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public int ImportedCount { get; set; }
            public int SkippedCount { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
            public List<string> Warnings { get; set; } = new List<string>();
        }
    }
}