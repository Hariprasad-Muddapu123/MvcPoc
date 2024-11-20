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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .ToTable("Users");
            
            builder.Entity<User>()
                .Property(x => x.PhoneNumber)
            .HasColumnName("Mobile");

           builder.Entity<Bike>()
            .HasOne(b => b.User)          // A bike has one user
            .WithMany(u => u.Bikes)      // A user can have many bikes
            .HasForeignKey(b => b.UserId)  // Foreign key in Bike table
            .OnDelete(DeleteBehavior.Cascade);

        }

    }

}
