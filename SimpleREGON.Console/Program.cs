using SimpleREGON;

var regon = new SimpleRegon();
var result = await regon.Login();
Console.WriteLine(result);
Console.WriteLine();