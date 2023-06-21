using SimpleREGON;

SimpleRegon regon = new();
await regon.LoginAsync();
string data = await regon.FindByRegonAsync("002195172", "000173516");
Console.WriteLine(data);
