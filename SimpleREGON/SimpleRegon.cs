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
    public SimpleRegon(string apiKey = "") => _httpService ??= new HttpService(apiKey);
}
