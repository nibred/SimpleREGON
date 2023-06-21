using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace SimpleREGON.Services;

internal class JsonService
{
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
            .Select(x => Convert.ToChar(int.Parse(x)))
            .Skip(19)
            .Take(20)
            .ToArray());
    }
    internal async Task<string> DeserializeValueAsync(HttpResponseMessage response)
    {
        if (!IsSuccessStatusCode(response))
            return string.Empty;
        var result = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await response.Content.ReadAsStreamAsync());
        if (result != null && result.ContainsKey("d"))
            return result["d"];
        return string.Empty;
    }
    internal StringContent SerializeLogin(string baseKey) => SerializeRequest(new Dictionary<string, string>()
        {
            {"pKluczUzytkownika", baseKey}
        });
    internal async Task<string> GetShortResponseAsync(HttpResponseMessage response)
    {
        List<Dictionary<string, string>>? dict = await DeserializeResponseToDict(response);
        if (dict == null)
            return string.Empty;
        foreach (var json in dict)
        {
            if (json.ContainsKey("RegonLink"))
                json.Remove("RegonLink");
            if (json.ContainsKey("DataZak") && json["DataZak"].StartsWith('-'))
                json["DataZak"] = "";
        }
        return SerializeDictToJson(dict);
    }

    internal async Task<StringContent> GetFullResponseAsync(HttpResponseMessage response)
    {
        Dictionary<string, string> content = new() 
        { 
            { "pNazwaRaportu", "" }, 
            { "pRegon", "" }, 
            { "pSilosID", "" } 
        };
        List<Dictionary<string, string>>? dict = await DeserializeResponseToDict(response);
        if (dict == null)
            return SerializeRequest(content);
        if (dict[0].ContainsKey("RegonLink"))
        {
            string pattern = @"danePobierzPelnyRaport\(""([^""]+)"",\s*""([^""]+)"",\s*([^\\s)]+)";
            Match? match = Regex.Match(dict[0]["RegonLink"], pattern);
            string regon = match.Groups[1].Value;
            string report = match.Groups[2].Value;
            string silosId = match.Groups[3].Value;
            content["pNazwaRaportu"] = report;
            content["pRegon"] = regon;
            content["pSilosID"] = silosId;
        }
        return SerializeRequest(content);
    }

    internal StringContent SerializeRequest<T>(T data) => new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    internal async Task<string> DeserializeRequestAsync(HttpResponseMessage response) => await DeserializeValueAsync(response);
    private bool IsSuccessStatusCode(HttpResponseMessage response) => response.IsSuccessStatusCode;
    private async Task<List<Dictionary<string, string>>?> DeserializeResponseToDict(HttpResponseMessage response)
    {
        string value = await DeserializeValueAsync(response);
        if (string.IsNullOrEmpty(value))
            return null;
        List<Dictionary<string, string>>? jsonDictionary = await JsonSerializer.DeserializeAsync<List<Dictionary<string, string>>>(new MemoryStream(Encoding.UTF8.GetBytes(value)));
        return jsonDictionary switch
        {
            null => null,
            _ => jsonDictionary,
        };
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
