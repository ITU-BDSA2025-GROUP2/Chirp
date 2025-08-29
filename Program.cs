// See https://aka.ms/new-console-template for more information
if (args[0] == "say")
{
    var message = args[1];
    var freaquency = int.Parse(args[2]);
    foreach (var arg in args)
        Console.WriteLine(message + " ");
}
