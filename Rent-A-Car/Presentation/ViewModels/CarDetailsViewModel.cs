using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Rent_A_Car.Presentation.ViewModels
{
    public class CarDetailsViewModel : ViewModelBase
    {
        private readonly RentACarDbContext _context;
        private readonly Car _car;
        private int _rentalHours;
        private string _hourText = DateTime.Now.Hour.ToString("00");
        private string _minuteText = DateTime.Now.Minute.ToString("00");
        private DateTime _selectedDate = DateTime.Now.Date;

        public string CarName => _car?.Name ?? "Неизвестно";
        public Car Car => _car;

        public string HourText
        {
            get => _hourText;
            set
            {
                if (int.TryParse(value, out int hour) && hour >= 0 && hour <= 23)
                {
                    SetProperty(ref _hourText, value.PadLeft(2, '0'));
                }
                else
                {
                    SetProperty(ref _hourText, "00");
                }

                OnPropertyChanged(nameof(SelectedStartTime));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public string MinuteText
        {
            get => _minuteText;
            set
            {
                if (int.TryParse(value, out int minute) && minute >= 0 && minute <= 59)
                {
                    SetProperty(ref _minuteText, value.PadLeft(2, '0'));
                }
                else
                {
                    SetProperty(ref _minuteText, "00");
                }

                OnPropertyChanged(nameof(SelectedStartTime));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                SetProperty(ref _selectedDate, value);
                OnPropertyChanged(nameof(SelectedStartTime));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public DateTime SelectedStartTime
        {
            get
            {
                if (!int.TryParse(HourText, out int hour))
                    hour = 0;

                if (!int.TryParse(MinuteText, out int minute))
                    minute = 0;

                return new DateTime(
                    SelectedDate.Year,
                    SelectedDate.Month,
                    SelectedDate.Day,
                    hour,
                    minute,
                    0
                );
            }
        }

        public int RentalHours
        {
            get => _rentalHours;
            set
            {
                int minHours = Car.MinRentalHours < 1 ? 1 : Car.MinRentalHours;
                int maxHours = Car.MaxRentalHours < 1 ? 99 : Car.MaxRentalHours;

                if (value >= minHours && value <= maxHours)
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

                if (_car.DiscountStepHours == 0 || _car.DiscountPerStep == 0)
                {
                    return _car.PricePerHour * RentalHours;
                }

                int discountSteps = RentalHours / _car.DiscountStepHours;
                int discountPerHour = discountSteps * _car.DiscountPerStep;

                decimal effectiveRate = _car.PricePerHour - discountPerHour;
                if (effectiveRate < 0) effectiveRate = 0;

                return effectiveRate * RentalHours;
            }
        }

        private bool IsCarAvailable()
        {
            _car.CheckAndUpdateAvailability();
            _context.SaveChanges();

            var utcSelectedTime = DateTime.SpecifyKind(SelectedStartTime, DateTimeKind.Local).ToUniversalTime();

            if (!_car.IsAvailable)
                return false;

            if (_car.EndOfRentalTime.HasValue && _car.EndOfRentalTime.Value > utcSelectedTime)
                return false;

            return true;
        }

        public string EndOfRentalTimeDisplay
        {
            get
            {
                if (!_car.IsAvailable && _car.EndOfRentalTime.HasValue)
                {
                    var endOfRental = DateTime.SpecifyKind(_car.EndOfRentalTime.Value, DateTimeKind.Utc).ToLocalTime();
                    return $"Автомобиль будет доступен с {endOfRental:dd.MM.yyyy HH:mm}";
                }
                return string.Empty;
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

            _car.CheckAndUpdateAvailability();
            _context.SaveChanges();

            HourText = DateTime.Now.Hour.ToString("00");
            MinuteText = DateTime.Now.Minute.ToString("00");
            SelectedDate = DateTime.Now.Date;

            int minHours = Car.MinRentalHours < 1 ? 1 : Car.MinRentalHours;
            int maxHours = Car.MaxRentalHours < 1 ? 99 : Car.MaxRentalHours;

            RentalHours = car.IsAvailable
                ? Math.Clamp(minHours, 1, maxHours)
                : 0;

            // Загрузка истории аренд
            LastRentals = new ObservableCollection<Rental>(
                _context.Rentals
                    .Where(r => r.CarId == _car.Id)
                    .OrderByDescending(r => r.StartTime)
                    .Take(5)
                    .AsEnumerable()
                    .Select(r =>
                    {
                        r.StartTime = DateTime.SpecifyKind(r.StartTime, DateTimeKind.Utc).ToLocalTime();
                        return r;
                    })
                    .ToList()
            );

            IncreaseHoursCommand = new RelayCommand(OnIncreaseHours);
            DecreaseHoursCommand = new RelayCommand(OnDecreaseHours);
            SaveRentalCommand = new RelayCommand(OnSaveRental);
        }

        private void OnIncreaseHours()
        {
            int currentMin = Car.MinRentalHours < 1 ? 1 : Car.MinRentalHours;
            int currentMax = Car.MaxRentalHours < 1 ? 99 : Car.MaxRentalHours;

            if (RentalHours < currentMax)
                RentalHours++;
        }

        private void OnDecreaseHours()
        {
            int currentMin = Car.MinRentalHours < 1 ? 1 : Car.MinRentalHours;
            int currentMax = Car.MaxRentalHours < 1 ? 99 : Car.MaxRentalHours;

            if (RentalHours > currentMin)
                RentalHours--;
        }

        private bool IsValidTime()
        {
            if (!int.TryParse(HourText, out int hour) ||
                !int.TryParse(MinuteText, out int minute))
                return false;

            return hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59;
        }

        private void OnSaveRental()
        {
            if (RentalHours <= 0 || _car == null || !IsValidTime())
                return;

            if (!IsCarAvailable())
                return;

            var utcStartTime = DateTime.SpecifyKind(SelectedStartTime, DateTimeKind.Local).ToUniversalTime();

            var rental = new Rental
            {
                CarId = _car.Id,
                StartTime = utcStartTime,
                HoursRented = RentalHours,
                TotalPrice = TotalPrice
            };

            _context.Rentals.Add(rental);
            _car.IsAvailable = false;
            _car.EndOfRentalTime = utcStartTime.AddHours(RentalHours);
            _context.SaveChanges();

            LastRentals.Insert(0, rental);

            OnPropertyChanged(nameof(IsCarAvailable));
            OnPropertyChanged(nameof(EndOfRentalTimeDisplay));
            OnPropertyChangedForCar();

            MessageBox.Show($"Автомобиль {_car.Name} арендован на {RentalHours} часов. Сумма: {TotalPrice:C}");
        }

        private void OnPropertyChangedForCar()
        {
            OnPropertyChanged(nameof(Car));
            OnPropertyChanged(nameof(EndOfRentalTimeDisplay));
            OnPropertyChanged(nameof(IsCarAvailable));
        }
    }
}