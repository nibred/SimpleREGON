using SimpleREGON.Models;
using SimpleREGON.Models.Request;
using SimpleREGON.Models.Response;
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
    internal async Task<string?> ParseDataAsync(HttpResponseMessage response)
    {
        var result = await JsonSerializer.DeserializeAsync<DataValue>(await response.Content.ReadAsStreamAsync());
        return result?.Data;
    }
    internal StringContent SerializeLogin(string baseKey)
    {
        Login login = new()
        {
            Key = baseKey
        };
        return CreateStringContent(login);
    }
    internal StringContent SerializeMainRequest(Identyfikator identyfikator, string search)
    {
        GetData data = new();
        _ = identyfikator switch
        {
            Identyfikator.NIP => data.pParametryWyszukiwania.Nip = search,
            Identyfikator.NIPy => data.pParametryWyszukiwania.Nipy = search,
            Identyfikator.REGON => data.pParametryWyszukiwania.Regon = search,
            Identyfikator.REGONy9 => data.pParametryWyszukiwania.Regony9zn = search,
            Identyfikator.REGONy14 => data.pParametryWyszukiwania.Regony14zn = search,
            Identyfikator.KRS => data.pParametryWyszukiwania.Krs = search,
            Identyfikator.KRSy => data.pParametryWyszukiwania.Krsy = search,
            _ => throw new NotImplementedException()
        };
        return CreateStringContent(data);
    }
    internal async Task<List<T>> DeserializeMainRequestAsync<T>(HttpResponseMessage response)
    {
        var payload = new MemoryStream(Encoding.UTF8.GetBytes(await ParseDataAsync(response) ?? ""));
        return await JsonSerializer.DeserializeAsync<List<T>>(payload);
    }
    private StringContent CreateStringContent<T>(T data)
    {
        string payload = JsonSerializer.Serialize(data);
        return new StringContent(payload, Encoding.UTF8, "application/json");
    }

}
