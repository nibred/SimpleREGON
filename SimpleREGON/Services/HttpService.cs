using System.Diagnostics;

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
    internal async Task LoginAsync()
    {
        _baseApiKey ??= await GetBaseKeyAsync();
        await UpdateApiKeyAsync();
        _apiKeyUpdateTimer = new(async state => await UpdateApiKeyAsync(), null, Settings.ApiKeyUpdateIntervalMinutes, Settings.ApiKeyUpdateIntervalMinutes);
    }
    internal async Task<string> SearchAsync<T>(T value)
    {
        StringContent content = _jsonService.SerializeRequest(value);
        HttpResponseMessage response = await _httpClient.PostAsync(Settings.ApiDataSearchUrl, content);
        return await _jsonService.DeserializeRequestAsync(response);
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
        AddHeader("Referer", Settings.MainPage);
        AddHeader("User-Agent", Settings.UserAgent);
        AddHeader("Connection", "keep-alive");
        AddHeader("Sid", "");
    }
    private async Task UpdateApiKeyAsync()
    {
        var response = await PostAsync(Settings.ApiLoginUrl, _jsonService.SerializeLogin(_baseApiKey));
        _sessionApiKey = await _jsonService.DeserializeValueAsync(response);
        AddHeader("Sid", _sessionApiKey);
    }
    private async Task<string> GetBaseKeyAsync()
    {
        var response = await GetAsync(Settings.MainPage);
        return await _jsonService.ParseBaseKeyAsync(response);
    }
}
