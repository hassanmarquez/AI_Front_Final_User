using AutoSense.ViewModels;

namespace AutoSense.Views
{
    public partial class WelcomePage : FlyoutPage
    {
        public WelcomePage(WelcomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OnShowMenuClicked(object sender, EventArgs e)
        {
            IsPresented = !IsPresented;
        }

        private async void OnBluetoothClicked(object sender, EventArgs e)
        {
            IsPresented = false; // Cierra el menú lateral
            await Navigation.PushAsync(new BluetoothPage());
        }

        private async void OnDiagnosticClicked(object sender, EventArgs e)
        {
            IsPresented = false; // Cierra el menú lateral
            await Navigation.PushAsync(new DiagnosticPage());
        }

        private async void OnServicesClicked(object sender, EventArgs e)
        {
            IsPresented = false; // Cierra el menú lateral
            await Navigation.PushAsync(new ServicesPage());
        }

        private async void OnGaucheClicked(object sender, EventArgs e)
        {
            IsPresented = false; // Cierra el menú lateral
            await Navigation.PushAsync(new GauchePage());
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool respuesta = await DisplayAlert("Cerrar Sesión",
                "¿Está seguro que desea cerrar sesión?",
                "Sí", "No");

            if (respuesta)
            {
                // Implementar lógica de cierre de sesión aquí
                await Navigation.PushAsync(new LoginPage());
                Navigation.RemovePage(Navigation.NavigationStack[0]);
            }
        }
    }
}