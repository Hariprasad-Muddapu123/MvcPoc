using BikeBuddy.Models;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BikeBuddy.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Bike> Bikes { get; set; }  

        public DbSet<City> Cities { get; set; }

        public DbSet<Ride> Rides { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Models.Notification> Notifications {  get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .ToTable("Users");
            
            builder.Entity<User>()
                .Property(x => x.PhoneNumber)
            .HasColumnName("Mobile");

           builder.Entity<Bike>()
            .HasOne(b => b.User)          
            .WithMany(u => u.Bikes)  
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            // Ride-User Relationship
            builder.Entity<Ride>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rides)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Ride-Bike Relationship
            builder.Entity<Ride>()
                .HasOne(r => r.Bike)
                .WithMany(b => b.Rides)
                .HasForeignKey(r => r.BikeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.Ride)
                .WithOne(r => r.Payment)
                .HasForeignKey<Payment>(p => p.RideId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Payment>()
                .HasOne(p => p.Bike)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BikeId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }

}
