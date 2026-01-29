using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class СharacteristicValueContext : DbContext
    {
        public DbSet<СharacteristicValue> СharacteristicValue { get; set; }
        public СharacteristicValueContext()
        {
            Database.EnsureCreated();
            СharacteristicValue.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
