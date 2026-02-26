using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Context
{
    public class EquipmentContext : DbContext
    {
        // Основные таблицы
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<SoftwareDeveloper> SoftwareDevelopers { get; set; }
        public DbSet<Software> Software { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<ConsumableType> ConsumableTypes { get; set; }
        public DbSet<Consumable> Consumables { get; set; }
        public DbSet<ConsumableAttribute> ConsumableAttributes { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<NetworkSetting> NetworkSettings { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        // Таблицы связей
        public DbSet<EquipmentSoftware> EquipmentSoftware { get; set; }
        public DbSet<EquipmentConsumable> EquipmentConsumables { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=EquipmentAccountingDB;uid=root;pwd=;",
                new MySqlServerVersion(new Version(8, 0, 0)));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка составных первичных ключей
            modelBuilder.Entity<EquipmentSoftware>()
                .HasKey(es => new { es.EquipmentId, es.SoftwareId });

            modelBuilder.Entity<EquipmentConsumable>()
                .HasKey(ec => new { ec.EquipmentId, ec.ConsumableId });

            // Настройка уникальности полей
            modelBuilder.Entity<Equipment>()
                .HasIndex(e => e.InventoryNumber)
                .IsUnique();

            modelBuilder.Entity<NetworkSetting>()
                .HasIndex(n => n.IpAddress)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Настройка связей и каскадного удаления
            modelBuilder.Entity<Model>()
                .HasOne<EquipmentType>()
                .WithMany()
                .HasForeignKey(m => m.EquipmentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Software>()
                .HasOne<SoftwareDeveloper>()
                .WithMany()
                .HasForeignKey(s => s.DeveloperId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne<Classroom>()
                .WithMany()
                .HasForeignKey(e => e.ClassroomId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.ResponsibleUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.TempResponsibleUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne<Direction>()
                .WithMany()
                .HasForeignKey(e => e.DirectionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne<Status>()
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                .HasOne<Model>()
                .WithMany()
                .HasForeignKey(e => e.ModelId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Consumable>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.ResponsibleUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Consumable>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.TempResponsibleUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Consumable>()
                .HasOne<ConsumableType>()
                .WithMany()
                .HasForeignKey(c => c.ConsumableTypeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ConsumableAttribute>()
                .HasOne<Consumable>()
                .WithMany()
                .HasForeignKey(ca => ca.ConsumableId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(i => i.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<InventoryItem>()
                .HasOne<Inventory>()
                .WithMany()
                .HasForeignKey(ii => ii.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryItem>()
                .HasOne<Equipment>()
                .WithMany()
                .HasForeignKey(ii => ii.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryItem>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ii => ii.CheckedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NetworkSetting>()
                .HasOne<Equipment>()
                .WithMany()
                .HasForeignKey(n => n.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EquipmentSoftware>()
                .HasOne<Equipment>()
                .WithMany()
                .HasForeignKey(es => es.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EquipmentSoftware>()
                .HasOne<Software>()
                .WithMany()
                .HasForeignKey(es => es.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EquipmentConsumable>()
                .HasOne<Equipment>()
                .WithMany()
                .HasForeignKey(ec => ec.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EquipmentConsumable>()
                .HasOne<Consumable>()
                .WithMany()
                .HasForeignKey(ec => ec.ConsumableId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка таблиц
            modelBuilder.Entity<Classroom>().ToTable("Classrooms");
            modelBuilder.Entity<Direction>().ToTable("Directions");
            modelBuilder.Entity<Status>().ToTable("Statuses");
            modelBuilder.Entity<EquipmentType>().ToTable("EquipmentTypes");
            modelBuilder.Entity<Model>().ToTable("Models");
            modelBuilder.Entity<SoftwareDeveloper>().ToTable("SoftwareDevelopers");
            modelBuilder.Entity<Software>().ToTable("Software");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Equipment>().ToTable("Equipment");
            modelBuilder.Entity<ConsumableType>().ToTable("ConsumableTypes");
            modelBuilder.Entity<Consumable>().ToTable("Consumables");
            modelBuilder.Entity<ConsumableAttribute>().ToTable("ConsumableAttributes");
            modelBuilder.Entity<Inventory>().ToTable("Inventories");
            modelBuilder.Entity<InventoryItem>().ToTable("InventoryItems");
            modelBuilder.Entity<NetworkSetting>().ToTable("NetworkSettings");
            modelBuilder.Entity<ErrorLog>().ToTable("ErrorLogs");
            modelBuilder.Entity<EquipmentSoftware>().ToTable("EquipmentSoftware");
            modelBuilder.Entity<EquipmentConsumable>().ToTable("EquipmentConsumables");
        }
    }
}