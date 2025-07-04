using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rent_A_Car.Domain.Entities;
using System;

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

        // Конвертер для DateTime в UTC
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        modelBuilder.Entity<Rental>()
            .Property(r => r.StartTime)
            .HasConversion(dateTimeConverter);

        base.OnModelCreating(modelBuilder);
    }
}
