namespace SimpleREGON.Services;

internal class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonService _jsonService;
    private string? _sessionApiKey;
    private string? _baseApiKey;
    private Timer? _apiKeyUpdateTimer;

    internal HttpService()
    {
        _httpClient ??= new HttpClient();
        _jsonService ??= new JsonService();
        ConfigureBaseHeaders();
    }
    internal async Task<bool> LoginAsync()
    {
        _baseApiKey ??= await GetBaseKeyAsync();
        await UpdateApiKeyAsync();
        _apiKeyUpdateTimer = new(async state => await UpdateApiKeyAsync(), null, Settings.ApiKeyUpdateIntervalMinutes, Settings.ApiKeyUpdateIntervalMinutes);
        return string.IsNullOrEmpty(_sessionApiKey);
    }
    internal async Task<string> SearchAsync<T>(string requestUri, T value)
    {
        StringContent content = _jsonService.SerializeRequest(value);
        HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
        var r = await _jsonService.ParseFullReportDataAsync(response);
        response = await _httpClient.PostAsync(Settings.UrlApiFullDataEndpoint, r);
        return await _jsonService.GetResponseAsync(response);
    }
    private async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }
    private async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync(url, content, cancellationToken)
            .ConfigureAwait(false);
    }
    private void AddHeader(string key, string? value)
    {
        _httpClient.DefaultRequestHeaders.Remove(key);
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }
    private void ConfigureBaseHeaders()
    {
        AddHeader("Accept", "application/json");
        AddHeader("Host", "wyszukiwarkaregon.stat.gov.pl");
        AddHeader("Origin", "https://wyszukiwarkaregon.stat.gov.pl");
        AddHeader("Referer", Settings.UrlMainPage);
        AddHeader("User-Agent", Settings.UserAgent);
        AddHeader("Connection", "keep-alive");
        AddHeader("Sid", "");
    }
    private async Task UpdateApiKeyAsync()
    {
        HttpResponseMessage response = await PostAsync(Settings.UrlApiLoginEndpoint, _jsonService.SerializeLogin(_baseApiKey ?? string.Empty));
        _sessionApiKey = await _jsonService.DeserializeValueAsync(response);
        AddHeader("Sid", _sessionApiKey);
    }
    private async Task<string> GetBaseKeyAsync()
    {
        HttpResponseMessage response = await GetAsync(Settings.UrlMainPage);
        return await _jsonService.ParseBaseKeyAsync(response);
    }
}
