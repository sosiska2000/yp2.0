using EquipmentManagement.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EquipmentManagement.Client.Services.Api
{
    public class PolzovateliApiClient : ApiClientBase
    {
        public PolzovateliApiClient(string baseUrl) : base(baseUrl) { }

        // Получить всех пользователей
        public async Task<List<Users>> GetPolzovateli()
        {
            return await GetAsync<List<Users>>("/api/Polzovateli/list");
        }

        // Получить пользователя по ID
        public async Task<Users> GetPolzovatelById(int id)
        {
            return await GetAsync<Users>($"/api/Polzovateli/item/{id}");
        }

        // Создать пользователя
        public async Task<Users> CreatePolzovatel(Users user)
        {
            return await PostAsync<Users>("/api/Polzovateli/create", user);
        }

        // Удалить пользователя
        public async Task<string> DeletePolzovatel(int id)
        {
            return await GetAsync<string>($"/api/Polzovateli/delete/{id}");
        }
    }
}   