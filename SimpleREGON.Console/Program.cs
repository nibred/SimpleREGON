using SimpleREGON;

SimpleRegon regon = new();
await regon.LoginAsync();
string data = await regon.FindByRegonAsync("241123546", "015603280");
Console.WriteLine(data);
