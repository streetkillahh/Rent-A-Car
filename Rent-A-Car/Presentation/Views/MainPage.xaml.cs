using Rent_A_Car.Infrastructure.Persistence;
using Rent_A_Car.Presentation.ViewModels;
using System.Windows.Controls;

namespace Rent_A_Car.Presentation.Views;

public partial class MainPage : Page
{
    public MainPage(RentACarDbContext context)
    {
        InitializeComponent();
        DataContext = new MainPageViewModel(context);
    }
}