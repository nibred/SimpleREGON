using SimpleREGON.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleREGON;

public class SimpleRegon
{
    private readonly HttpService? _httpService;
    public SimpleRegon() => _httpService ??= new HttpService();
    public async Task<bool> Login() => await _httpService.Login();
    public async Task<bool> Login(string apiKey) => await _httpService.Login(apiKey);
    public bool ValidateNip(string nip)
    {
        nip = nip.Replace("-", string.Empty);
        if (nip.Length != 10 || nip.Any(chr => !Char.IsDigit(chr)))
            return false;
        int[] weights = { 6, 5, 7, 2, 3, 4, 5, 6, 7, 0 };
        int sum = nip.Zip(weights, (digit, weight) => (digit - '0') * weight).Sum();
        return (sum % 11) == (nip[9] - '0');
    }
}