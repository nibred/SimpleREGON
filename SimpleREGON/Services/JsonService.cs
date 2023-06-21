using System.Text;
using System.Text.Json;
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
    internal StringContent SerializeLogin(string baseKey) => CreateStringContent(new Dictionary<string, string>()
        {
            {"pKluczUzytkownika", baseKey}
        });
    internal StringContent SerializeRequest<T>(T data) => CreateStringContent(data);
    internal async Task<string> DeserializeRequestAsync(HttpResponseMessage response) => await DeserializeValueAsync(response);
    private StringContent CreateStringContent<T>(T data) => new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    private bool IsSuccessStatusCode(HttpResponseMessage response) => response.IsSuccessStatusCode;
}
