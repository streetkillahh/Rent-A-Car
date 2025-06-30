using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Domain.Entities;

namespace Rent_A_Car.Infrastructure.Persistence;

public class RentACarDbContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }

    public RentACarDbContext(DbContextOptions<RentACarDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>()
            .HasMany(c => c.Rentals)
            .WithOne(r => r.Car)
            .HasForeignKey(r => r.CarId);

        base.OnModelCreating(modelBuilder);
    }
}