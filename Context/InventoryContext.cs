using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class InventoryContext : DbContext
    {
        public DbSet<Inventory> Inventory { get; set; }
        public InventoryContext()
        {
            Database.EnsureCreated();
            Inventory.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
