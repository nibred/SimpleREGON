using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace SimpleREGON.Services;

internal class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonService _jsonService;
    private string _apiKey;
    private DateTimeOffset _timestamp = DateTimeOffset.MinValue;
    internal string CurrentKey { get; private set; }

    internal HttpService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _jsonService = new JsonService();
        ConfigureBaseHeaders();
        SetHeader("Sid", _apiKey);
        CurrentKey = _apiKey;
    }
    internal async Task<(bool success, string result)> TryRequestAsync(string requestUri, JsonContent content)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            if (!await CheckTimestamp())
            {
                return (false, _jsonService.ThrowErrorJson(Settings.ApiKeyNotFound));
            }
        }
        HttpRequestMessage request = new(HttpMethod.Post, requestUri) { Content = content };
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return (true, await response.Content.ReadAsStringAsync());
        }
        return (false, _jsonService.ThrowErrorJson($"{response.StatusCode}"));
    }

    private void SetHeader(string key, string value)
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
        SetHeader("User-Agent", RandomizeUserAgent());
        SetHeader("Connection", "keep-alive");
    }

    private string RandomizeUserAgent()
    {
        Random random = new();
        string version = random.Next(100, 135).ToString();
        string[] baseUserAgents =
        [
            $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/537.36",
            $"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/537.36",
            $"Mozilla/5.0 (iPhone; CPU iPhone OS 18_3_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.3 Mobile/15E148 Safari/604.1",
            $"Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version}.0.0.0 Mobile Safari/537.36",
            $"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/537.36",
        ];
        return baseUserAgents[random.Next(baseUserAgents.Length)];
    }

    private async Task<bool> CheckTimestamp()
    {
        if ((DateTimeOffset.UtcNow - _timestamp).TotalMinutes >= 2)
        {
            string sid = await GetSid();
            if (string.IsNullOrWhiteSpace(sid))
            {
                return false;
            }
            _timestamp = DateTimeOffset.UtcNow;
            SetHeader("Sid", sid);
            CurrentKey = sid;
        }
        return true;
    }

    public async Task<string> GetSid()
    {
        HttpResponseMessage result = await _httpClient.GetAsync(Settings.UrlMainPage);
        if (!result.IsSuccessStatusCode)
        {
            return string.Empty;
        }
        string? page = await result.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(page))
        {
            return string.Empty;
        }
        Match bytes = Regex.Match(page, @"'String\.fromCharCode\(((?<bytes>\d+)[,\)])+");
        string convertBytes = new([.. bytes.Groups["bytes"].Captures.Select(b => (char)Byte.Parse(b.Value))]);
        string key = convertBytes.Substring(convertBytes.IndexOf('\'') + 1, convertBytes.LastIndexOf('\'') - convertBytes.IndexOf('\'') - 1);
        JsonContent content = JsonService.SerializeToJsonContent(new { pKluczUzytkownika = key });
        HttpResponseMessage response = await _httpClient.PostAsync(Settings.UrlApiLoginEndpoint, content);
        if (!response.IsSuccessStatusCode) return string.Empty;
        return _jsonService.DeserializeResponse(await response.Content.ReadAsStringAsync());
    }
}
