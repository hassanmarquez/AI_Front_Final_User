
namespace AutoSense.ViewModels
{
    public class SummaryViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;
        private string _date;
        private string _errorCode;
        private string _problemDescription;
        private string _suggestions;
        private string _labor;
        private string _components;
        private string _repairTime;

        public SummaryViewModel()
        {
            
        }

        public string Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public string ErrorCode
        {
            get => _errorCode;
            set => SetProperty(ref _errorCode, value);
        }

        public string ProblemDescription
        {
            get => _problemDescription;
            set => SetProperty(ref _problemDescription, value);
        }

        public string Suggestions
        {
            get => _suggestions;
            set => SetProperty(ref _suggestions, value);
        }

        public string Labor
        {
            get => _labor;
            set => SetProperty(ref _labor, value);
        }

        public string Components
        {
            get => _components;
            set => SetProperty(ref _components, value);
        }

        public string RepairTime
        {
            get => _repairTime;
            set => SetProperty(ref _repairTime, value);
        }

        public void OnScheduleRepairCommand()
        {
            Console.WriteLine("Scheduling repair...");
        }
    }

}
