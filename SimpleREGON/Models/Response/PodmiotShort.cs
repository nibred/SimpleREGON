using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleREGON.Models.Response;

internal class PodmiotShort
{
    public string Regon { get; set; }
    public string RegonLink { get; set; }
    public string Typ { get; set; }
    public string Nazwa { get; set; }
    public string Wojewodztwo { get; set; }
    public string Powiat { get; set; }
    public string Gmina { get; set; }
    public string KodPocztowy { get; set; }
    public string Miejscowosc { get; set; }
    public string Ulica { get; set; }
    public string Numer_Nieruchomosci { get; set; }
    public string DataZak { get; set; }
}
