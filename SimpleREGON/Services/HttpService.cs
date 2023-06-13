namespace SimpleREGON.Services;

internal class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonService _jsonService;
    private string? _sessionApiKey;
    private string? _baseApiKey;

    internal HttpService()
    {
        _httpClient ??= new HttpClient();
        _jsonService ??= new JsonService();
        ConfigureBaseHeaders();
    }
    internal string? BaseApiKey => _baseApiKey;
    internal async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }
    internal async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync(url, content, cancellationToken)
            .ConfigureAwait(false);
    }
    private void AddHeader(string key, string value)
    {
        _httpClient.DefaultRequestHeaders.Remove(key);
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }
    private void ConfigureBaseHeaders()
    {
        AddHeader("Accept", "application/json");
        AddHeader("Host", "wyszukiwarkaregon.stat.gov.pl");
        AddHeader("Origin", @"https://wyszukiwarkaregon.stat.gov.pl");
        AddHeader("Referer", Settings.MainPage);
        AddHeader("User-Agent", Settings.UserAgent);
        AddHeader("Connection", "keep-alive");
        AddHeader("Sid", "");
    }
    internal async Task UpdateApiKey(string apiKey)
    {
        var response = await PostAsync(Settings.ApiLoginUrl, _jsonService.SerializeLogin(apiKey));
        _sessionApiKey = await _jsonService.ParseSessionKeyAsync(response);
        AddHeader("Sid", _sessionApiKey);
    }

    internal async Task<bool> Login()
    {
        _baseApiKey ??= await GetBaseKey();
        await UpdateApiKey(_baseApiKey);
        return !string.IsNullOrEmpty(_sessionApiKey);
    }
    internal async Task<bool> Login(string apiKey)
    {
        if (apiKey.Length < 20)
            return false;
        await UpdateApiKey(apiKey);
        return !string.IsNullOrEmpty(_sessionApiKey);
    }

    private async Task<string> GetBaseKey()
    {
        var response = await GetAsync(Settings.MainPage);
        return await _jsonService.ParseBaseKeyAsync(response);
    }
}
