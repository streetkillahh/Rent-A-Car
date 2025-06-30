using System.Collections.ObjectModel;
using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;
using System.Windows;
using Rent_A_Car.Presentation.Views.Windows;
using Rent_A_Car.Presentation.Views;
using System.Windows.Input;

namespace Rent_A_Car.Presentation.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly RentACarDbContext _context;

    public MainPageViewModel(RentACarDbContext context)
    {
        _context = context;
        Cars = new ObservableCollection<Car>(_context.Cars.ToList());
        AddCarCommand = new RelayCommand(OpenAddCarWindow);
        OpenCarDetailsCommand = new RelayCommand(OpenCarDetails);
    }

    public ObservableCollection<Car> Cars { get; set; }

    public ICommand AddCarCommand { get; }
    public ICommand OpenCarDetailsCommand { get; }

    private Car _selectedCar;
    public Car SelectedCar
    {
        get => _selectedCar;
        set
        {
            SetProperty(ref _selectedCar, value);
            if (value != null)
            {
                OpenCarDetails();
            }
        }
    }

    private void OpenAddCarWindow()
    {
        var window = new AddCarWindow
        {
            DataContext = new AddCarViewModel(_context)
        };
        window.ShowDialog();

        RefreshCars();
    }

    public void RefreshCars()
    {
        Cars.Clear();
        foreach (var car in _context.Cars.ToList())
        {
            Cars.Add(car);
        }
    }

    private void OpenCarDetails()
    {
        if (SelectedCar == null) return;

        var detailsPage = new CarDetailsPage
        {
            DataContext = new CarDetailsViewModel(_context, SelectedCar)
        };
        Application.Current.MainWindow.Close();
        Application.Current.MainWindow = new MainWindow();
        ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(detailsPage);
        Application.Current.MainWindow.Show();
    }
}