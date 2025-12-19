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
        public DbSet<ContractorPerson> ContractorPeople { get; set; } = null!;
        public DbSet<ContractorHardwareRequest> ContractorHardwareRequest { get; set; } = null!;
        public DbSet<ContractorAccessRequest> ContractorAccessRequest { get; set; } = null!;
        public DbSet<ContractorDocument> ContractorDocument { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Partner>(entity =>
            {
                entity.ToTable("Partner", schema: "etm");
                entity.HasKey(e => e.PartnerCode);
                entity.Property(e => e.PartnerCode).HasMaxLength(32).IsRequired().HasColumnName("PartnerCode");
                entity.Property(e => e.DisplayName).HasMaxLength(128).IsRequired().HasColumnName("DisplayName");
                entity.Property(e => e.Description).HasMaxLength(512).HasColumnName("Description");
                entity.Property(e => e.IsActive).IsRequired().HasColumnName("IsActive");
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnName("UpdatedAt");
            });

            // SINGLE definitive one-to-one mapping between ContractorRequest and ContractorPerson
            modelBuilder.Entity<ContractorRequest>(entity =>
            {
                entity.ToTable("ContractorRequest", "etm");
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.PartnerCode).HasMaxLength(32).HasColumnName("PartnerCode");

                entity.HasOne(r => r.ContractorPerson)
                      .WithOne(p => p.ContractorRequest)
                      .HasForeignKey<ContractorPerson>(p => p.RequestId)
                      .HasConstraintName("FK_ContractorPerson_ContractorRequest_RequestId");
                entity.HasOne(r => r.ContractorHardwareRequest)
                      .WithOne(p => p.ContractorRequest)
                      .HasForeignKey<ContractorHardwareRequest>(p => p.RequestId)
                      .HasConstraintName("FK_ContractorHardwareRequest_ContractorRequest_RequestId");
            });

            modelBuilder.Entity<ContractorRequest>()
                .HasOne(r => r.Partner)
                .WithMany()
                .HasForeignKey(r => r.PartnerCode);

            // ContractorPerson minimal mapping; do NOT re-declare the relationship again here
            modelBuilder.Entity<ContractorPerson>(entity =>
            {
                entity.ToTable("ContractorPerson", schema: "etm");
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.RequestId).HasColumnName("RequestId");
                // other properties can be configured as needed...
            });

            modelBuilder.Entity<ContractorHardwareRequest>(entity =>
            {
                entity.ToTable("ContractorHardwareRequest", schema: "etm");
                entity.HasKey(e => e.RequestId);
                entity.Property(e => e.RequestId).HasColumnName("RequestId");
                // other properties can be configured as needed...
            });
        }
    }
}