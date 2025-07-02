namespace Rent_A_Car.Domain.Entities;

public class Car
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PricePerHour { get; set; }
    public bool IsAvailable { get; set; } = true;

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}