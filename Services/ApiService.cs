using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EquipmentManagement.Client.Models;

namespace EquipmentManagement.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7263/api"; // ⚠️ ИЗМЕНИТЕ ЭТОТ URL НА ВАШ АДРЕС API

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        // Аудитории
        public async Task<List<Classroom>> GetClassroomsAsync() =>
            await _httpClient.GetFromJsonAsync<List<Classroom>>("Classrooms");

        public async Task<Classroom> GetClassroomAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Classroom>($"Classrooms/{id}");

        public async Task AddClassroomAsync(Classroom classroom) =>
            await _httpClient.PostAsJsonAsync("Classrooms/Add", classroom);

        public async Task UpdateClassroomAsync(Classroom classroom) =>
            await _httpClient.PutAsJsonAsync("Classrooms/Edit", classroom);

        public async Task DeleteClassroomAsync(int id) =>
            await _httpClient.DeleteAsync($"Classrooms/Delete/{id}");

        // Направления
        public async Task<List<Direction>> GetDirectionsAsync() =>
            await _httpClient.GetFromJsonAsync<List<Direction>>("Directions");

        public async Task<Direction> GetDirectionAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Direction>($"Directions/{id}");

        public async Task AddDirectionAsync(Direction direction) =>
            await _httpClient.PostAsJsonAsync("Directions/Add", direction);

        public async Task UpdateDirectionAsync(Direction direction) =>
            await _httpClient.PutAsJsonAsync("Directions/Edit", direction);

        public async Task DeleteDirectionAsync(int id) =>
            await _httpClient.DeleteAsync($"Directions/Delete/{id}");

        // Статусы
        public async Task<List<Status>> GetStatusesAsync() =>
            await _httpClient.GetFromJsonAsync<List<Status>>("Statuses");

        public async Task<Status> GetStatusAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Status>($"Statuses/{id}");

        public async Task AddStatusAsync(Status status) =>
            await _httpClient.PostAsJsonAsync("Statuses/Add", status);

        public async Task UpdateStatusAsync(Status status) =>
            await _httpClient.PutAsJsonAsync("Statuses/Edit", status);

        public async Task DeleteStatusAsync(int id) =>
            await _httpClient.DeleteAsync($"Statuses/Delete/{id}");

        // Типы оборудования
        public async Task<List<EquipmentType>> GetEquipmentTypesAsync() =>
            await _httpClient.GetFromJsonAsync<List<EquipmentType>>("EquipmentTypes");

        public async Task<EquipmentType> GetEquipmentTypeAsync(int id) =>
            await _httpClient.GetFromJsonAsync<EquipmentType>($"EquipmentTypes/{id}");

        public async Task AddEquipmentTypeAsync(EquipmentType type) =>
            await _httpClient.PostAsJsonAsync("EquipmentTypes/Add", type);

        public async Task UpdateEquipmentTypeAsync(EquipmentType type) =>
            await _httpClient.PutAsJsonAsync("EquipmentTypes/Edit", type);

        public async Task DeleteEquipmentTypeAsync(int id) =>
            await _httpClient.DeleteAsync($"EquipmentTypes/Delete/{id}");

        // Модели
        public async Task<List<Model>> GetModelsAsync() =>
            await _httpClient.GetFromJsonAsync<List<Model>>("Models");

        public async Task<Model> GetModelAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Model>($"Models/{id}");

        public async Task<List<Model>> GetModelsByTypeAsync(int typeId) =>
            await _httpClient.GetFromJsonAsync<List<Model>>($"Models/ByType/{typeId}");

        public async Task AddModelAsync(Model model) =>
            await _httpClient.PostAsJsonAsync("Models/Add", model);

        public async Task UpdateModelAsync(Model model) =>
            await _httpClient.PutAsJsonAsync("Models/Edit", model);

        public async Task DeleteModelAsync(int id) =>
            await _httpClient.DeleteAsync($"Models/Delete/{id}");

        // Разработчики ПО
        public async Task<List<SoftwareDeveloper>> GetSoftwareDevelopersAsync() =>
            await _httpClient.GetFromJsonAsync<List<SoftwareDeveloper>>("SoftwareDevelopers");

        public async Task<SoftwareDeveloper> GetSoftwareDeveloperAsync(int id) =>
            await _httpClient.GetFromJsonAsync<SoftwareDeveloper>($"SoftwareDevelopers/{id}");

        public async Task AddSoftwareDeveloperAsync(SoftwareDeveloper developer) =>
            await _httpClient.PostAsJsonAsync("SoftwareDevelopers/Add", developer);

        public async Task UpdateSoftwareDeveloperAsync(SoftwareDeveloper developer) =>
            await _httpClient.PutAsJsonAsync("SoftwareDevelopers/Edit", developer);

        public async Task DeleteSoftwareDeveloperAsync(int id) =>
            await _httpClient.DeleteAsync($"SoftwareDevelopers/Delete/{id}");

        // Программы
        public async Task<List<Software>> GetSoftwareListAsync() =>
            await _httpClient.GetFromJsonAsync<List<Software>>("Software");

        public async Task<Software> GetSoftwareAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Software>($"Software/{id}");

        public async Task<List<Software>> GetSoftwareByDeveloperAsync(int developerId) =>
            await _httpClient.GetFromJsonAsync<List<Software>>($"Software/ByDeveloper/{developerId}");

        public async Task AddSoftwareAsync(Software software) =>
            await _httpClient.PostAsJsonAsync("Software/Add", software);

        public async Task UpdateSoftwareAsync(Software software) =>
            await _httpClient.PutAsJsonAsync("Software/Edit", software);

        public async Task DeleteSoftwareAsync(int id) =>
            await _httpClient.DeleteAsync($"Software/Delete/{id}");

        // Пользователи
        public async Task<List<User>> GetUsersAsync() =>
            await _httpClient.GetFromJsonAsync<List<User>>("Users");

        public async Task<User> GetUserAsync(int id) =>
            await _httpClient.GetFromJsonAsync<User>($"Users/{id}");

        public async Task<List<User>> GetUsersByRoleAsync(string role) =>
            await _httpClient.GetFromJsonAsync<List<User>>($"Users/ByRole/{role}");

        public async Task AddUserAsync(User user) =>
            await _httpClient.PostAsJsonAsync("Users/Add", user);

        public async Task UpdateUserAsync(User user) =>
            await _httpClient.PutAsJsonAsync("Users/Edit", user);

        public async Task DeleteUserAsync(int id) =>
            await _httpClient.DeleteAsync($"Users/Delete/{id}");

        // Оборудование
        public async Task<List<Equipment>> GetEquipmentListAsync() =>
            await _httpClient.GetFromJsonAsync<List<Equipment>>("Equipment");

        public async Task<Equipment> GetEquipmentAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Equipment>($"Equipment/{id}");

        public async Task<List<Equipment>> SearchEquipmentAsync(string name) =>
            await _httpClient.GetFromJsonAsync<List<Equipment>>($"Equipment/Search/{name}");

        public async Task<List<Equipment>> GetEquipmentByClassroomAsync(int classroomId) =>
            await _httpClient.GetFromJsonAsync<List<Equipment>>($"Equipment/ByClassroom/{classroomId}");

        public async Task<List<Equipment>> GetEquipmentByResponsibleAsync(int userId) =>
            await _httpClient.GetFromJsonAsync<List<Equipment>>($"Equipment/ByResponsible/{userId}");

        public async Task AddEquipmentAsync(Equipment equipment) =>
            await _httpClient.PostAsJsonAsync("Equipment/Add", equipment);

        public async Task UpdateEquipmentAsync(Equipment equipment) =>
            await _httpClient.PutAsJsonAsync("Equipment/Edit", equipment);

        public async Task DeleteEquipmentAsync(int id) =>
            await _httpClient.DeleteAsync($"Equipment/Delete/{id}");

        // Типы расходных материалов
        public async Task<List<ConsumableType>> GetConsumableTypesAsync() =>
            await _httpClient.GetFromJsonAsync<List<ConsumableType>>("ConsumableTypes");

        public async Task<ConsumableType> GetConsumableTypeAsync(int id) =>
            await _httpClient.GetFromJsonAsync<ConsumableType>($"ConsumableTypes/{id}");

        public async Task AddConsumableTypeAsync(ConsumableType type) =>
            await _httpClient.PostAsJsonAsync("ConsumableTypes/Add", type);

        public async Task UpdateConsumableTypeAsync(ConsumableType type) =>
            await _httpClient.PutAsJsonAsync("ConsumableTypes/Edit", type);

        public async Task DeleteConsumableTypeAsync(int id) =>
            await _httpClient.DeleteAsync($"ConsumableTypes/Delete/{id}");

        // Расходные материалы
        public async Task<List<Consumable>> GetConsumablesListAsync() =>
            await _httpClient.GetFromJsonAsync<List<Consumable>>("Consumables");

        public async Task<Consumable> GetConsumableAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Consumable>($"Consumables/{id}");

        public async Task<List<Consumable>> GetConsumablesByTypeAsync(int typeId) =>
            await _httpClient.GetFromJsonAsync<List<Consumable>>($"Consumables/ByType/{typeId}");

        public async Task<List<Consumable>> GetConsumablesForEquipmentAsync(int equipmentId) =>
            await _httpClient.GetFromJsonAsync<List<Consumable>>($"Consumables/ByEquipment/{equipmentId}");

        public async Task AddConsumableAsync(Consumable consumable) =>
            await _httpClient.PostAsJsonAsync("Consumables/Add", consumable);

        public async Task UpdateConsumableAsync(Consumable consumable) =>
            await _httpClient.PutAsJsonAsync("Consumables/Edit", consumable);

        public async Task DeleteConsumableAsync(int id) =>
            await _httpClient.DeleteAsync($"Consumables/Delete/{id}");

        // Характеристики расходников
        public async Task<List<ConsumableAttribute>> GetConsumableAttributesAsync(int consumableId) =>
            await _httpClient.GetFromJsonAsync<List<ConsumableAttribute>>($"Consumables/{consumableId}/Attributes");

        public async Task AddConsumableAttributeAsync(ConsumableAttribute attribute) =>
            await _httpClient.PostAsJsonAsync("Consumables/AddAttribute", attribute);

        public async Task UpdateConsumableAttributeAsync(ConsumableAttribute attribute) =>
            await _httpClient.PutAsJsonAsync("Consumables/EditAttribute", attribute);

        public async Task DeleteConsumableAttributeAsync(int id) =>
            await _httpClient.DeleteAsync($"Consumables/DeleteAttribute/{id}");

        // Инвентаризация
        public async Task<List<Inventory>> GetInventoriesListAsync() =>
            await _httpClient.GetFromJsonAsync<List<Inventory>>("Inventory");

        public async Task<Inventory> GetInventoryAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Inventory>($"Inventory/{id}");

        public async Task<List<InventoryItem>> GetInventoryItemsAsync(int inventoryId) =>
            await _httpClient.GetFromJsonAsync<List<InventoryItem>>($"Inventory/{inventoryId}/Items");

        public async Task CreateInventoryAsync(Inventory inventory, List<int> equipmentIds) =>
            await _httpClient.PostAsJsonAsync("Inventory/Create", new { inventory, equipmentIds });

        public async Task AddEquipmentToInventoryAsync(int inventoryId, int equipmentId) =>
            await _httpClient.PostAsJsonAsync("Inventory/AddEquipment", new { inventoryId, equipmentId });

        public async Task CheckInventoryItemAsync(int inventoryId, int equipmentId, int? userId, string comment) =>
            await _httpClient.PostAsJsonAsync("Inventory/Check", new { inventoryId, equipmentId, userId, comment });

        public async Task UpdateInventoryAsync(Inventory inventory) =>
            await _httpClient.PutAsJsonAsync("Inventory/Edit", inventory);

        public async Task DeleteInventoryAsync(int id) =>
            await _httpClient.DeleteAsync($"Inventory/Delete/{id}");

        // Сетевые настройки
        public async Task<List<NetworkSetting>> GetNetworkSettingsListAsync() =>
            await _httpClient.GetFromJsonAsync<List<NetworkSetting>>("NetworkSettings");

        public async Task<NetworkSetting> GetNetworkSettingAsync(int id) =>
            await _httpClient.GetFromJsonAsync<NetworkSetting>($"NetworkSettings/{id}");

        public async Task<List<NetworkSetting>> GetNetworkSettingsByEquipmentAsync(int equipmentId) =>
            await _httpClient.GetFromJsonAsync<List<NetworkSetting>>($"NetworkSettings/ByEquipment/{equipmentId}");

        public async Task<Dictionary<string, bool>> CheckNetworkAsync() =>
            await _httpClient.GetFromJsonAsync<Dictionary<string, bool>>("NetworkSettings/CheckNetwork");

        public async Task AddNetworkSettingAsync(NetworkSetting setting) =>
            await _httpClient.PostAsJsonAsync("NetworkSettings/Add", setting);

        public async Task UpdateNetworkSettingAsync(NetworkSetting setting) =>
            await _httpClient.PutAsJsonAsync("NetworkSettings/Edit", setting);

        public async Task DeleteNetworkSettingAsync(int id) =>
            await _httpClient.DeleteAsync($"NetworkSettings/Delete/{id}");

        // Связи оборудование-ПО
        public async Task<List<EquipmentSoftware>> GetEquipmentSoftwareListAsync() =>
            await _httpClient.GetFromJsonAsync<List<EquipmentSoftware>>("EquipmentSoftware");

        public async Task<List<Software>> GetSoftwareByEquipmentAsync(int equipmentId) =>
            await _httpClient.GetFromJsonAsync<List<Software>>($"EquipmentSoftware/ByEquipment/{equipmentId}");

        public async Task<List<Equipment>> GetEquipmentBySoftwareAsync(int softwareId) =>
            await _httpClient.GetFromJsonAsync<List<Equipment>>($"EquipmentSoftware/BySoftware/{softwareId}");

        public async Task AddEquipmentSoftwareAsync(int equipmentId, int softwareId) =>
            await _httpClient.PostAsJsonAsync("EquipmentSoftware/Add", new { equipmentId, softwareId });

        public async Task DeleteEquipmentSoftwareAsync(int equipmentId, int softwareId) =>
            await _httpClient.DeleteAsync($"EquipmentSoftware/Delete?equipmentId={equipmentId}&softwareId={softwareId}");

        // Связи оборудование-расходники
        public async Task<List<EquipmentConsumable>> GetEquipmentConsumablesListAsync() =>
            await _httpClient.GetFromJsonAsync<List<EquipmentConsumable>>("EquipmentConsumables");

        public async Task<List<Consumable>> GetConsumablesByEquipmentIdAsync(int equipmentId) =>
            await _httpClient.GetFromJsonAsync<List<Consumable>>($"EquipmentConsumables/ByEquipment/{equipmentId}");

        public async Task<List<Equipment>> GetEquipmentByConsumableIdAsync(int consumableId) =>
            await _httpClient.GetFromJsonAsync<List<Equipment>>($"EquipmentConsumables/ByConsumable/{consumableId}");

        public async Task AddEquipmentConsumableAsync(int equipmentId, int consumableId) =>
            await _httpClient.PostAsJsonAsync("EquipmentConsumables/Add", new { equipmentId, consumableId });

        public async Task DeleteEquipmentConsumableAsync(int equipmentId, int consumableId) =>
            await _httpClient.DeleteAsync($"EquipmentConsumables/Delete?equipmentId={equipmentId}&consumableId={consumableId}");

        // Лог ошибок
        public async Task<List<ErrorLog>> GetErrorLogsAsync() =>
            await _httpClient.GetFromJsonAsync<List<ErrorLog>>("ErrorLogs");

        public async Task<ErrorLog> GetErrorLogAsync(int id) =>
            await _httpClient.GetFromJsonAsync<ErrorLog>($"ErrorLogs/{id}");

        public async Task ClearErrorLogsAsync() =>
            await _httpClient.DeleteAsync("ErrorLogs/Clear");
    }
}