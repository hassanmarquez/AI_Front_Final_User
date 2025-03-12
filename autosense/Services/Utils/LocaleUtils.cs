namespace AutoSense.Services.Utils;

public class LocaleUtils
{
    public const string SpeakLanguage = "inglés (Estados Unidos)"; //"English (United States)";
    private static IEnumerable<Locale>? locales;

    public static async Task<IEnumerable<Locale>> GetLocalesAsync()
    {
        if (locales == null)
        {
            locales = await TextToSpeech.GetLocalesAsync();
        }
        return locales;
    }

    public static async Task<Locale?> GetLocaleByNameAsync(string localeName)
    {
        var locales = await GetLocalesAsync();
        var locale = locales.FirstOrDefault(l => l.Name == localeName);
        return locale;
    }

    public static async Task<Locale> GetCurrentLocaleAsync()
    {
        var locale = await GetLocaleByNameAsync(SpeakLanguage);
        if(locale == null)
        {
            locale = (await GetLocalesAsync()).FirstOrDefault(l => l.Name.ToLower().Contains("english") || l.Name.ToLower().Contains("ingl"));
        }
        return locale!;
    }

}
