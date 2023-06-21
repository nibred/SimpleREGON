using SimpleREGON.Models.Request;
using SimpleREGON.Services;

namespace SimpleREGON;

public class SimpleRegon
{
    private readonly HttpService? _httpService;
    public SimpleRegon() => _httpService ??= new HttpService();

    public async Task Login() => await _httpService!.LoginAsync();
    public bool ValidateNip(string nip)
    {
        nip = nip.Replace("-", string.Empty);
        if (nip.Length != 10 || nip.Any(chr => !char.IsDigit(chr)))
            return false;
        int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7, 0 };
        int sum = nip.Zip(weights, (d, w) => (d - '0') * w).Sum() % 11;
        return (sum % 10) == (nip[9] - '0');
    }
    public bool ValidateRegon(string regon)
    {
        int[] weights9 = { 8, 9, 2, 3, 4, 5, 6, 7 };
        int[] weights14 = { 2, 4, 8, 5, 0, 9, 7, 3, 6, 1, 2, 4, 8 };
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
    public async Task<string> FindByNipAsync(params string[] nipy)
    {
        GetData getData = new();
        foreach (string nip in nipy)
        {
            if (!ValidateNip(nip)) return string.Empty;
        }
        string joinNips = string.Join("\n", nipy);
        _ = nipy.Length switch
        {
            1 => getData.pParametryWyszukiwania.Nip = nipy[0],
            _ => getData.pParametryWyszukiwania.Nipy = joinNips
        };
        return await _httpService!.SearchAsync(getData);
    }
    public async Task<string> FindByRegonAsync(params string[] regony)
    {
        GetData getData = new();
        foreach (string regon in regony)
        {
            if (!ValidateRegon(regon)) return string.Empty;
        }
        if (!regony.All(x => x.Length == regony[0].Length))
            return string.Empty;
        string joinRegons = string.Join("\n", regony);
        _ = regony.Length switch
        {
            1 => getData.pParametryWyszukiwania.Regon = regony[0],
            _ => regony[0].Length switch
            {
                9 => getData.pParametryWyszukiwania.Regony9zn = joinRegons,
                _ => getData.pParametryWyszukiwania.Regony14zn = joinRegons
            }
        };
        return await _httpService!.SearchAsync(getData);
    }
}