using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class RasxodMaterialsContext : DbContext
    {
        public DbSet<RasxodMaterials> RasxodMaterials { get; set; }
        public RasxodMaterialsContext()
        {
            Database.EnsureCreated();
            RasxodMaterials.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
