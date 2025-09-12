using SimpleDB;
using System.CommandLine;

namespace test;

public class IntegrationTest
{
    [Fact]

    public void Test1()
    {
        var args = new string[] { "cheep", "Test1" };

        Program.Main(args);

        var args2 = new string[] { "read" };

        var result = Program.Main(args2);

        Assert.Contains("Test1", result);
    }
}