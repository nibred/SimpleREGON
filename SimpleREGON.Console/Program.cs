using SimpleREGON;

SimpleRegon regon = new();
await regon.Login();
string data = await regon.FindByNipAsync("5210088110");
Console.WriteLine(data);
await Task.Delay(70000);
data = await regon.FindByRegonAsync("241123546", "015603280");
Console.WriteLine(data);
Console.ReadLine();