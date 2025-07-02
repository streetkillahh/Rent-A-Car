namespace Rent_A_Car.Domain.Entities;

public class Car
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PricePerHour { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int DiscountStepHours { get; set; } = 5;
    public int DiscountPerStep { get; set; } = 250;

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}