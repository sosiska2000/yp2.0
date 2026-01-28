using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Connect
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AktPriemaPeredachi> AktyPriemaPeredachi { get; set; }
        public DbSet<AktOborudovanie> AktyOborudovanie { get; set; }
        public DbSet<AktMaterialy> AktyMaterialy { get; set; }
        public DbSet<Polzovatel> Polzovateli { get; set; }
        public DbSet<Napravlenie> Napravleniya { get; set; }
        public DbSet<Status> Statusy { get; set; }
        public DbSet<TipOborudovania> TipyOborudovania { get; set; }
        public DbSet<VidModeli> VidyModelei { get; set; }
        public DbSet<Auditoria> Auditorii { get; set; }
        public DbSet<Razrabotchik> Razrabotchiki { get; set; }
        public DbSet<Programma> Programmy { get; set; }
        public DbSet<Oborudovanie> Oborudovanie { get; set; }
        public DbSet<SetevyeNastroiki> SetevyeNastroiki { get; set; }
        public DbSet<TipRaskhodnogoMateriala> TipyRaskhodnykhMaterialov { get; set; }
        public DbSet<RaskhodnyMaterial> RaskhodnyeMaterialy { get; set; }
        public DbSet<KharakteristikaMateriala> KharakteristikiMaterialov { get; set; }
        public DbSet<Inventarizatsia> Inventarizatsii { get; set; }
        public DbSet<InventarizatsiaDetal> InventarizatsiaDetali { get; set; }
        public DbSet<LogOshibki> LogiOshibok { get; set; }

        public DbSet<OborudovanieProgramma> OborudovanieProgrammy { get; set; }
        public DbSet<OborudovanieRaskhodnyMaterial> OborudovanieRaskhodnyeMaterialy { get; set; }

        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseMySql(
                "server=127.0.0.1;" +
                "port=3306;" +
                "database=inventory_system;" +
                "uid=root;" +
                "pwd=;" +
                "charset=utf8;" +
                "Convert Zero Datetime=True",
                new MySqlServerVersion(new Version(8, 0, 0))
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<OborudovanieProgramma>()
                .HasKey(op => new { op.OborudovanieId, op.ProgrammaId });

            modelBuilder.Entity<OborudovanieRaskhodnyMaterial>()
                .HasKey(orm => new { orm.OborudovanieId, orm.RaskhodnyMaterialId });
        }
    }
}