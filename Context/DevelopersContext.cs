using Microsoft.EntityFrameworkCore;
using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using EquipmentManagement.Client.Pages.Developers;

namespace EquipmentManagement.Client.Context
{
    public class DevelopersContext : DbContext
    {
        public DbSet<Developers> Developers { get; set; }
        public DevelopersContext()
        {
            Database.EnsureCreated();
            Developers.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
