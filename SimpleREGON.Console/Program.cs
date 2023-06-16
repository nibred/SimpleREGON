using SimpleREGON;

SimpleRegon regon = new ();
await regon.LoginAsync();
var data = await regon.FindByRegonAsync("");
Console.WriteLine(data);
