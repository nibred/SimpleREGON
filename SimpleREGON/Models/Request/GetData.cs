using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleREGON.Models.Request;

internal class GetData
{
    public bool jestWojPowGmnMiej { get; set; } = true;
    public Parameters pParametryWyszukiwania { get; set; }
}

internal class Parameters
{
    public string? NazwaPodmiotu { get; set; } = null;
    public string? NumerNieruchomosci { get; set; } = null;
    public string? AdsSymbolGminy { get; set; } = null;
    public string? AdsSymbolMiejscowosci { get; set; } = null;
    public string? AdsSymbolPowiatu { get; set; } = null;
    public string? AdsSymbolUlicy { get; set; } = null;
    public string? AdsSymbolWojewodztwa { get; set; } = null;
    public string? Dzialalnosci { get; set; } = null;
    public bool PrzewazajacePKD { get; set; } = false;
    public string? Regon { get; set; } = null;
    public string? Krs { get; set; } = null;
    public string? Nip { get; set; } = null;
    public string? Regony9zn { get; set; } = null;
    public string? Regony14zn { get; set; } = null;
    public string? Krsy { get; set; } = null;
    public string? Nipy { get; set; } = null;
    public string? NumerwRejestrzeLubEwidencji { get; set; } = null;
    public string? OrganRejestrowy { get; set; } = null;
    public string? RodzajRejestru { get; set; } = null;
    public string? FormaPrawna { get; set; } = null;
}