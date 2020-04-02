using AddressService.Repo.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AddressService.Repo
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        public virtual DbSet<AddressDetailsEntity> AddressDetails { get; set; }
        public virtual DbSet<PostcodeEntity> PostCode { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressDetailsEntity>(entity =>
            {
                entity.ToTable("AddressDetails", "Address");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasKey(x => x.Id);

                entity.Property(e => e.PostCodeId).HasColumnName("PostCodeId");

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddressLine3)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Locality)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                
                entity.HasOne(d => d.PostCode)
                    .WithMany(p => p.AddressDetails)
                    .HasForeignKey(d => d.PostCodeId)
                    .HasConstraintName("FK_AddressDetails_Address_PostCode");
            });

            modelBuilder.Entity<PostcodeEntity>(entity =>
            {
                entity.ToTable("PostCode", "Address");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasKey(x => x.Id);

                entity.Property(e => e.Postcode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasIndex(u => u.Postcode)
                    .IsUnique();

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime2(0)");
            });
        }
    }
}
