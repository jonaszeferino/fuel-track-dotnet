using Microsoft.EntityFrameworkCore;

namespace MeuProjetoBlazorServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<FuelRecord> FuelRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações específicas para a tabela Vehicles
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicles");
                
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityColumn();
                
                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .IsRequired();
                
                entity.Property(e => e.TankCapacity)
                    .HasColumnName("tank_capacity")
                    .IsRequired();
                
                entity.Property(e => e.Year)
                    .HasColumnName("year")
                    .IsRequired();
                
                entity.Property(e => e.Subtitle)
                    .HasColumnName("subtitle");
                
                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValue(false);
            });

            // Configurações específicas para a tabela FuelRecords
            modelBuilder.Entity<FuelRecord>(entity =>
            {
                entity.ToTable("fuel_records");
                
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityColumn();
                
                entity.Property(e => e.Odometer)
                    .HasColumnName("odometer")
                    .HasColumnType("numeric")
                    .IsRequired();
                
                entity.Property(e => e.FuelAmount)
                    .HasColumnName("fuel_amount")
                    .HasColumnType("numeric")
                    .IsRequired();
                
                entity.Property(e => e.FuelPricePerUnit)
                    .HasColumnName("fuel_price_per_unit")
                    .HasColumnType("numeric")
                    .IsRequired();
                
                entity.Property(e => e.TotalCost)
                    .HasColumnName("total_cost")
                    .HasColumnType("numeric")
                    .IsRequired();
                
                entity.Property(e => e.FuelType)
                    .HasColumnName("fuel_type")
                    .HasMaxLength(20)
                    .IsRequired();
                
                entity.Property(e => e.FullTank)
                    .HasColumnName("full_tank")
                    .IsRequired();
                
                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasMaxLength(500);
                
                entity.Property(e => e.VehicleId)
                    .HasColumnName("vehicle_id")
                    .IsRequired();
                
                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasDefaultValue(false);
                
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relacionamento com Vehicle
                entity.HasOne(e => e.Vehicle)
                    .WithMany()
                    .HasForeignKey(e => e.VehicleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
