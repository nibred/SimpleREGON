using SimpleREGON;

SimpleRegon regon = new();
await regon.LoginAsync();
//string data = await regon.FindByRegonAsync("000173516");
var data = await regon.TryGetServiceStatusAsync();
Console.WriteLine(data.status);
