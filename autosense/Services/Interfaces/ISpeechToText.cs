using System.Globalization;

namespace AutoSense.Services.Interfaces;

public interface ISpeechToText
{
    Task<bool> RequestPermissions();

    Task<string> Listen(CultureInfo culture,
        IProgress<string> recognitionResult,
        CancellationToken cancellationToken);
}