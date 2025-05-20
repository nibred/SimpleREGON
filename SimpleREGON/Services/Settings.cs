namespace SimpleREGON.Services;

internal static class Settings
{
    public const string UrlMainPage = "https://wyszukiwarkaregon.stat.gov.pl/appBIR/index.aspx";
    public const string UrlApiBase = "https://wyszukiwarkaregon.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc/ajaxEndpoint/";
    public const string UrlApiHost = "wyszukiwarkaregon.stat.gov.pl";

    public const string UrlApiLoginEndpoint = UrlApiBase + "Zaloguj";
    public const string UrlApiDataEndpoint = UrlApiBase + "daneSzukaj";
    public const string UrlApiFullDataEndpoint = UrlApiBase + "DanePobierzPelnyRaport";
    public const string UrlApiGetValueEndpoint = UrlApiBase + "GetValue";

    public const string ApiKeyNotFound = "api key not found";
    public const string IncorrectValue = "incorrect value";
    public const string ParseError = "result is empty";

    public static readonly Dictionary<string, string> SessionStatusCodeDescription = new()
    {
        ["0"] = "Sesja nie istnieje",
        ["1"] = "Sesja istnieje"
    };

    public static readonly Dictionary<string, string> ServiceStatusCodeDescription = new()
    {
        ["0"] = "Usługa niedostępna",
        ["1"] = "Usługa dostępna",
        ["2"] = "Przerwa techniczna"
    };

    public static readonly Dictionary<string, string> ResultKeysTranscription = new()
    {
        ["regon9"] = "regon",
        ["regon14"] = "regon14",
        ["nip"] = "nip",
        ["krs"] = "krs",
        ["statusnip"] = "statusNip",
        ["nazwisko"] = "nazwisko",
        ["imie1"] = "pierwszeImie",
        ["imie2"] = "drugieImie",
        ["nazwa"] = "nazwa",
        ["numertelefonu"] = "numerTelefonu",
        ["adresemail"] = "email",
        ["adresstronyinternetowej"] = "site",
        ["numerfaksu"] = "faks",
        ["nazwapodstawowejformyprawnej"] = "podstawowaFormaPrawna",
        ["nazwaszczegolnejformyprawnej"] = "szczegolnaFormaPrawna",
        ["nazwaformywlasnosci"] = "formaWlasnosci",
        ["datawpisudoregon"] = "dataWpisuDoRegon",
        ["dataskresleniazregon"] = "dataSkresleniaZRegon",
        ["dataskresleniazregondzial"] = "dataSkresleniaZRegon",
        ["datapowstania"] = "dataPowstania",
        ["datarozpoczeciadzialalnosci"] = "dataRozpoczeciaDzialalnosci",
        ["datawpisudoregondzialalnosci"] = "dataWpisuDoRegonDzialalnosci",
        ["datazawieszeniadzialalnosci"] = "dataZawieszeniaDzialalnosci",
        ["datawpisudorejestruewidencji"] = "dataWpisuDoRejestruEwidencji",
        ["datawznowieniadzialalnosci"] = "dataWznowieniaDzialalnosci",
        ["datazakonczeniadzialalnosci"] = "dataZakonczeniaDzialalnosci",
        ["numerwrejestrzelubewidencji"] = "numerWRejestrzeLubEwidencji",
        ["adsiedznazwakraju"] = "kraj",
        ["adsiedznazwawojewodztwa"] = "wojewodztwo",
        ["adsiedznazwapowiatu"] = "powiat",
        ["adsiedznazwagminy"] = "gmina",
        ["adsiedznazwamiejscowosci"] = "miejscowosc",
        ["adsiedznazwaulicy"] = "ulica",
        ["nazwaorganurejestrowego"] = "nazwaOrganuRejestrowego",
        ["nazwarodzajurejestru"] = "nazwaRodzajuRejestru",
        ["adsiedzkodpocztowy"] = "kodPocztowy",
        ["adsiedznumernieruchomosci"] = "numerNieruchomosci",
        ["adsiedznumerlokalu"] = "numerLokalu",
        ["pkdkod"] = "kod",
        ["pkdnazwa"] = "nazwa",
        ["jednosteklokalnych"] = "jednostekLokalnych",
        ["datazaistnieniazmiany"] = "dataZaistnieniaZmiany"
    };
}
