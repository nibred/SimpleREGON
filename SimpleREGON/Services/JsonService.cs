using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SimpleREGON.Services;

internal class JsonService
{
    private readonly SerializationService _serializationService;
    private readonly DeserializationService _deserializationService;
    private readonly HttpService _httpService;
    private string? _sessionApiKey;
    private string? _baseApiKey;
    private Timer? _apiKeyUpdateTimer;
    private const string BaseKeyPattern = @"eval\(String.fromCharCode\((.*)\)\)";
    public JsonService()
    {
        _serializationService ??= new SerializationService();
        _deserializationService ??= new DeserializationService();
        _httpService ??= new HttpService();
    }
    internal async Task<bool> LoginAsync()
    {
        _baseApiKey ??= await GetBaseKeyAsync();
        await UpdateApiKeyAsync();
        _apiKeyUpdateTimer = new(async state => await UpdateApiKeyAsync(), null, Settings.ApiKeyUpdateIntervalMinutes, Settings.ApiKeyUpdateIntervalMinutes);
        return string.IsNullOrEmpty(_sessionApiKey);
    }
    internal async Task<string> GetStatusAsync(string value)
    {
        StringContent content = _serializationService.SerializeBaseRequest("pNazwaParametru", value);
        Stream? answer = await _httpService.PostRequestAsync(Settings.UrlApiGetValueEndpoint, content);
        return await _deserializationService.DeserializeBaseRequestAsync(answer);
    }
    internal async Task<(bool result, string status)> TryGetStatusAsync(Func<Task<string>> function)
    {
        string status = await function();
        return (!string.IsNullOrEmpty(status), status);
    }
    //internal async Task<string> GetResponseAsync(HttpResponseMessage response)
    //{
    //    var resultDict = await DeserializeResponseToDict(response) ?? new List<Dictionary<string, string>>();
    //    foreach (var json in resultDict)
    //    {
    //        json.Remove("RegonLink");
    //        json.Remove("nazwaRaportu");
    //        if (json.TryGetValue("DataZak", out string? dataZak) && dataZak.StartsWith('-'))
    //            json["DataZak"] = string.Empty;
    //    }
    //    return SerializeDictToJson(resultDict);
    //}

    //internal async Task<StringContent> ParseFullReportDataAsync(HttpResponseMessage response)
    //{
    //    var content = new Dictionary<string, string>
    //    {
    //        ["pNazwaRaportu"] = "",
    //        ["pRegon"] = "",
    //        ["pSilosID"] = ""
    //    };
    //    List<Dictionary<string, string>>? resultDict = await DeserializeResponseToDict(response);
    //    if (resultDict != null && resultDict[0].ContainsKey("RegonLink"))
    //    {
    //        string pattern = @"danePobierzPelnyRaport\(""([^""]+)"",\s*""([^""]+)"",\s*([^\\s)]+)";
    //        Match match = Regex.Match(resultDict[0]["RegonLink"], pattern);
    //        content["pNazwaRaportu"] = match.Groups[2].Value;
    //        content["pRegon"] = match.Groups[1].Value;
    //        content["pSilosID"] = match.Groups[3].Value;
    //    }

    //    return SerializeRequest(content);
    //}

    //internal StringContent SerializeRequest<T>(T data) => new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    //private async Task<List<Dictionary<string, string>>?> DeserializeResponseToDict(HttpResponseMessage response)
    //{
    //    string value = await DeserializeValueAsync(response);
    //    if (string.IsNullOrEmpty(value))
    //        return null;
    //    byte[] jsonBytes = Encoding.UTF8.GetBytes(value);
    //    var stream = new MemoryStream(jsonBytes);

    //    return await JsonSerializer.DeserializeAsync<List<Dictionary<string, string>>>(stream);
    //}
    //private string SerializeDictToJson<T>(T data)
    //{
    //    return JsonSerializer.Serialize(data, new JsonSerializerOptions
    //    {
    //        WriteIndented = true,
    //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    //    });
    //}
    private async Task<string> GetBaseKeyAsync()
    {
        string response = await _httpService.GetRequestAsync(Settings.UrlMainPage);
        Match? match = Regex.Match(response, BaseKeyPattern, RegexOptions.Multiline);
        string? charCodes = match?.Groups[1]?.Value ?? string.Empty;
        return string.Join("", charCodes
            .Split(',')
            .Select(x => (char)int.Parse(x))
            .Skip(19)
            .Take(20));
    }
    private async Task UpdateApiKeyAsync()
    {
        Stream? response = await _httpService.PostRequestAsync(Settings.UrlApiLoginEndpoint,
            _serializationService.SerializeBaseRequest("pKluczUzytkownika", _baseApiKey ?? string.Empty));
        _sessionApiKey = await _deserializationService.DeserializeBaseRequestAsync(response);
        _httpService.SetHeader("Sid", _sessionApiKey);
    }
}
