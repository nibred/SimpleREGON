using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SimpleREGON.Services;

internal class JsonService
{
    private readonly string[] _chunks = ["y1y", "yy2", "yy3", "yy4", "yy5", "yy6", "yy7", "yy8", "yy9", "y20", "y2y", "y22", "y23", "y24", "y25", "y26", "y27", "y28", "y29", "y30", "!3!", "!32", "!33", "!34", "!35", "!36", "!37", "!38", "!39", "!40", "!4!", "!42", "!43", "!44", "!45", "!46", "!47", "!48", "!49", "!50", "!5!", "!52", "!53", "1b4", "1bb", "1ba", "1b7", "1b8", "1b9", "1a0", "1a1", "1a2", "1a3", "1a4", "1ab", "1aa", "1a7", "1a8", "1a9", "170", "171", "172", "173", "174", "17b", "71a"];
    private readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    internal JsonService() { }

    internal string ExtractAndSerializeResponse(string data, string? pkds)
    {
        JsonElement jsonData = JsonSerializer.Deserialize<JsonElement>(data);
        JsonElement jsonPkds = JsonSerializer.Deserialize<JsonElement>(pkds ?? "[]");
        Dictionary<string, string> result = ExtractMappedValues(jsonData[0].EnumerateObject());
        List<Dictionary<string, string>> pkdList = [];
        foreach (var element in jsonPkds.EnumerateArray())
        {
            var pkdEntry = ExtractMappedValues(element.EnumerateObject());
            if (pkdEntry.Count > 0)
                pkdList.Add(pkdEntry);
        }
        var responseData = result.ToDictionary(
            pair => pair.Key,
            pair => (object)pair.Value);
        if (pkdList.Count > 0)
            responseData["pkds"] = pkdList;

        return SerializeToJsonString(new
        {
            success = true,
            data = responseData
        });
    }

    internal string ThrowErrorJson(string description) => SerializeToJsonString(new { success = false, description });

    internal string SerializeToJsonString<T>(T parameters) => JsonSerializer.Serialize(parameters, _options);

    internal static JsonContent SerializeToJsonContent<T>(T parameters)
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = null
        };
        return JsonContent.Create(parameters, options: options);
    }

    internal string DeserializeResponse(string? response)
    {
        JsonElement jsonResponse = JsonSerializer.Deserialize<JsonElement>(response ?? "{}");
        if (jsonResponse.TryGetProperty("d", out var result))
        {
            if (result.GetString()!.StartsWith("enc"))
                return Decode(result.GetString()!);
            return result.GetString()!;
        }
        return string.Empty;
    }

    internal static JsonContent? ParseDetails(string data)
    {
        var match = Regex.Match(data, @"danePobierzPelnyRaport\D*(\d+)\W*(\w+)\W*(\d*)");
        if (match.Success)
        {
            string regon = match.Groups[1].Value;
            string reportType = match.Groups[2].Value;
            string number = match.Groups[3].Value;
            if (string.IsNullOrWhiteSpace(number))
                number = "undefined";
            return SerializeToJsonContent(new { pNazwaRaportu = reportType, pRegon = regon, pSilosID = number });
        }
        return null;
    }

    private string Decode(string input)
    {
        string output = string.Empty;
        for (int i = 3; i < input.Length; i += 3)
        {
            string c = input.Substring(i, 3);
            output += _alphabet[Array.IndexOf(_chunks, c)];
        }
        return Encoding.UTF8.GetString(Convert.FromBase64String(output));
    }

    private static Dictionary<string, string> ExtractMappedValues(IEnumerable<JsonProperty> properties)
    {
        Dictionary<string, string>? mappedValues = [];
        foreach (var property in properties)
        {
            string key = property.Name.ToLowerInvariant();
            string baseField = key.Contains('_') ? key[(key.IndexOf('_') + 1)..] : key;
            if (!Settings.ResultKeysTranscription.TryGetValue(baseField, out var mappedKey))
                continue;
            string value = property.Value.GetString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                mappedValues[mappedKey] = value;
            }
        }
        return mappedValues;
    }
}
