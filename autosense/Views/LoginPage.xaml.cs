using AutoSense.Services;
using AutoSense.ViewModels;

namespace AutoSense.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(new AuthenticationService(), Navigation);
        }
    }
}