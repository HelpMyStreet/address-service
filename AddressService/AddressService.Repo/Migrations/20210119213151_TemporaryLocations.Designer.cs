﻿// <auto-generated />
using System;
using AddressService.Repo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AddressService.Repo.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210119213151_TemporaryLocations")]
    partial class TemporaryLocations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.AddressDetailsEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AddressLine1")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine2")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine3")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<DateTime>("LastUpdated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<string>("Locality")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<int>("PostcodeId")
                        .HasColumnName("PostcodeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostcodeId");

                    b.ToTable("AddressDetail","Address");
                });

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.Location", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("AddressLine1")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine2")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine3")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("Instructions")
                        .HasColumnType("varchar(max)")
                        .IsUnicode(false);

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9, 6)");

                    b.Property<string>("Locality")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9, 6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200)
                        .IsUnicode(false);

                    b.Property<string>("PostCode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Location","Address");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AddressLine1 = "Age UK Lincoln & South Lincolnshire",
                            AddressLine2 = "36 Park Street",
                            AddressLine3 = "",
                            Instructions = "{\"Intro\":\"Location 1 intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location 1 close\"}",
                            Latitude = 53.230492m,
                            Locality = "Lincoln",
                            Longitude = -0.54142m,
                            Name = "Location 1",
                            PostCode = "LN1 1UQ",
                            ShortName = "Short Location 1"
                        },
                        new
                        {
                            Id = 2,
                            AddressLine1 = "Location 2 Address Line 1",
                            AddressLine2 = "Location 2 Address Line 2",
                            AddressLine3 = "Location 2 Address Line 3",
                            Instructions = "{\"Intro\":\"Location 2 intro\",\"Steps\":[{\"Heading\":\"Heading 3\",\"Detail\":\"Detail 3\"},{\"Heading\":\"Heading 4\",\"Detail\":\"Detail 4\"}],\"Close\":\"Location 2 close\"}",
                            Latitude = 53.231289m,
                            Locality = "Lincoln",
                            Longitude = -0.54217m,
                            Name = "Location 2",
                            PostCode = "LN1 1DD",
                            ShortName = "Short Location 2"
                        });
                });

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.PostcodeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FriendlyName")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("LastUpdated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Postcode")
                        .IsUnique()
                        .HasName("IXF_Postcode_Postcode")
                        .HasFilter("[IsActive] = 1")
                        .HasAnnotation("SqlServer:Include", new[] { "Latitude", "Longitude" });

                    b.HasIndex("Latitude", "Longitude")
                        .HasName("IXF_Postcode_Latitude_Longitude")
                        .HasFilter("[IsActive] = 1")
                        .HasAnnotation("SqlServer:Include", new[] { "Postcode" });

                    b.ToTable("Postcode","Address");
                });

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.PreComputedNearestPostcodesEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("CompressedNearestPostcodes")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Postcode")
                        .IsUnique();

                    b.ToTable("PreComputedNearestPostcodes","Address");
                });

            modelBuilder.Entity("HelpMyStreet.PostcodeCoordinates.EF.Entities.PostcodeStagingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Postcode_Staging","Staging");
                });

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.AddressDetailsEntity", b =>
                {
                    b.HasOne("AddressService.Repo.EntityFramework.Entities.PostcodeEntity", "PostCode")
                        .WithMany("AddressDetails")
                        .HasForeignKey("PostcodeId")
                        .HasConstraintName("FK_AddressDetails_Address_Postcode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
