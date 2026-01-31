using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class NetworkSettingsContext : DbContext
    {
        public DbSet<NetworkSettings> NetworkSettings { get; set; }
        public NetworkSettingsContext()
        {
            Database.EnsureCreated();
            NetworkSettings.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
