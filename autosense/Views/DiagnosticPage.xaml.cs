using AutoSense.Models;
using AutoSense.Platforms;
using AutoSense.Services.Interfaces;
using AutoSense.Services.Models;
using AutoSense.Services.Services.Diagnostic;
using AutoSense.Services.Utils;
using System.Collections.ObjectModel;
using System.Globalization;

namespace AutoSense.Views
{
    public partial class DiagnosticPage : ContentPage
    {
        public ObservableCollection<DiagnosticMessage> GeneratedTexts { get; set; } = new ObservableCollection<DiagnosticMessage>(); 
        private CancellationTokenSource tokenListen = new CancellationTokenSource();
        private CancellationTokenSource tokenSpeak = new CancellationTokenSource();
        private IDiagnosticService diagnosticService;
        private ISpeechToText speechToText;

        public DiagnosticPage()
        {
            InitializeComponent();
            speechToText = new SpeechToTextImplementation();
            diagnosticService = new DiagnosticService(EnumStatus.Start, new List<string>());
            GeneratedTextList.ItemsSource = GeneratedTexts;
        }

        private async void OnSpeechToTextClicked(object sender, EventArgs e)
        {
            ListenSpeakCancel();

            // Add the generated text to the list
            string generatedText = await ActivateSpeechToTextAsync();
            GeneratedTexts.Add(new DiagnosticMessage
            {
                Rol = RoleEnums.user,
                Text = generatedText,
                BackgroundColor = Colors.LightBlue,
                HorizontalOptions = LayoutOptions.Start
            });

            // Add the AI response to the list
            string aiResponse = await CallDiagnosticService();
            GeneratedTexts.Add(new DiagnosticMessage
            {
                Rol = RoleEnums.assistant,
                Text = aiResponse,
                BackgroundColor = Colors.LightGreen,
                HorizontalOptions = LayoutOptions.End
            });

            // Speak the response
            if (aiResponse != null)
            {
                SpeakService(aiResponse);
            }
        }

        private async Task<string> ActivateSpeechToTextAsync()
        {
            string recognitionText = "";

            try
            {
                var isAuthorized = await speechToText.RequestPermissions();
                if (isAuthorized)
                {
                    recognitionText = await speechToText.Listen(CultureInfo.GetCultureInfo("en-us"),
                        new Progress<string>(partialText =>
                        {
                            if (DeviceInfo.Platform == DevicePlatform.Android)
                            {
                                recognitionText = partialText;
                            }
                            else
                            {
                                recognitionText += partialText + " ";
                            }
                        }), tokenListen.Token);
                }
                else
                {
                    await DisplayAlert("Permission Error", "No microphone access", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

            return recognitionText;
        }

        private void ListenSpeakCancel()
        {
            tokenListen?.Cancel();
            tokenListen = new CancellationTokenSource();

            tokenSpeak?.Cancel();
            tokenSpeak = new CancellationTokenSource();
        }

        private Task<string> CallDiagnosticService()
        {
            var messages = DiagnosticMessage.GetListMessages(GeneratedTexts.ToList());
            return diagnosticService.NextIteration(messages);
        }

        private async void SpeakService(string prompt)
        {
            await TextToSpeech.SpeakAsync(prompt,
                new SpeechOptions
                {
                    Locale = await LocaleUtils.GetCurrentLocaleAsync()
                },
                tokenSpeak.Token
            );
        }

        private async void OnSummaryClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SummaryPage());
        }
       

    }

}