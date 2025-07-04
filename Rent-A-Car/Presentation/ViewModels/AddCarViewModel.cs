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
            SelectImageCommand = new RelayCommand(OnSelectImage);
            SaveCommand = new RelayCommand(OnSave);
        }

        private string _name = string.Empty;
        private int _pricePerHour;
        private int _discountStepHours;
        private int _discountPerStep;
        private int _minRentalHours;
        private int _maxRentalHours;
        private string _imagePath = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int PricePerHour
        {
            get => _pricePerHour;
            set => SetProperty(ref _pricePerHour, value);
        }

        public int DiscountStepHours
        {
            get => _discountStepHours;
            set => SetProperty(ref _discountStepHours, value);
        }

        public int DiscountPerStep
        {
            get => _discountPerStep;
            set => SetProperty(ref _discountPerStep, value);
        }

        public int MinRentalHours
        {
            get => _minRentalHours;
            set => SetProperty(ref _minRentalHours, value);
        }

        public int MaxRentalHours
        {
            get => _maxRentalHours;
            set => SetProperty(ref _maxRentalHours, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public ICommand SelectImageCommand { get; }
        private void OnSelectImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.bmp;*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }
        public ICommand SaveCommand { get; }
        
        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("Введите название автомобиля.");
                return;
            }

            if (MinRentalHours < 1 || MaxRentalHours < MinRentalHours)
            {
                MessageBox.Show("Мин. часы должны быть ≥ 1, Макс. - 99.");
                return;
            }

            var car = new Car
            {
                Name = Name,
                PricePerHour = PricePerHour,
                MinRentalHours = MinRentalHours,
                MaxRentalHours = MaxRentalHours,
                DiscountStepHours = DiscountStepHours,
                DiscountPerStep = DiscountPerStep,
                IsAvailable = true,
                ImagePath = ImagePath
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