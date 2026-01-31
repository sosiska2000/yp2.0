using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

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
