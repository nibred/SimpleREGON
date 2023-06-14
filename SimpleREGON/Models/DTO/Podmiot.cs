namespace SimpleREGON.Models.DTO;

public class Podmiot
{
    public string Regon { get; init; }
    public string Nazwa { get; init; }
    public string Wojewodztwo { get; init; }
    public string Powiat { get; init; }
    public string Gmina { get; init; }
    public string KodPocztowy { get; init; }
    public string Miejscowosc { get; init; }
    public string Ulica { get; init; }
    public string NumerNieruchomosci { get; init; }
    public bool Skreslony { get; init; }
}
