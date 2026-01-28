using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class OborAndRasxodContext : DbContext
    {
        public DbSet<OborAndRasxod> OborAndRasxod { get; set; }
        public OborAndRasxodContext()
        {
            Database.EnsureCreated();
            OborAndRasxod.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
