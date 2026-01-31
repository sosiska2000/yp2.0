using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class ViewModelContext : DbContext
    {
        public DbSet<ViewModel> ViewModel { get; set; }
        public ViewModelContext()
        {
            Database.EnsureCreated();
            ViewModel.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
