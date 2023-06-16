namespace SimpleREGON.Services;

internal class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonService _jsonService;
    private string? _sessionApiKey;
    private string? _baseApiKey;
    private bool _userApiKey;
    private DateTime _sessionApiKeyTime;

    internal HttpService()
    {
        _httpClient ??= new HttpClient();
        _jsonService ??= new JsonService();
        ConfigureBaseHeaders();
    }

    internal async Task<bool> LoginAsync()
    {
        _userApiKey = false;
        _baseApiKey ??= await GetBaseKeyAsync();
        await UpdateApiKeyAsync(_baseApiKey);
        return SetSessionTime();
    }
    internal async Task<bool> LoginAsync(string apiKey)
    {
        if (apiKey.Length < 20)
            return false;
        await UpdateApiKeyAsync(apiKey);
        if (SetSessionTime())
        {
            _userApiKey = true;
            return true;
        }
        return false;
    }
    internal async Task<string> SearchAsync<T>(T value)
    {
        await CheckApiKeyAsync();
        StringContent content = _jsonService.SerializeRequest(value);
        HttpResponseMessage response = await _httpClient.PostAsync(Settings.ApiDataSearchUrl, content);
        return await _jsonService.DeserializeRequestAsync(response);
    }
    private async Task CheckApiKeyAsync()
    {
        if (!_userApiKey || DateTime.Now.Subtract(_sessionApiKeyTime).TotalMinutes >= Settings.ApiKeyValidMinutes)
            await LoginAsync();
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
    private async Task UpdateApiKeyAsync(string apiKey)
    {
        var response = await PostAsync(Settings.ApiLoginUrl, _jsonService.SerializeLogin(apiKey));
        _sessionApiKey = await _jsonService.DeserializeValueAsync(response);
        AddHeader("Sid", _sessionApiKey);
    }
    private bool SetSessionTime()
    {
        if (!string.IsNullOrEmpty(_sessionApiKey))
        {
            _sessionApiKeyTime = DateTime.Now;
            return true;
        }
        return false;
    }
    private async Task<string> GetBaseKeyAsync()
    {
        var response = await GetAsync(Settings.MainPage);
        return await _jsonService.ParseBaseKeyAsync(response);
    }
}
