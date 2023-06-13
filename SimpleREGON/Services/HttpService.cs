using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleREGON.Services;

internal class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly JsonService _jsonService;
    private string _userApiKey;
    private string _tempApiKey;
    private string _baseApiKey;
    private DateTime _apiKeyUpdateTime;

    internal HttpService(string apiKey)
    {
        _httpClient ??= new HttpClient();
        _userApiKey = apiKey;
        _apiKeyUpdateTime = DateTime.Now;
        _jsonService = new JsonService();
        ConfigureBaseHeaders();
    }
    internal async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        await UpdateApiKey();
        return await _httpClient.GetAsync(url, cancellationToken)
            .ConfigureAwait(false);
    }
    internal async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, CancellationToken cancellationToken = default)
    {
        await UpdateApiKey();
        return await _httpClient.PostAsync(url, content, cancellationToken)
            .ConfigureAwait(false);
    }
    private void AddHeader(string key, string value) => _httpClient.DefaultRequestHeaders.Add(key, value);
    private void ConfigureBaseHeaders()
    {
        AddHeader("Accept", "application/json");
        AddHeader("Host", "wyszukiwarkaregon.stat.gov.pl");
        AddHeader("Origin", @"https://wyszukiwarkaregon.stat.gov.pl");
        AddHeader("Referer", Settings.MainPage);
        AddHeader("User-Agent", Settings.UserAgent);
        AddHeader("Connection", "keep-alive");
        AddHeader("Sid", _userApiKey);
    }
    private async Task UpdateApiKey()
    {
        if (!string.IsNullOrWhiteSpace(_userApiKey)) return;
        if (string.IsNullOrWhiteSpace(_baseApiKey))
        {
            _baseApiKey = await GetBaseKey();
        }
        if (string.IsNullOrWhiteSpace(_tempApiKey) || 
            DateTime.Now.Subtract(_apiKeyUpdateTime).TotalMinutes >= Settings.ApiKeyValidMinutes)
        {
            var response = await PostAsync(Settings.ApiLoginUrl, _jsonService.SerializeLogin(_baseApiKey));
            _tempApiKey = await _jsonService.UpdateKeyAsync(response);
            AddHeader("Sid", _tempApiKey);
        }         
    }
    private async Task<string> GetBaseKey()
    {
        var response = await GetAsync(Settings.MainPage);
        return await _jsonService.ParseBaseKeyAsync(response);
    }
}
