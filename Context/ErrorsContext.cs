using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;


namespace EquipmentManagement.Client.Context
{
    public class ErrorsContext : DbContext
    {
        public DbSet<Errors> Errors { get; set; }
        public ErrorsContext()
        {
            Database.EnsureCreated();
            Errors.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
