using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class AuditoriesContext : DbContext
    {
        public DbSet<Auditories> Auditories { get; set; }
        public AuditoriesContext()
        {
            Database.EnsureCreated();
            Auditories.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
