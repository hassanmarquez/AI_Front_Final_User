namespace AutoSense.Views;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new NavigationPage (new LoginPage());// new AppShell();

        // Ocultar el botón de retroceso en la página principal
        NavigationPage.SetHasBackButton(MainPage, false);
    }
}
