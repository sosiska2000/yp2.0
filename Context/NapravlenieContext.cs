using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class NapravlenieContext : DbContext
    {
        public DbSet<Napravlenie> Napravlenie { get; set; }
        public NapravlenieContext()
        {
            Database.EnsureCreated();
            Napravlenie.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
