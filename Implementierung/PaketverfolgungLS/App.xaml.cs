using System.Windows;

namespace Paketverfolgung;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            Database.EnsureCreatedAndSeeded();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Datenbank konnte nicht initialisiert werden:\n" + ex.Message,
                "Startfehler",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
