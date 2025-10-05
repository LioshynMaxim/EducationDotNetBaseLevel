using Madule1_Task1_ClassLibrary;

Console.WriteLine($"Please, enter your name");

var userName = Console.ReadLine();

Console.WriteLine(TeskHelloHelper.SayHello(userName ?? ""));
