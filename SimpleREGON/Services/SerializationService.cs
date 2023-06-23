using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleREGON.Services;

internal class SerializationService
{
    internal StringContent SerializeBaseRequest(string key, string value)
    {
        return SerializeRequest(new Dictionary<string, string>()
        {
            [key] = value
        });
    }
    internal StringContent SerializeFullDataRequest(string reportName, string regon, string silosId)
    {
        return SerializeRequest(new Dictionary<string, string>()
        {
            ["pNazwaRaportu"] = reportName,
            ["pRegon"] = regon,
            ["pSilosID"] = silosId
        });
    }
    internal StringContent SerializeDataRequest(Dictionary<Parameter, string> parameters)
    {
        Data data = new();
        foreach (var parameter in parameters)
        {
            _ = parameter.Key switch
            {
                Parameter.NIP => data.pParametryWyszukiwania.Nip = parameter.Value,
                Parameter.NIPy => data.pParametryWyszukiwania.Nipy = parameter.Value,
                Parameter.REGON => data.pParametryWyszukiwania.Regon = parameter.Value,
                Parameter.REGONy9 => data.pParametryWyszukiwania.Regony9zn = parameter.Value,
                Parameter.REGONy14 => data.pParametryWyszukiwania.Regony14zn = parameter.Value,
                Parameter.KRS => data.pParametryWyszukiwania.Krs = parameter.Value,
                Parameter.KRSy => data.pParametryWyszukiwania.Krsy = parameter.Value,
                _ => throw new NotImplementedException(),
            };
        }
        return SerializeRequest(data);
    }

    private StringContent SerializeRequest<T>(T data) => new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    private class Data
    {
        public bool jestWojPowGmnMiej { get; set; } = true;
        public Parameters pParametryWyszukiwania { get; set; }
        public Data() => pParametryWyszukiwania = new Parameters();
        public class Parameters
        {
            public string? NazwaPodmiotu { get; set; }
            public string? NumerNieruchomosci { get; set; }
            public string? AdsSymbolGminy { get; set; }
            public string? AdsSymbolMiejscowosci { get; set; }
            public string? AdsSymbolPowiatu { get; set; }
            public string? AdsSymbolUlicy { get; set; }
            public string? AdsSymbolWojewodztwa { get; set; }
            public string? Dzialalnosci { get; set; }
            public bool PrzewazajacePKD { get; set; } = false;
            public string? Regon { get; set; }
            public string? Krs { get; set; }
            public string? Nip { get; set; }
            public string? Regony9zn { get; set; }
            public string? Regony14zn { get; set; }
            public string? Krsy { get; set; }
            public string? Nipy { get; set; }
            public string? NumerwRejestrzeLubEwidencji { get; set; }
            public string? OrganRejestrowy { get; set; }
            public string? RodzajRejestru { get; set; }
            public string? FormaPrawna { get; set; }
        }
    }
}
