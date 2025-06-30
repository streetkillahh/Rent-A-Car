using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Domain.Entities;

namespace Rent_A_Car.Infrastructure.Persistence
{
    public class RentACarDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        public RentACarDbContext(DbContextOptions<RentACarDbContext> options)
            : base(options) { }
    }
}