using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;

namespace Rent_A_Car.Presentation.ViewModels;

public class StatisticsViewModel : ViewModelBase
{
    private readonly RentACarDbContext _context;

    public StatisticsViewModel(RentACarDbContext context)
    {
        _context = context;
        LoadStatistics();
    }

    private int _totalHours;
    private decimal _totalRevenue;
    private int _totalCars;
    private int _availableCars;

    public int TotalHours
    {
        get => _totalHours;
        private set => SetProperty(ref _totalHours, value);
    }

    public decimal TotalRevenue
    {
        get => _totalRevenue;
        private set => SetProperty(ref _totalRevenue, value);
    }

    public int TotalCars
    {
        get => _totalCars;
        private set => SetProperty(ref _totalCars, value);
    }

    public int AvailableCars
    {
        get => _availableCars;
        private set => SetProperty(ref _availableCars, value);
    }

    public void LoadStatistics()
    {
        TotalCars = _context.Cars.Count();
        AvailableCars = _context.Cars.Count(c => c.IsAvailable);
        TotalHours = _context.Rentals.Sum(r => r.HoursRented);
        TotalRevenue = _context.Rentals.Sum(r => r.TotalPrice);
    }
}