using AutoSense.ViewModels;

namespace AutoSense.Views
{
    public partial class SummaryPage : ContentPage
    {
        public SummaryPage()
        {
            InitializeComponent();
            BindingContext = new SummaryViewModel();
        }
    }
}

