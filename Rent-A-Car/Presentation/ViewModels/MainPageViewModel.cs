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
        Cars = new ObservableCollection<Car>(
        _context.Cars.AsEnumerable().Select(c =>
        {
            c.CheckAndUpdateAvailability();
            return c;
        })
        .ToList()
        );
        AddCarCommand = new RelayCommand(OpenAddCarWindow);
        OpenCarDetailsCommand = new RelayCommand(OpenCarDetails);
        ShowStatisticsCommand = new RelayCommand(ShowStatistics);

        _context.SaveChanges();
    }

    public ObservableCollection<Car> Cars { get; set; }

    public ICommand AddCarCommand { get; }
    public ICommand OpenCarDetailsCommand { get; }
    public ICommand ShowStatisticsCommand { get; }

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

        var car = _context.Cars
            .FirstOrDefault(c => c.Id == SelectedCar.Id);

        if (car == null) return;

        var detailsPage = new CarDetailsPage
        {
            DataContext = new CarDetailsViewModel(_context, car)
        };

        ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(detailsPage);
    }
    private void ShowStatistics()
    {
        var statsPage = new StatisticsPage(_context)
        {
            DataContext = new StatisticsViewModel(_context)
        };
        Application.Current.MainWindow.Close();
        Application.Current.MainWindow = new MainWindow();
        ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(statsPage);
        Application.Current.MainWindow.Show();
    }
}