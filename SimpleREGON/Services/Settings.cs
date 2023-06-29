namespace SimpleREGON.Services;

internal static class Settings
{
    public const string UrlMainPage = "https://wyszukiwarkaregon.stat.gov.pl/appBIR/index.aspx";
    public const string UrlApiBase = "https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc/ajaxEndpoint/";
    public const string UrlApiHost = "wyszukiwarkaregon.stat.gov.pl";

    public const string UrlApiLoginEndpoint = UrlApiBase + "Zaloguj";
    public const string UrlApiDataEndpoint = UrlApiBase + "daneSzukaj";
    public const string UrlApiFullDataEndpoint = UrlApiBase + "DanePobierzPelnyRaport";
    public const string UrlApiGetValueEndpoint = UrlApiBase + "GetValue";

    public static readonly TimeSpan ApiKeyUpdateIntervalMinutes = TimeSpan.FromMinutes(3);
    public static string UserAgent => RandomizeUserAgent();

    public static readonly Dictionary<string, string> SessionStatusCodeDescription = new()
    {
        ["0"] = "Sesja nie istnieje",
        ["1"] = "Sesja istnieje"
    };

    public static readonly Dictionary<string, string> ServiceStatusCodeDescription = new()
    {
        ["0"] = "Usługa niedostępna",
        ["1"] = "Usługa dostępna",
        ["2"] = "Przerwa techniczna"
    };

    private static string RandomizeUserAgent()
    {
        string version = _random.Next(100, 115).ToString();
        string webKit = $"{_random.Next(440, 550)}.{_random.Next(10, 100)}";
        string[] baseUserAgents =
        {
            $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/{webKit} (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/{webKit}",
            $"Mozilla/5.0 (Windows NT 10.0; rv:{version}.0) Gecko/20100101 Firefox/{version}.0",
            $"Mozilla/5.0 (Macintosh; Intel Mac OS X 13_4) AppleWebKit/{webKit} (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/{webKit}"
        };
        return baseUserAgents[_random.Next(baseUserAgents.Length)];
    }
    private static readonly Random _random = new();
}
