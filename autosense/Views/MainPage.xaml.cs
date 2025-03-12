using AutoSense.Pages;
using AutoSense.Platforms;
using AutoSense.Services.Interfaces;
using AutoSense.Services.Services.Assistant;
using Plugin.LocalNotification;
using System.Globalization;

namespace AutoSense.Views;

public partial class MainPage : ContentPage
{
    private ISpeechToText speechToText;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();
    private CancellationTokenSource tokenSpeak = new CancellationTokenSource();
    private IEnumerable<Locale> locales;

    public Command ListenCommand { get; set; }
    public Command ListenCancelCommand { get; set; }
    public string RecognitionText { get; set; }

    public MainPage()
    {
        speechToText = new SpeechToTextImplementation();
        InitializeComponent();
        //Setting speechToText
        //this.speechToText = speechToText;
        ListenCommand = new Command(Listen);
        ListenCancelCommand = new Command(ListenCancel);
        RecognitionText = "Welcome, how can I help you?";

        BindingContext = this;

        //recive notifications
        LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
    }
    
    private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
    {
        string message = e.Request.Title + " " + e.Request.Description;
        if (e.IsDismissed) 
        {
            message = "Don't dismissed this notification " + message;
        }
        SpeakService(e.Request.Title + " " + e.Request.Description);
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        locales = await TextToSpeech.GetLocalesAsync();
        foreach (Locale locale in locales)
        {
            Languages.Items.Add(locale.Name);
        }
        //Languages.SelectedItem = "English (United States)";
        Languages.SelectedItem = "inglés (Estados Unidos)";

        SpeakService("Welcome to AutoSense Assistent");

        OnLocalNotificationRequest();
    }

    private async void OnBluetoothClicked(object sender, EventArgs e)
    {
        var bluetoothPage = new BluetoothConnectPage();
        await Navigation.PushModalAsync(bluetoothPage);
    }

    private async void Listen()
    {
        var isAuthorized = await this.speechToText.RequestPermissions();
        if (isAuthorized)
        {
            try
            {

                RecognitionText = await speechToText.Listen(CultureInfo.GetCultureInfo("en-us"),
                    new Progress<string>(partialText =>
                    {
                        if (DeviceInfo.Platform == DevicePlatform.Android)
                        {
                            RecognitionText = partialText;
                        }
                        else
                        {
                            RecognitionText += partialText + " ";
                        }

                        OnPropertyChanged(nameof(RecognitionText));
                    }), tokenSource.Token);

                CallAIService();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        else
        {
            await DisplayAlert("Permission Error", "No microphone access", "OK");
        }
    }

    private void ListenCancel()
    {
        tokenSource?.Cancel();
        tokenSource = new CancellationTokenSource();

        tokenSpeak?.Cancel();
        tokenSpeak = new CancellationTokenSource();
    }

    private async void OnSpeakClicked(object sender, EventArgs e)
    {
        SpeakService(RecognitionText);

        CallAIService();
    }

    private async void SpeakService(string prompt)
    {
        if (Languages.SelectedIndex > 0)
        {
            await TextToSpeech.SpeakAsync(prompt,
                new SpeechOptions
                {
                    Locale = locales.Single(l => l.Name == Languages.SelectedItem.ToString())
                },
                tokenSpeak.Token
                );
        }
    }

    private async void CallAIService()
    {
        IAssistantService client = new ApiAssistantService();
        var response = await client.GenerateResponseAsync(RecognitionText);
        if (response != null)
        {
            SpeakService(response);
        }
    }

    private async void OnLocalNotificationRequest()
    {
        var requestNotification = new NotificationRequest()
        {
            NotificationId = 1337,
            Title = "Remember take the mateinance",
            Subtitle = "AutoSense",
            Description = "It's tiem to chage the oil",
            BadgeNumber = 42,
            CategoryType = NotificationCategoryType.Reminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(5),
                //NotifyRepeatInterval = TimeSpan.FromDays(1),
                //NotifyAutoCancelTime = DateTime.Now.AddSeconds(60)
            },
            /*Android = new AndroidOptions
            {
                LaunchAppWhenTapped = true,
                VibrationPattern = [100000, 1000, 10000, 5000], 
            },*/
        };
        await LocalNotificationCenter.Current.Show(requestNotification);
    }

}

