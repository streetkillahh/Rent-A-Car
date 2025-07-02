using System.Windows;
using System.Windows.Input;
using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;
using Rent_A_Car.Presentation.Views.Windows;

namespace Rent_A_Car.Presentation.ViewModels
{
    public class AddCarViewModel : ViewModelBase
    {
        private readonly RentACarDbContext _context;

        public AddCarViewModel(RentACarDbContext context)
        {
            _context = context;
            SaveCommand = new RelayCommand(OnSave);
        }

        private string _name = string.Empty;
        private decimal _pricePerHour;
        private bool _isAvailable;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public decimal PricePerHour
        {
            get => _pricePerHour;
            set => SetProperty(ref _pricePerHour, value);
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        public ICommand SaveCommand { get; }

        private void OnSave()
        {
            var car = new Car
            {
                Name = Name,
                PricePerHour = PricePerHour,
                IsAvailable = IsAvailable
            };

            _context.Cars.Add(car);
            _context.SaveChanges();

            MessageBox.Show("Автомобиль добавлен!");
            foreach (Window window in Application.Current.Windows)
            {
                if (window is AddCarWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}