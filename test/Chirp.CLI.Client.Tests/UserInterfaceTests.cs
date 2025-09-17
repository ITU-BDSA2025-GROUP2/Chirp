using SimpleDB;

namespace chirp.CLI.Client.Tests;

public class UserInterfaceTests
{  
    [Fact]
    public void PrintCheeps()
    {
        var message = new Messages()
        {
            Author = "Karam",
            Message = "Hi",
            Timestamp = "123401230"
        };
        var records =  new List<Messages>();
        records.Add(message);
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        UserInterface.PrintCheeps(records);

        var expectedOutput = "Karam @ 02-01-1970 10:16:41 Hi\r\n";
        Assert.Equal(expectedOutput, stringWriter.ToString());
    }
}
