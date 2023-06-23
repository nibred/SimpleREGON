namespace SimpleREGON.Services;

internal static class Settings
{
    internal static string UrlMainPage => "https://wyszukiwarkaregon.stat.gov.pl/appBIR/index.aspx";
    internal static string UrlApiBase => "https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc/ajaxEndpoint/";
    internal static string UrlApiHost => "wyszukiwarkaregon.stat.gov.pl";
    internal static string UrlApiLoginEndpoint => $"{UrlApiBase}Zaloguj";
    internal static string UrlApiDataEndpoint => $"{UrlApiBase}daneSzukaj";
    internal static string UrlApiFullDataEndpoint => $"{UrlApiBase}DanePobierzPelnyRaport";
    internal static string UserAgent => RandomizeUserAgent();
    internal static TimeSpan ApiKeyUpdateIntervalMinutes => TimeSpan.FromMinutes(3);
    private static string RandomizeUserAgent()
    {
        Random random = new();
        string version = random.Next(100, 115).ToString();
        string webKit = $"{random.Next(440, 550)}.{random.Next(10, 100)}";
        string[] baseUserAgents = new[] { 
            $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/{webKit} (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/{webKit}",
            $"Mozilla/5.0 (Windows NT 10.0; rv:{version}.0) Gecko/20100101 Firefox/{version}.0",
            $"Mozilla/5.0 (Macintosh; Intel Mac OS X 13_4) AppleWebKit/{webKit} (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/{webKit}"};
        return baseUserAgents[random.Next(baseUserAgents.Length)];
    }
}
