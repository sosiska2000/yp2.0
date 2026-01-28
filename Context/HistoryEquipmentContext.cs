using Microsoft.EntityFrameworkCore;
using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;

namespace EquipmentManagement.Client.Context
{
    public class HistoryOborContext : DbContext
    {
        public DbSet<HistoryObor> HistoryObor { get; set; }
        public HistoryOborContext()
        {
            Database.EnsureCreated();
            HistoryObor.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
