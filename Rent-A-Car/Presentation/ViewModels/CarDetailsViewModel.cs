using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Rent_A_Car.Presentation.ViewModels
{
    public class CarDetailsViewModel : ViewModelBase
    {
        private readonly RentACarDbContext _context;
        private readonly Car _car;
        private int _rentalHours;

        public string CarName => _car?.Name ?? "Неизвестно";
        public Car Car => _car;

        public int RentalHours
        {
            get => _rentalHours;
            set
            {
                if (value >= 1 && value <= 99)
                {
                    SetProperty(ref _rentalHours, value);
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public decimal TotalPrice
        {
            get
            {
                if (_car == null || RentalHours < 1)
                    return 0;

                int discountSteps = RentalHours / _car.DiscountStepHours;
                int discountPerHour = discountSteps * _car.DiscountPerStep;

                decimal effectiveRate = _car.PricePerHour - discountPerHour;
                if (effectiveRate < 0) effectiveRate = 0;

                return effectiveRate * RentalHours;
            }
        }

        public ObservableCollection<Rental> LastRentals { get; }

        public ICommand IncreaseHoursCommand { get; }
        public ICommand DecreaseHoursCommand { get; }
        public ICommand SaveRentalCommand { get; }

        public CarDetailsViewModel(RentACarDbContext context, Car car)
        {
            _context = context;
            _car = car;
            RentalHours = 1;

            LastRentals = new ObservableCollection<Rental>(
                _context.Rentals
                    .Where(r => r.CarId == _car.Id)
                    .OrderByDescending(r => r.StartTime)
                    .Take(5)
                    .ToList()
            );

            IncreaseHoursCommand = new RelayCommand(OnIncreaseHours);
            DecreaseHoursCommand = new RelayCommand(OnDecreaseHours);
            SaveRentalCommand = new RelayCommand(OnSaveRental);
        }

        private void OnIncreaseHours()
        {
            if (RentalHours < 99)
                RentalHours++;
        }

        private void OnDecreaseHours()
        {
            if (RentalHours > 1)
                RentalHours--;
        }

        private void OnSaveRental()
        {
            if (RentalHours <= 0 || _car == null)
                return;

            DateTime rentalStartTime = DateTime.UtcNow;

            var rental = new Rental
            {
                CarId = _car.Id,
                StartTime = rentalStartTime,
                HoursRented = RentalHours,
                TotalPrice = TotalPrice
            };

            _context.Rentals.Add(rental);
            _car.IsAvailable = false;
            _context.SaveChanges();

            LastRentals.Insert(0, rental);

            MessageBox.Show($"Автомобиль {_car.Name} арендован на {RentalHours} часов. Сумма: {TotalPrice:C}");
        }
    }
}