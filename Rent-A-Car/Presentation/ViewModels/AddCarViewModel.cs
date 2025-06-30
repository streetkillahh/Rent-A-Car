using System.Windows;
using System.Windows.Input;
using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;

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

        private string _brand = string.Empty;
        private string _model = string.Empty;
        private decimal _pricePerHour;
        private bool _isAvailable;

        public string Brand
        {
            get => _brand;
            set => SetProperty(ref _brand, value);
        }

        public string Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
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
                Brand = Brand,
                Model = Model,
                PricePerHour = PricePerHour,
                IsAvailable = IsAvailable
            };

            _context.Cars.Add(car);
            _context.SaveChanges();

            MessageBox.Show("Автомобиль добавлен!");
            Application.Current.MainWindow.Close();
        }
    }
}