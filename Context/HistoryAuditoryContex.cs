using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class HistoryAuditoryContex : DbContext
    {
        public DbSet<HistoryAuditory> HistoryAuditory { get; set; }
        public HistoryAuditoryContex()
        {
            Database.EnsureCreated();
            HistoryAuditory.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
