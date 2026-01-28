using Microsoft.EntityFrameworkCore;
using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;

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
