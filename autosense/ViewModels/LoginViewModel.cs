using AutoSense.Services.Interfaces;
using AutoSense.Views;
using System.Windows.Input;

namespace AutoSense.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigation _navigation;

        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _demoTime;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool DemoTime
        {
            get => _demoTime;
            set => SetProperty(ref _demoTime, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand DemoOnCommand { get; }
        public ICommand QuickLoginCommand { get; }

        public LoginViewModel(IAuthenticationService authService, INavigation navigation)
        {
            _authService = authService;
            _navigation = navigation;
            LoginCommand = new Command(async () => await LoginAsync());
            DemoOnCommand = new Command(async () => await TaskDemoOnAsync());
            QuickLoginCommand = new Command(async () => await QuickLoginAsync());
            DemoTime = false;
        }

        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Por favor ingrese usuario y contraseña";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                bool isAuthenticated = await _authService.LoginAsync(Username, Password);

                if (isAuthenticated)
                {
                    await _navigation.PushAsync(new WelcomePage(new WelcomeViewModel(Username)), false);
                    _navigation.RemovePage(_navigation.NavigationStack[0]);
                }
                else
                {
                    ErrorMessage = "Usuario o contraseña incorrectos";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Ocurrió un error durante el inicio de sesión";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private Task TaskDemoOnAsync()
        {
            DemoTime = !DemoTime;
            return Task.CompletedTask;
        }

        private async Task QuickLoginAsync()
        {
            Username = "admin";
            Password = "password";
            await LoginAsync();
        }
    }
}