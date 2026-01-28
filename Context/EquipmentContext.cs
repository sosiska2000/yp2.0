using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class EquipmentContext : DbContext
    {
        public DbSet<Equipment> Equipment { get; set; }
        public EquipmentContext()
        {
            Database.EnsureCreated();
            Equipment.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
