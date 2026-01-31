using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class TypeCharacteristicsContext : DbContext
    {
        public DbSet<TypeCharacteristics> TypeCharacteristics { get; set; }
        public TypeCharacteristicsContext()
        {
            Database.EnsureCreated();
            TypeCharacteristics.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
