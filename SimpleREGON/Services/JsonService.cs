using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SimpleREGON.Services;

internal class JsonService
{
    private readonly SerializationService _serializationService;
    private readonly DeserializationService _deserializationService;
    private readonly HttpService _httpService;

    public JsonService()
    {
        _serializationService ??= new SerializationService();
        _deserializationService ??= new DeserializationService();
        _httpService ??= new HttpService();
    }
    internal async Task<string> ParseBaseKeyAsync(HttpResponseMessage response)
    {
        if (!IsSuccessStatusCode(response))
            return string.Empty;
        string result = await response.Content.ReadAsStringAsync();
        string pattern = @"eval\(String.fromCharCode\((.*)\)\)";
        Match? match = Regex.Match(result, pattern, RegexOptions.Multiline);
        string? charCodes = match?.Groups[1]?.Value ?? string.Empty;
        return string.Join("", charCodes
            .Split(',')
            .Select(x => (char)int.Parse(x))
            .Skip(19)
            .Take(20));
    }
    internal async Task<string> DeserializeValueAsync(HttpResponseMessage response)
    {
        if (!IsSuccessStatusCode(response))
            return string.Empty;
        Stream contentStream = await response.Content.ReadAsStreamAsync();
        var resultDict = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(contentStream);
        return resultDict?.GetValueOrDefault("d") ?? string.Empty;
    }
    internal StringContent SerializeLogin(string baseKey) => SerializeRequest(new Dictionary<string, string>()
        {
            ["pKluczUzytkownika"] = baseKey
        });
    internal async Task<string> GetResponseAsync(HttpResponseMessage response)
    {
        var resultDict = await DeserializeResponseToDict(response) ?? new List<Dictionary<string, string>>();
        foreach (var json in resultDict)
        {
            json.Remove("RegonLink");
            json.Remove("nazwaRaportu");
            if (json.TryGetValue("DataZak", out string? dataZak) && dataZak.StartsWith('-'))
                json["DataZak"] = string.Empty;
        }
        return SerializeDictToJson(resultDict);
    }

    internal async Task<StringContent> ParseFullReportDataAsync(HttpResponseMessage response)
    {
        var content = new Dictionary<string, string>
        {
            ["pNazwaRaportu"] = "",
            ["pRegon"] = "",
            ["pSilosID"] = ""
        };
        List<Dictionary<string, string>>? resultDict = await DeserializeResponseToDict(response);
        if (resultDict != null && resultDict[0].ContainsKey("RegonLink"))
        {
            string pattern = @"danePobierzPelnyRaport\(""([^""]+)"",\s*""([^""]+)"",\s*([^\\s)]+)";
            Match match = Regex.Match(resultDict[0]["RegonLink"], pattern);
            content["pNazwaRaportu"] = match.Groups[2].Value;
            content["pRegon"] = match.Groups[1].Value;
            content["pSilosID"] = match.Groups[3].Value;
        }

        return SerializeRequest(content);
    }

    internal StringContent SerializeRequest<T>(T data) => new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    private bool IsSuccessStatusCode(HttpResponseMessage response) => response.IsSuccessStatusCode;
    private async Task<List<Dictionary<string, string>>?> DeserializeResponseToDict(HttpResponseMessage response)
    {
        string value = await DeserializeValueAsync(response);
        if (string.IsNullOrEmpty(value))
            return null;
        byte[] jsonBytes = Encoding.UTF8.GetBytes(value);
        var stream = new MemoryStream(jsonBytes);

        return await JsonSerializer.DeserializeAsync<List<Dictionary<string, string>>>(stream);
    }
    private string SerializeDictToJson<T>(T data)
    {
        return JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}
