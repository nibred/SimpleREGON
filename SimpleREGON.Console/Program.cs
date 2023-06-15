using SimpleREGON;

SimpleRegon regon = new ();
//await regon.LoginAsync();
//var data = await regon.SearchNipAsync("9271651056", "9261497205", "9261643635", "9271789151", "9261540728");
//Console.WriteLine(data[0].Nazwa);
//Console.WriteLine(data[1].Nazwa);
//Console.WriteLine(data[2].Nazwa);
//Console.WriteLine(data[3].Nazwa);
//Console.WriteLine(data[4].Nazwa);
Console.WriteLine(regon.ValidateRegon("41726098878123"));