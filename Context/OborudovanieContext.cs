using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class OborudovanieContext : DbContext
    {
        public DbSet<Oborudovanie> Oborudovanie { get; set; }
        public OborudovanieContext()
        {
            Database.EnsureCreated();
            Oborudovanie.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
