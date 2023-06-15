using SimpleREGON.Models;
using SimpleREGON.Models.DTO;
using SimpleREGON.Models.Response;
using SimpleREGON.Services;

namespace SimpleREGON;

public class SimpleRegon
{
    private readonly HttpService? _httpService;
    public SimpleRegon() => _httpService ??= new HttpService();
    public async Task<bool> LoginAsync() => await _httpService!.LoginAsync();
    public async Task<bool> LoginAsync(string apiKey) => await _httpService!.LoginAsync(apiKey);
    public bool ValidateNip(string nip)
    {
        nip = nip.Replace("-", string.Empty);
        if (nip.Length != 10 || nip.Any(chr => !char.IsDigit(chr)))
            return false;
        int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7, 0 };
        int sum = nip.Zip(weights, (d, w) => (d - '0') * w).Sum();
        return (sum % 11) == (nip[9] - '0');
    }
    public bool ValidateRegon(string regon)
    {
        int[] weights9 = { 8, 9, 2, 3, 4, 5, 6, 7 };
        int[] weights14 = { 2, 4, 8, 5, 0, 9, 7, 3, 6, 1, 2, 4, 8 };
        if (regon.Any(chr => !char.IsDigit(chr)))
            return false;
        return regon.Length switch
        {
            9 => regon.Zip(weights9, (d, w) => (d - '0') * w).Sum() % 11 == (regon[8] - '0'),
            14 => regon.Zip(weights14, (d, w) => (d - '0') * w).Sum() % 11 == (regon[13] - '0'),
            _ => false
        };
    }
    public async Task<List<Podmiot>> SearchNipAsync(params string[] nipy)
    {
        foreach (string nip in nipy)
        {
            if (!ValidateNip(nip)) return new List<Podmiot>();
        }
        string joinNips = string.Join("\n", nipy);
        List<PodmiotShort> answer = new();
        if (nipy.Length == 1)
        {
            answer = await _httpService!.SearchAsync(Identyfikator.NIP, nipy[0]);
        }
        else
        {
            answer = await _httpService!.SearchAsync(Identyfikator.NIPy, joinNips);
        }
        List<Podmiot> podmiots = new();
        foreach (var podmiot in answer)
        {
            podmiots.Add(new Podmiot
            {
                Gmina = podmiot.Gmina,
                KodPocztowy = podmiot.KodPocztowy,
                Miejscowosc = podmiot.Miejscowosc,
                Nazwa = podmiot.Nazwa,
                NumerNieruchomosci = podmiot.Numer_Nieruchomosci,
                Powiat = podmiot.Powiat,
                Regon = podmiot.Regon,
                Skreslony = !podmiot.DataZak.Contains('-'),
                Ulica = podmiot.Ulica,
                Wojewodztwo = podmiot.Wojewodztwo
            });
        }
        return podmiots;
    }
}