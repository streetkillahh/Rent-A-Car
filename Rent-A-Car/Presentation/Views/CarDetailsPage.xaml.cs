using System.Windows.Controls;

namespace Rent_A_Car.Presentation.Views;

public partial class CarDetailsPage : Page
{
    public CarDetailsPage()
    {
        InitializeComponent();
    }

    private void Button_Back(object sender, System.Windows.RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }
}
