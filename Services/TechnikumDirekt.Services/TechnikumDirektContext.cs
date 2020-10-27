using Microsoft.EntityFrameworkCore;
using TechnikumDirekt.DataAccess.Models;

namespace TechnikumDirekt.DataAccess.Sql
{
    public class TechnikumDirektContext: DbContext
    {
        public TechnikumDirektContext(DbContextOptions<TechnikumDirektContext> options): base(options) {} 
        
        public DbSet<Hop> Hops { get; set; } 
        public DbSet<HopArrival> HopArrivals { get; set; } 
        public DbSet<Parcel> Parcels { get; set; } 
        public DbSet<Recipient> Recipients { get; set; } 
        public DbSet<Transferwarehouse> Transferwarehouses { get; set; } 
        public DbSet<Truck> Trucks { get; set; } 
        public DbSet<Warehouse> Warehouses { get; set; }

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
        }
    }
}