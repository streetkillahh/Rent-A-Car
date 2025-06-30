using Rent_A_Car.Presentation.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Rent_A_Car.Infrastructure.Persistence;

namespace Rent_A_Car;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var options = new DbContextOptionsBuilder<RentACarDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=rentacar;User Id=postgres;Password=streetadmin;")
            .Options;

        var context = new RentACarDbContext(options);
        context.Database.EnsureCreated();

        var mainWindow = new MainWindow();
        mainWindow.MainFrame.Navigate(new MainPage(context));
        mainWindow.Show();
    }
}
