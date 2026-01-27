using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Connect
{
    public class ApplicationDbContext : DbContext
    {
     
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


        public ApplicationDbContext()
        {
            Database.EnsureCreated(); 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseMySql(
                "server=localhost;" +    
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


            modelBuilder.Entity<Polzovatel>()
                .HasIndex(p => p.Login)
                .IsUnique();

            modelBuilder.Entity<Oborudovanie>()
                .HasIndex(o => o.InventarnyiNomer)
                .IsUnique();

            modelBuilder.Entity<SetevyeNastroiki>()
                .HasIndex(s => s.IpAdres)
                .IsUnique();


            modelBuilder.Entity<Oborudovanie>()
                .HasIndex(o => o.Nazvanie);

            modelBuilder.Entity<Oborudovanie>()
                .HasMany(o => o.Programmy)
                .WithMany()
                .UsingEntity(j => j.ToTable("oborudovanie_programmy"));


            modelBuilder.Entity<Oborudovanie>()
                .HasMany<OborudovanieRaskhodnyMaterial>()
                .WithOne(orm => orm.Oborudovanie)
                .HasForeignKey(orm => orm.OborudovanieId);

            modelBuilder.Entity<RaskhodnyMaterial>()
                .HasMany<OborudovanieRaskhodnyMaterial>()
                .WithOne(orm => orm.RaskhodnyMaterial)
                .HasForeignKey(orm => orm.RaskhodnyMaterialId);

            modelBuilder.Entity<OborudovanieRaskhodnyMaterial>()
                .HasKey(orm => new { orm.OborudovanieId, orm.RaskhodnyMaterialId });
        }
    }


    public class OborudovanieRaskhodnyMaterial
    {
        public int OborudovanieId { get; set; }
        public Oborudovanie Oborudovanie { get; set; }

        public int RaskhodnyMaterialId { get; set; }
        public RaskhodnyMaterial RaskhodnyMaterial { get; set; }

        public int Kolichestvo { get; set; } = 1;
    }
}