using Rent_A_Car.Domain.Entities;

public class Car
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PricePerHour { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime? EndOfRentalTime { get; set; }
    public int DiscountStepHours { get; set; }
    public int DiscountPerStep { get; set; }
    public int MinRentalHours { get; set; } = 1;
    public int MaxRentalHours { get; set; } = 99;
    public string? ImagePath { get; set; }

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public void CheckAndUpdateAvailability()
    {
        if (EndOfRentalTime.HasValue && EndOfRentalTime.Value <= DateTime.UtcNow)
        {
            IsAvailable = true;
            EndOfRentalTime = null;
        }
    }
}