using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using TechnikumDirekt.DataAccess.Interfaces;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class TechnikumDirektContext: DbContext, ITechnikumDirektContext
    {
        public TechnikumDirektContext(DbContextOptions<TechnikumDirektContext> options): base(options) {} 
        
        public DbSet<Hop> Hops { get; set; } 
        public DbSet<HopArrival> HopArrivals { get; set; } 
        public DbSet<Parcel> Parcels { get; set; } 
        public DbSet<Recipient> Recipients { get; set; } 
        public DbSet<Transferwarehouse> Transferwarehouses { get; set; } 
        public DbSet<Truck> Trucks { get; set; } 
        public DbSet<Warehouse> Warehouses { get; set; }
        public new DatabaseFacade Database
        {
            get { return base.Database; }
            set{} 
        }
        public new IModel Model 
        {             
            get { return base.Model; }
            set{} 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // HopArrival pk config
            modelBuilder.Entity<HopArrival>()
                .HasKey(ha => new {ha.ParcelTrackingId, ha.HopCode});
            
            modelBuilder.Entity<HopArrival>()
                .HasOne(ha => ha.Parcel)
                .WithMany(p => p.HopArrivals)
                .HasForeignKey(ha => ha.ParcelTrackingId);

            modelBuilder.Entity<HopArrival>()
                .HasOne(ha => ha.Hop)
                .WithMany(h => h.HopArrivals)
                .HasForeignKey(ha => ha.HopCode);
            
            /*modelBuilder.Entity<Hop>()
                .HasOne(h => h.ParentWarehouse)
                .WithMany(pw => pw.NextHops)
                .HasForeignKey(h => h.ParentCode)
                .OnDelete(DeleteBehavior.Cascade);*/
        }
    }
}