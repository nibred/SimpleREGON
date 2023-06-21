namespace SimpleREGON.Services;

internal static class Settings
{
    internal static string MainPage => "https://wyszukiwarkaregon.stat.gov.pl/appBIR/index.aspx";
    internal static string UserAgent => "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/526.18 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/526.18";
    internal static string ApiLoginUrl => "https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc/ajaxEndpoint/Zaloguj";
    internal static string ApiDataSearchUrl => "https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc/ajaxEndpoint/daneSzukaj";
    internal static TimeSpan ApiKeyUpdateIntervalMinutes => TimeSpan.FromMinutes(4);
}
