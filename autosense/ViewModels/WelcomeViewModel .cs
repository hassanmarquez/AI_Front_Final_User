using System.Windows.Input;

namespace AutoSense.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }
        private string _titleMessage;
        public string TitleMessage 
        { 
            get => _titleMessage; 
            set => SetProperty(ref _titleMessage, value); 
        }


        public ICommand LogoutCommand { get; }

        public WelcomeViewModel(string username)
        {
            WelcomeMessage = $"¡Welcome, {username}!";
            TitleMessage = $"                 {WelcomeMessage}                   ";
            LogoutCommand = new Command(async () => await Application.Current.MainPage.Navigation.PopAsync());
        }
    }
}
