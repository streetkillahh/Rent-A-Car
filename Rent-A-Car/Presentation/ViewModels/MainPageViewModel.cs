using System.Collections.ObjectModel;
using System.Windows.Input;
using Rent_A_Car.Domain.Entities;
using Rent_A_Car.Infrastructure.Persistence;
using Rent_A_Car.Presentation.Views.Windows;

namespace Rent_A_Car.Presentation.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly RentACarDbContext _context;

        public MainPageViewModel(RentACarDbContext context)
        {
            _context = context;
            Cars = new ObservableCollection<Car>(_context.Cars.ToList());
            AddCarCommand = new RelayCommand(OpenAddCarWindow);
        }

        public ObservableCollection<Car> Cars { get; set; }

        public ICommand AddCarCommand { get; }

        private void OpenAddCarWindow()
        {
            var window = new AddCarWindow
            {
                DataContext = new AddCarViewModel(_context)
            };
            window.ShowDialog();
        }
    }
}