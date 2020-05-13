using AddressService.Repo.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;

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
            SqlConnection conn = (SqlConnection)Database.GetDbConnection();

            if (conn.DataSource.Contains("database.windows.net"))
            {
                conn.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
            }
        }

        public virtual DbSet<AddressDetailsEntity> AddressDetails { get; set; }
        public virtual DbSet<PostcodeEntity> Postcode { get; set; }
        public virtual DbSet<PreComputedNearestPostcodesEntity> PreComputedNearestPostcodes { get; set; }

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
                entity.ToTable("AddressDetail", "Address");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasKey(x => x.Id);

                entity.Property(e => e.PostcodeId).HasColumnName("PostcodeId");

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

                entity.Property(e => e.LastUpdated)
                    .IsRequired()
                    .HasDefaultValueSql("GetUtcDate()")
                    .HasColumnType("datetime2(0)");


                entity.HasOne(d => d.PostCode)
                    .WithMany(p => p.AddressDetails)
                    .HasForeignKey(d => d.PostcodeId)
                    .HasConstraintName("FK_AddressDetails_Address_Postcode");
            });

            modelBuilder.Entity<PostcodeEntity>(entity =>
            {
                entity.ToTable("Postcode", "Address");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasKey(x => x.Id);

                entity.Property(e => e.Postcode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasColumnType("decimal(9,6)");

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasColumnType("decimal(9,6)");

                // Spacial types not supported in Net Core 2.1
                entity.Ignore(e => e.Coordinates);
                //entity.Property(e => e.Coordinates)
                //    .HasColumnType("geography")
                //    .HasComputedColumnSql("[geography]::Point([Latitude],[Longitude],(4326))) PERSISTED");
                entity.Property(e => e.FriendlyName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasIndex(u => u.Postcode)
                    .HasName("IX_Postcode_Postcode")
                    .IsUnique();


                entity.HasIndex(u => u.Postcode)
                    .HasName("IXF_Postcode_Postcode")
                    .IsUnique()
                    .HasFilter("[IsActive] = 1")
                    .ForSqlServerInclude(nameof(PostcodeEntity.Latitude), nameof(PostcodeEntity.Longitude));

                entity.HasIndex(u => new {u.Latitude, u.Longitude})
                    .ForSqlServerInclude(nameof(PostcodeEntity.Postcode))
                    .HasName("IXF_Postcode_Latitude_Longitude")
                    .HasFilter("[IsActive] = 1");

                entity.Property(e => e.LastUpdated)
                    .IsRequired()
                    .HasDefaultValueSql("GetUtcDate()")
                    .HasColumnType("datetime2(0)");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
            });


            modelBuilder.Entity<PreComputedNearestPostcodesEntity>(entity =>
            {
                entity.ToTable("PreComputedNearestPostcodes", "Address");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Postcode)
                    .IsUnicode(false)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.CompressedNearestPostcodes)
                    .IsRequired();

                entity.HasIndex(u => u.Postcode)
                    .IsUnique();

            });
        }
    }
}
