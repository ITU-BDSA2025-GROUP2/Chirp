namespace Chirp.CLI.Client.Tests;
using Server;


//comment test

public class UserInterfaceTests
{
    [Fact]
    public void PrintCheeps()
    {
        var message = new Messages()
        {
            Author = "Karam",
            Message = "Hi",
            Timestamp = 123401230
        };
        var records = new List<Messages>();
        records.Add(message);
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        UserInterface.PrintCheeps(records);

        var expectedOutput = "Karam @ 01/02/1970 10:16:41 Hi\n";
        Assert.Equal(expectedOutput, stringWriter.ToString());
    }
}
