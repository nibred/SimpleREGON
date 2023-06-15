using SimpleREGON;

SimpleRegon regon = new ();
await regon.LoginAsync();
var data = await regon.FindByRegonAsync("012050075");
Console.WriteLine(data[0].Regon);