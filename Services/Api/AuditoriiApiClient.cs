using EquipmentManagement.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Services.Api
{
    public class AuditoriiApiClient : ApiClientBase
    {
        public AuditoriiApiClient(string baseUrl) : base(baseUrl) { }

        // Получить аудиторию по ID с пользователями
        public async Task<Auditories> GetAuditoriaById(int id)
        {
            return await GetAsync<Auditories>($"/api/Auditorii/item/{id}");
        }

        // Получить оборудование в аудитории
        public async Task<List<Oborudovanie>> GetEquipmentInAudience(int audienceId)
        {
            return await GetAsync<List<Oborudovanie>>($"/api/Auditorii/equipment/{audienceId}");
        }

        // Получить количество оборудования в аудитории
        public async Task<int> GetEquipmentCount(int audienceId)
        {
            return await GetAsync<int>($"/api/Auditorii/equipment-count/{audienceId}");
        }

        // Создать аудиторию
        public async Task<CreateAudienceResponse> CreateAuditoria(CreateAudienceRequest request)
        {
            return await PostAsync<CreateAudienceRequest, CreateAudienceResponse>("/api/Auditorii/create", request);
        }

        // Обновить аудиторию
        public async Task UpdateAuditoria(int id, UpdateAudienceRequest request)
        {
            await PutAsync($"/api/Auditorii/update/{id}", request);
        }

        // Удалить аудиторию
        public async Task DeleteAuditoria(int id)
        {
            await DeleteAsync($"/api/Auditorii/delete/{id}");
        }
    }

    // DTO классы (можно вынести в отдельную папку Models/DTO)
    public class CreateAudienceRequest
    {
        public string Nazvanie { get; set; } = string.Empty;
        public string? SokrashennoeNazvanie { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }
    }

    public class UpdateAudienceRequest
    {
        public int Id { get; set; }
        public string Nazvanie { get; set; } = string.Empty;
        public string? SokrashennoeNazvanie { get; set; }
        public int? OtvetstvennyiPolzovatelId { get; set; }
        public int? VremennoOtvetstvennyiPolzovatelId { get; set; }
    }

    public class CreateAudienceResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}