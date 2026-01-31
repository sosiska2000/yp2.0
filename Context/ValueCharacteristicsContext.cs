using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class ValueCharacteristicsContext : DbContext
    {
        public DbSet<ValueCharacteristics> ValueCharacteristics { get; set; }
        public ValueCharacteristicsContext()
        {
            Database.EnsureCreated();
            ValueCharacteristics.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
