using System;

namespace Rent_A_Car.Domain.Entities;

public class Rental
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndOfRentalTime => StartTime.AddHours(HoursRented);
    public int HoursRented { get; set; }
    public decimal TotalPrice { get; set; }
}