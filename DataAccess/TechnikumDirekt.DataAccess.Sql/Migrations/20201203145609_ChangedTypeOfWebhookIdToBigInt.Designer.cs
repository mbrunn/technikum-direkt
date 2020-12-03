﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using TechnikumDirekt.DataAccess.Sql;

namespace TechnikumDirekt.DataAccess.Sql.Migrations
{
    [DbContext(typeof(TechnikumDirektContext))]
    [Migration("20201203145609_ChangedTypeOfWebhookIdToBigInt")]
    partial class ChangedTypeOfWebhookIdToBigInt
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Hop", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HopType")
                        .HasColumnType("int");

                    b.Property<Point>("LocationCoordinates")
                        .HasColumnType("geography");

                    b.Property<string>("LocationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentTraveltimeMins")
                        .HasColumnType("int");

                    b.Property<string>("ParentWarehouseCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ProcessingDelayMins")
                        .HasColumnType("int");

                    b.HasKey("Code");

                    b.HasIndex("ParentWarehouseCode");

                    b.ToTable("Hops");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Hop");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.HopArrival", b =>
                {
                    b.Property<string>("ParcelTrackingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("HopCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("HopArrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.HasKey("ParcelTrackingId", "HopCode");

                    b.HasIndex("HopCode");

                    b.ToTable("HopArrivals");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Parcel", b =>
                {
                    b.Property<string>("TrackingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("RecipientId")
                        .HasColumnType("int");

                    b.Property<int?>("SenderId")
                        .HasColumnType("int");

                    b.Property<int?>("State")
                        .HasColumnType("int");

                    b.Property<float?>("Weight")
                        .HasColumnType("real");

                    b.HasKey("TrackingId");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("Parcels");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Recipient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Recipients");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Webhook", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ParcelTrackingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ParcelTrackingId");

                    b.ToTable("Webhooks");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Transferwarehouse", b =>
                {
                    b.HasBaseType("TechnikumDirekt.DataAccess.Models.Hop");

                    b.Property<string>("LogisticsPartner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogisticsPartnerUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Geometry>("RegionGeometry")
                        .HasColumnType("Geometry");

                    b.HasDiscriminator().HasValue("Transferwarehouse");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Truck", b =>
                {
                    b.HasBaseType("TechnikumDirekt.DataAccess.Models.Hop");

                    b.Property<string>("NumberPlate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Geometry>("RegionGeometry")
                        .HasColumnName("Truck_RegionGeometry")
                        .HasColumnType("Geometry");

                    b.HasDiscriminator().HasValue("Truck");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Warehouse", b =>
                {
                    b.HasBaseType("TechnikumDirekt.DataAccess.Models.Hop");

                    b.Property<int?>("Level")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("Warehouse");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Hop", b =>
                {
                    b.HasOne("TechnikumDirekt.DataAccess.Models.Warehouse", "ParentWarehouse")
                        .WithMany("NextHops")
                        .HasForeignKey("ParentWarehouseCode");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.HopArrival", b =>
                {
                    b.HasOne("TechnikumDirekt.DataAccess.Models.Hop", "Hop")
                        .WithMany("HopArrivals")
                        .HasForeignKey("HopCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TechnikumDirekt.DataAccess.Models.Parcel", "Parcel")
                        .WithMany("HopArrivals")
                        .HasForeignKey("ParcelTrackingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Parcel", b =>
                {
                    b.HasOne("TechnikumDirekt.DataAccess.Models.Recipient", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId");

                    b.HasOne("TechnikumDirekt.DataAccess.Models.Recipient", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");
                });

            modelBuilder.Entity("TechnikumDirekt.DataAccess.Models.Webhook", b =>
                {
                    b.HasOne("TechnikumDirekt.DataAccess.Models.Parcel", "Parcel")
                        .WithMany()
                        .HasForeignKey("ParcelTrackingId");
                });
#pragma warning restore 612, 618
        }
    }
}
