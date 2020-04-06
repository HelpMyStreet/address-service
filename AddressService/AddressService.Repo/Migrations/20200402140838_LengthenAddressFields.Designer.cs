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
    [Migration("20200402140838_LengthenAddressFields")]
    partial class LengthenAddressFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.AddressDetailsEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AddressLine1")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine2")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine3")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("Locality")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<int>("PostCodeId")
                        .HasColumnName("PostCodeId");

                    b.HasKey("Id");

                    b.HasIndex("PostCodeId");

                    b.ToTable("AddressDetails","Address");
                });

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.PostcodeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2(0)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Postcode")
                        .IsUnique();

                    b.ToTable("PostCode","Address");
                });

            modelBuilder.Entity("AddressService.Repo.EntityFramework.Entities.AddressDetailsEntity", b =>
                {
                    b.HasOne("AddressService.Repo.EntityFramework.Entities.PostcodeEntity", "PostCode")
                        .WithMany("AddressDetails")
                        .HasForeignKey("PostCodeId")
                        .HasConstraintName("FK_AddressDetails_Address_PostCode")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
