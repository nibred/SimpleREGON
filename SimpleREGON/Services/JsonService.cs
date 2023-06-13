using SimpleREGON.Models.Request;
using SimpleREGON.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleREGON.Services;

internal class JsonService
{
    internal async Task<string> ParseBaseKeyAsync(HttpResponseMessage response)
    {
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
    internal async Task<string?> ParseSessionKeyAsync(HttpResponseMessage response)
    {
        var result = await JsonSerializer.DeserializeAsync<DataValue>(await response.Content.ReadAsStreamAsync());
        return result?.Data;
    }
    internal StringContent SerializeLogin(string baseKey)
    {
        Login login = new();
        login.Key = baseKey;
        string payload = JsonSerializer.Serialize(login);
        return new StringContent(payload, Encoding.UTF8, "application/json");
    }

}
