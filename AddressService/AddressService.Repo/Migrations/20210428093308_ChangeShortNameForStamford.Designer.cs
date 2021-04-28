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
    [Migration("20210428093308_ChangeShortNameForStamford")]
    partial class ChangeShortNameForStamford
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
                            Id = -1,
                            AddressLine1 = "Greetwell Road",
                            AddressLine2 = "Lincoln",
                            AddressLine3 = "",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 53.234482m,
                            Locality = "Lincolnshire",
                            Longitude = -0.51499m,
                            Name = "Lincoln County Hospital",
                            PostCode = "LN2 5QY",
                            ShortName = "Lincoln County Hospital"
                        },
                        new
                        {
                            Id = -2,
                            AddressLine1 = "Sibsey Road",
                            AddressLine2 = "Boston",
                            AddressLine3 = "",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 52.993149m,
                            Locality = "Lincolnshire",
                            Longitude = -0.00684m,
                            Name = "Pilgrim Hospital, Boston",
                            PostCode = "PE21 9QS",
                            ShortName = "Boston (Pilgrim Hospital)"
                        },
                        new
                        {
                            Id = -3,
                            AddressLine1 = "High Holme Rd",
                            AddressLine2 = "Louth",
                            AddressLine3 = "",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 53.371208m,
                            Locality = "Lincolnshire",
                            Longitude = -0.00451m,
                            Name = "Louth Community Hospital",
                            PostCode = "LN11 0EU",
                            ShortName = "Louth"
                        },
                        new
                        {
                            Id = -4,
                            AddressLine1 = "Grantham Meres Leisure Centre Table Tennis Club",
                            AddressLine2 = "Grantham Meres Leisure Centre",
                            AddressLine3 = "Trent Road",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 52.903179m,
                            Locality = "Grantham",
                            Longitude = -0.66045m,
                            Name = "Table Tennis Club, Grantham",
                            PostCode = "NG31 7XQ",
                            ShortName = "Grantham"
                        },
                        new
                        {
                            Id = -5,
                            AddressLine1 = "Cliff Villages Medical Practice",
                            AddressLine2 = "Mere Rd",
                            AddressLine3 = "Waddington",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 53.165936m,
                            Locality = "Lincoln",
                            Longitude = -0.535592m,
                            Name = "Waddington Branch Surgery, South Lincoln",
                            PostCode = "LN5 9NX",
                            ShortName = "Lincoln South (Waddington Branch Surgery)"
                        },
                        new
                        {
                            Id = -6,
                            AddressLine1 = "St. Mary’s Medical Centre",
                            AddressLine2 = "Wharf Road",
                            AddressLine3 = "Stamford",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 52.650925m,
                            Locality = "Lincolnshire",
                            Longitude = -0.477465m,
                            Name = "St. Mary’s Medical Centre, Stamford",
                            PostCode = "PE9 2DH",
                            ShortName = "Stamford (St. Mary’s Medical Centre)"
                        },
                        new
                        {
                            Id = -7,
                            AddressLine1 = "Franklin Hall",
                            AddressLine2 = "Halton Road",
                            AddressLine3 = "Spilsby",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 53.1723m,
                            Locality = "",
                            Longitude = 0.099136m,
                            Name = "Franklin Hall, Spilsby",
                            PostCode = "PE23 5LA",
                            ShortName = "Spilsby"
                        },
                        new
                        {
                            Id = -8,
                            AddressLine1 = "Sidings Medical Practice",
                            AddressLine2 = "14 Sleaford Rd",
                            AddressLine3 = "Boston",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 52.975942m,
                            Locality = "",
                            Longitude = -0.033522m,
                            Name = "Sidings Medical Practice, Boston",
                            PostCode = "PE21 8EG",
                            ShortName = "Boston (Sidings Medical Practice)"
                        },
                        new
                        {
                            Id = -9,
                            AddressLine1 = "Ruston Sports & Social Club",
                            AddressLine2 = "Newark Road",
                            AddressLine3 = "Lincoln",
                            Instructions = "{\"Intro\":null,\"Steps\":[{\"Heading\":\"Information\",\"Detail\":\"Please make sure to arrive 15 minutes before the start of your shift and bring clothing appropriate for the weather on the day as you may be asked to spend some time outside during your shift.\"}],\"Close\":null}",
                            Latitude = 53.196498m,
                            Locality = "",
                            Longitude = -0.574294m,
                            Name = "Ruston Sports and Social Club, Lincoln",
                            PostCode = "LN6 8RN",
                            ShortName = "Lincoln (Ruston Sports and Social Club)"
                        },
                        new
                        {
                            Id = -10,
                            AddressLine1 = "Portland Medical Practice",
                            AddressLine2 = "60 Portland St",
                            AddressLine3 = "Lincoln",
                            Instructions = "{\"Intro\":\"Location intro\",\"Steps\":[{\"Heading\":\"Heading 1\",\"Detail\":\"Detail 1\"},{\"Heading\":\"Heading 2\",\"Detail\":\"Detail 2\"}],\"Close\":\"Location close\"}",
                            Latitude = 53.22372m,
                            Locality = "",
                            Longitude = -0.539074m,
                            Name = "Portland Medical Practice, Lincoln",
                            PostCode = "LN5 7LB",
                            ShortName = "Lincoln (Portland Medical Practice)"
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
