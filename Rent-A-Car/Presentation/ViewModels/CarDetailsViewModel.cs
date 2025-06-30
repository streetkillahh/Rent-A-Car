using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;
using System.Windows;
using System.Windows.Input;

namespace Rent_A_Car.Presentation.ViewModels;

public class CarDetailsViewModel : ViewModelBase
{
    private readonly RentACarDbContext _context;
    private readonly Car _car;
    private int _rentalHours;

    public Car Car => _car;
    public int RentalHours
    {
        get => _rentalHours;
        set => SetProperty(ref _rentalHours, value);
    }

    public ICommand IncreaseHoursCommand { get; }
    public ICommand DecreaseHoursCommand { get; }
    public ICommand SaveRentalCommand { get; }

    public CarDetailsViewModel(RentACarDbContext context, Car car)
    {
        _context = context;
        _car = car;
        RentalHours = 1;

        IncreaseHoursCommand = new RelayCommand(OnIncreaseHours);
        DecreaseHoursCommand = new RelayCommand(OnDecreaseHours);
        SaveRentalCommand = new RelayCommand(OnSaveRental);
    }

    private void OnIncreaseHours()
    {
        if (RentalHours < 24) RentalHours++;
    }

    private void OnDecreaseHours()
    {
        if (RentalHours > 1) RentalHours--;
    }

    private void OnSaveRental()
    {
        _car.IsAvailable = false;
        _context.SaveChanges();

        MessageBox.Show($"Автомобиль {_car.Brand} {_car.Model} арендован на {RentalHours} часов.");
        Application.Current.MainWindow.Close();
    }
}