using SimpleREGON;

SimpleRegon regon = new();
await regon.LoginAsync();
//string data = await regon.FindByRegonAsync("000173516");
string data = await regon.GetDateStatusAsync();
Console.WriteLine(data);
