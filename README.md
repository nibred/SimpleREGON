# SimpleREGON

**SimpleREGON** to biblioteka .NET umożliwiająca pobieranie danych firm po numerach **NIP**, **REGON** lub **KRS** z [wyszukiwarki REGON](https://wyszukiwarkaregon.stat.gov.pl/appBIR/index.aspx) **bez konieczności używania klucza API**. Dane zwracane są w formacie JSON


## Użycie
#### Inicjalizacja klienta
```csharp
var client = new SimpleRegon(); //lub new SimpleRegon("<twoj-api-klucz>")
```
#### Walidacja numerów
```csharp
bool isValidNip = client.ValidateNip("7282609358"); //true
bool isValidRegon = client.ValidateRegon("123456789"); //false
```
#### Statusy sesji, usługi i danych
```csharp
string stanDanych = await client.GetDateStatusAsync(); 
// {"success": true, "status": "12-05-2025"}

string statusUslugi = await client.GetServiceStatusAsync(); 
// {"success": true, "status": "Usluga dostepna"}

string statusSesji = await client.GetSessionStatusAsync(); 
// {"success": true, "status": "Sesja istnieje"}
```
#### Pobieranie danych
```csharp
string data = await client.GetDataByNipAsync("5210088110"); 
// lub GetDataByRegonAsync("<regon>"), GetDataByKrsAsync("<krs>") 
```
#### Przykładowa odpowiedź
```json
{ 
    "success": true, 
    "data": {
        "regon": "012050075",
        "regon14": "01205007500000",
        "nip": "5210088110",
        "nazwa": "MCDONALD'S POLSKA SPOLKA Z OGRANICZONA ODPOWIEDZIALNOSCIA",
        "numerWRejestrzeLubEwidencji": "0000097409",
        "dataWpisuDoRejestruEwidencji": "2002-04-23",
        "dataPowstania": "1991-07-02",
        "dataRozpoczeciaDzialalnosci": "1991-07-02",
        "dataZaistnieniaZmiany": "2023-08-31",
        "kodPocztowy": "02-674",
        "numerNieruchomosci": "15",
        "numerTelefonu": "222115800",
        "kraj": "POLSKA",
        "wojewodztwo": "MAZOWIECKIE",
        "powiat": "Warszawa",
        "gmina": "Mokotow",
        "miejscowosc": "Warszawa",
        "ulica": "ul. Marynarska",
        "podstawowaFormaPrawna": "1 - OSOBA PRAWNA",
        "szczegolnaFormaPrawna": "117 - SPOLKI Z OGRANICZONA ODPOWIEDZIALNOSCIA",
        "formaWlasnosci": "216 - WLASNOSC ZAGRANICZNA",
        "nazwaOrganuRejestrowego": "SAD REJONOWY DLA M.ST.WARSZAWY W WARSZAWIE,XIII WYDZIAL GOSPODARCZY KRAJOWEGO REJESTRU SADOWEGO",
        "nazwaRodzajuRejestru": "REJESTR PRZEDSIEBIORCOW",
        "jednostekLokalnych": "0",
        "pkds": [
            { "kod": "56.10.A", "nazwa": "RESTAURACJE I INNE STALE PLACOWKI GASTRONOMICZNE" },
            { "kod": "41.10.Z", "nazwa": "REALIZACJA PROJEKTOW BUDOWLANYCH ZWIAZANYCH ZE WZNOSZENIEM BUDYNKOW" },
            { "kod": "41.20.Z", "nazwa": "ROBOTY BUDOWLANE ZWIAZANE ZE WZNOSZENIEM BUDYNKOW MIESZKALNYCH I NIEMIESZKALNYCH" },
            { "kod": "47.11.Z", "nazwa": "SPRZEDAZ DETALICZNA PROWADZONA W NIEWYSPECJALIZOWANYCH SKLEPACH Z PRZEWAGA ZYWNOSCI, NAPOJOW I WYROBOW TYTONIOWYCH" },
            { "kod": "68.10.Z", "nazwa": "KUPNO I SPRZEDAZ NIERUCHOMOSCI NA WLASNY RACHUNEK" },
            { "kod": "68.20.Z", "nazwa": "WYNAJEM I ZARZADZANIE NIERUCHOMOSCIAMI WLASNYMI LUB DZIERZAWIONYMI" },
            { "kod": "70.22.Z", "nazwa": "POZOSTALE DORADZTWO W ZAKRESIE PROWADZENIA DZIALALNOSCI GOSPODARCZEJ I ZARZADZANIA" },
            { "kod": "82.99.Z", "nazwa": "POZOSTALA DZIALALNOSC WSPOMAGAJACA PROWADZENIE DZIALALNOSCI GOSPODARCZEJ, GDZIE INDZIEJ NIESKLASYFIKOWANA" }
        ]
    }
}
```
## Obsługa błędów
Każda odpowiedź zawiera pole **success**. W przypadku błędu zwracany jest obiekt JSON z **success = false** oraz opisem błędu w polu **description**
```json
{
  "success": false,
  "description": "result is empty"
}
```
    
## Licencja
MIT
