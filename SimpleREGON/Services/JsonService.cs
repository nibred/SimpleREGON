using SimpleREGON.Models;
using SimpleREGON.Models.Request;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
    internal async Task<string> DeserializeValueAsync(HttpResponseMessage response)
    {
        var result = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await response.Content.ReadAsStreamAsync());
        if (result != null && result.ContainsKey("d")) 
            return result["d"];
        return string.Empty;
    }
    internal StringContent SerializeLogin(string baseKey)
    {
        Login login = new()
        {
            Key = baseKey
        };
        return CreateStringContent(login);
    }
    internal StringContent SerializeRequest<T>(T data)
    {
        return CreateStringContent(data);
    }

    internal async Task<string> DeserializeRequestAsync(HttpResponseMessage response)
    {
        return await DeserializeValueAsync(response);
    }
    //internal async Task<List<T>> DeserializeMainRequestAsync<T>(HttpResponseMessage response)
    //{
    //    var payload = new MemoryStream(Encoding.UTF8.GetBytes(await ParseDataAsync(response) ?? ""));
    //    return await JsonSerializer.DeserializeAsync<List<T>>(payload);
    //}
    private StringContent CreateStringContent<T>(T data)
    {
        string payload = JsonSerializer.Serialize(data);
        return new StringContent(payload, Encoding.UTF8, "application/json");
    }

}
