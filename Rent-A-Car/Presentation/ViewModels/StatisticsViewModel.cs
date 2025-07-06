using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System;

namespace Rent_A_Car.Presentation.ViewModels
{
    public enum TimeRange
    {
        Всё_время,
        За_день,
        За_неделю,
        За_месяц
    }

    public class StatisticsViewModel : ViewModelBase
    {
        private readonly RentACarDbContext _context;

        public StatisticsViewModel(RentACarDbContext context)
        {
            _context = context;

            AvailableCars = new ObservableCollection<Car>(_context.Cars.ToList());
            SelectedCar = AvailableCars.FirstOrDefault(); // Выбираем первый авто по умолчанию
            SelectedTimeRange = TimeRange.Всё_время;

            LoadStatistics();
        }

        private Car? _selectedCar;
        private TimeRange _selectedTimeRange;
        private int _totalCars;
        private int _availableCarsCount;
        private int _totalHours;
        private decimal _totalRevenue;
        private int _totalRentals;

        public ObservableCollection<Car> AvailableCars { get; }
        public TimeRange[] TimeRanges => Enum.GetValues<TimeRange>();

        public TimeRange SelectedTimeRange
        {
            get => _selectedTimeRange;
            set
            {
                SetProperty(ref _selectedTimeRange, value);
                LoadStatistics();
            }
        }

        public Car? SelectedCar
        {
            get => _selectedCar;
            set
            {
                SetProperty(ref _selectedCar, value);
                LoadStatistics();
            }
        }

        public int TotalCars
        {
            get => _totalCars;
            private set => SetProperty(ref _totalCars, value);
        }

        public int AvailableCarsCount
        {
            get => _availableCarsCount;
            private set => SetProperty(ref _availableCarsCount, value);
        }

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

        public int TotalRentals
        {
            get => _totalRentals;
            private set => SetProperty(ref _totalRentals, value);
        }

        public void LoadStatistics()
        {
            IQueryable<Rental> rentalsQuery = _context.Rentals;
            IQueryable<Car> carsQuery = _context.Cars;

            if (SelectedCar != null)
            {
                rentalsQuery = rentalsQuery.Where(r => r.CarId == SelectedCar.Id);
                carsQuery = carsQuery.Where(c => c.Id == SelectedCar.Id);
            }

            var utcNow = DateTime.UtcNow;

            switch (SelectedTimeRange)
            {
                case TimeRange.За_день:
                    var today = utcNow.Date;
                    rentalsQuery = rentalsQuery.Where(r => r.StartTime >= today && r.StartTime < today.AddDays(1));
                    break;

                case TimeRange.За_неделю:
                    var diff = (int)utcNow.DayOfWeek - 1;
                    if (diff < 0) diff = 6;
                    var startOfWeek = utcNow.Date.AddDays(-diff);
                    rentalsQuery = rentalsQuery.Where(r => r.StartTime >= startOfWeek);
                    break;

                case TimeRange.За_месяц:
                    var startOfMonth = new DateTime(utcNow.Year, utcNow.Month, 1);
                    rentalsQuery = rentalsQuery.Where(r => r.StartTime >= startOfMonth);
                    break;

                case TimeRange.Всё_время:
                default:
                    break;
            }

            TotalCars = carsQuery.Count();
            AvailableCarsCount = carsQuery.Count(c => c.IsAvailable);
            TotalHours = rentalsQuery.Any() ? rentalsQuery.Sum(r => r.HoursRented) : 0;
            TotalRevenue = rentalsQuery.Any() ? rentalsQuery.Sum(r => r.TotalPrice) : 0m;
            TotalRentals = rentalsQuery.Count();
        }
    }
}