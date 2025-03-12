using AutoSense.Platforms;
using Microsoft.Extensions.Logging;
using AutoSense.Views;
using AutoSense.Services.Interfaces;
using AutoSense.Services;
using AutoSense.ViewModels;
using Plugin.LocalNotification;
using AutoSense.Services.Services.Diagnostic;

namespace AutoSense;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Registrar servicios
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<ISpeechToText, SpeechToTextImplementation>();

        // Registrar ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<WelcomeViewModel>();
        builder.Services.AddTransient<SummaryViewModel>();

        // Registrar Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<WelcomePage>();
        builder.Services.AddTransient<SummaryPage>();

        return builder.Build();
	}
}
