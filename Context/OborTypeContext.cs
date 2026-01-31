using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class OborTypeContext : DbContext
    {
        public DbSet<OborType> OborType { get; set; }
        public OborTypeContext()
        {
            Database.EnsureCreated();
            OborType.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
