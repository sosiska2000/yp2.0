using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquipmentManagement.Client.Context.Database;
using EquipmentManagement.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentManagement.Client.Context
{
    public class HistoryAudiencesContext : DbContext
    {
        public DbSet<HistoryAudiences> HistoryAudiences { get; set; }
        public HistoryAudiencesContext()
        {
            Database.EnsureCreated();
            HistoryAudiences.Load();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Config.connection, Config.version);
        }
    }
}
