using SimpleREGON;

SimpleRegon regon = new();
var serviceStatus = await regon.GetServiceStatusAsync();
var sessionStatus = await regon.GetSessionStatusAsync();
var dateStatus = await regon.GetDateStatusAsync();
Console.WriteLine(serviceStatus);
Console.WriteLine(sessionStatus);
Console.WriteLine(dateStatus);
Console.WriteLine($"Api key  = {regon.GetCurrentApiKey}");
string? input = "7010790303";
Console.WriteLine(await regon.GetDataByNipAsync(input));