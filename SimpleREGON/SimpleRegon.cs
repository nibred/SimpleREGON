using SimpleREGON.Services;

namespace SimpleREGON;

public class SimpleRegon(string apiKey = "")
{
    private readonly HttpService _httpService = new(apiKey);
    private readonly JsonService _jsonService = new();

    public string GetCurrentApiKey => _httpService.CurrentKey;

    public static bool ValidateNip(string nip)
    {
        nip = nip.Trim().Replace("-", string.Empty);
        if (nip.Length != 10 || nip.Any(chr => !char.IsDigit(chr)))
            return false;
        int[] weights = [6, 5, 7, 2, 3, 4, 5, 6, 7, 0];
        int sum = nip.Zip(weights, (d, w) => (d - '0') * w).Sum() % 11;
        return (sum % 10) == (nip[9] - '0');
    }
    public static bool ValidateRegon(string regon)
    {
        regon = regon.Trim();
        int[] weights9 = [8, 9, 2, 3, 4, 5, 6, 7];
        int[] weights14 = [2, 4, 8, 5, 0, 9, 7, 3, 6, 1, 2, 4, 8];
        if (regon.Any(chr => !char.IsDigit(chr)))
            return false;
        int regonSum(int[] weights) => regon.Zip(weights, (d, w) => (d - '0') * w).Sum() % 11;
        return regon.Length switch
        {
            9 => regonSum(weights9) % 10 == (regon[8] - '0'),
            14 => regonSum(weights14) % 10 == (regon[13] - '0'),
            _ => false
        };
    }

    public Task<string> GetDataByNipAsync(string nip)
    {
        Dictionary<string, string> parameters = new() { { "Nip", nip } };
        return GetDataAsync(nip, parameters, ValidateNip);
    }
    public Task<string> GetDataByRegonAsync(string regon)
    {
        Dictionary<string, string> parameters = new() { { "Regon", regon } };
        return GetDataAsync(regon, parameters, ValidateRegon);
    }
    public Task<string> GetDataByKrsAsync(string krs)
    {
        Dictionary<string, string> parameters = new() { { "Krs", krs } };
        return GetDataAsync(krs, parameters);
    }
    public async Task<string> GetDateStatusAsync() => await GetStatusAsync("StanDanych");

    public async Task<string> GetSessionStatusAsync() => await GetStatusAsync("StatusSesji", result => Settings.SessionStatusCodeDescription[_jsonService.DeserializeResponse(result)]);

    public async Task<string> GetServiceStatusAsync() => await GetStatusAsync("StatusUslugi", result => Settings.ServiceStatusCodeDescription[_jsonService.DeserializeResponse(result)]);

    private async Task<string> GetStatusAsync(string value, Func<string, string>? func = null)
    {
        var content = JsonService.SerializeToJsonContent(new { pNazwaParametru = value });
        var (success, result) = await _httpService.TryRequestAsync(Settings.UrlApiGetValueEndpoint, content);
        if (success)
        {
            if (func is null)
            {
                return _jsonService.SerializeToJsonString(new { success = true, status = _jsonService.DeserializeResponse(result) });
            }
            else
            {
                return _jsonService.SerializeToJsonString(new { success = true, status = func(result) });
            }
        }
        return result;
    }
    private async Task<string> GetDataAsync(string value, Dictionary<string, string> parameters, Func<string, bool>? validateFunc = null)
    {
        if (validateFunc is not null && !validateFunc(value))
            return _jsonService.ThrowErrorJson(Settings.IncorrectValue);

        var searchParameters = parameters.ToDictionary(
            p => p.Key,
            p => p.Value.Trim().Replace("-", string.Empty));

        var searchPayload = JsonService.SerializeToJsonContent(new
        {
            pParametryWyszukiwania = searchParameters,
            jestWojPowGmnMiej = true
        });
        var (success, response) = await _httpService.TryRequestAsync(Settings.UrlApiDataEndpoint, searchPayload);
        if (!success || string.IsNullOrWhiteSpace(response))
            return response;

        string searchData = _jsonService.DeserializeResponse(response);
        var reportPayload = JsonService.ParseDetails(searchData);
        if (reportPayload is null)
            return _jsonService.ThrowErrorJson(Settings.ParseError);
        (success, response) = await _httpService.TryRequestAsync(Settings.UrlApiFullDataEndpoint, reportPayload);
        if (!success || string.IsNullOrWhiteSpace(response))
            return response;

        var fullData = _jsonService.DeserializeResponse(response);
        var pkdPayload = JsonService.ParseDetails(fullData);
        string? pkdData = null;
        if (pkdPayload is not null)
        {
            (success, response) = await _httpService.TryRequestAsync(Settings.UrlApiFullDataEndpoint, pkdPayload);
            if (!success || string.IsNullOrWhiteSpace(response))
                return response;
            pkdData = _jsonService.DeserializeResponse(response);
        }

        return _jsonService.ExtractAndSerializeResponse(fullData, pkdData);
    }
}