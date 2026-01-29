using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class СharacteristicTypeContext : DbContext
    {
        public DbSet<СharacteristicType> СharacteristicType { get; set; }
        public СharacteristicTypeContext()
        {
            Database.EnsureCreated();
            СharacteristicType.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
