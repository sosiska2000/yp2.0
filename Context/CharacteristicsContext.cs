using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class CharacteristicsContext : DbContext
    {
        public DbSet<Characteristics> Characteristics { get; set; }
        public CharacteristicsContext()
        {
            Database.EnsureCreated();
            Characteristics.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
