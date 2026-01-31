using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class HistoryRashodContext : DbContext
    {
        public DbSet<HistoryRashod> HistoryRashod { get; set; }
        public HistoryRashodContext()
        {
            Database.EnsureCreated();
            HistoryRashod.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
