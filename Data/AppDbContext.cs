using Microsoft.EntityFrameworkCore;

namespace Emerus.ETM.Admin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vendor> Vendors { get; set; } = null!;
        public DbSet<ContractorRequest> ContractorRequests { get; set; } = null!;
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<Partner> Partners { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("Partner", schema: "etm");
                entity.HasKey(e => e.PartnerCode);

                entity.Property(e => e.PartnerCode)
                    .HasMaxLength(32)
                    .IsRequired()
                    .HasColumnName("PartnerCode");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(128)
                    .IsRequired()
                    .HasColumnName("DisplayName");

                entity.Property(e => e.Description)
                    .HasMaxLength(512)
                    .HasColumnName("Description");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("IsActive");

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnName("CreatedAt");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasColumnName("UpdatedAt");
            });
        }
    }
}