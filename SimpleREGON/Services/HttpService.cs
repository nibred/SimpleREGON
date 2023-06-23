namespace SimpleREGON.Services;

internal class HttpService
{
    private readonly HttpClient _httpClient;
    internal HttpService()
    {
        _httpClient ??= new HttpClient();
        ConfigureBaseHeaders();
    }
    internal async Task<Stream> PostRequestAsync(string requestUri, StringContent content)
    {
        var response = await _httpClient.PostAsync(requestUri, content);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStreamAsync();
        return Stream.Null;
    }
    internal async Task<string> GetRequestAsync(string requestUri)
    {
        var response = await _httpClient.GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();
        return string.Empty;
    }
    internal void SetHeader(string key, string? value)
    {
        _httpClient.DefaultRequestHeaders.Remove(key);
        _httpClient.DefaultRequestHeaders.Add(key, value);
    }
    private void ConfigureBaseHeaders()
    {
        SetHeader("Accept", "application/json");
        SetHeader("Host", Settings.UrlApiHost);
        SetHeader("Origin", $"https://{Settings.UrlApiHost}");
        SetHeader("Referrer", Settings.UrlMainPage);
        SetHeader("User-Agent", Settings.UserAgent);
        SetHeader("Connection", "keep-alive");
        SetHeader("Sid", "");
    }
}
