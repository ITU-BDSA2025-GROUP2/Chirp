using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;



public class end2end
{
    private Program _program;
    public end2end()
    {
        var car = new string[] { "" };
    }

    [Fact]
    public async Task ProgramRuns()
    {
        var args = new string[] { "cheeps" };

        var result = Program.Main(args);
        Program.Stop();

        Assert.Equal(0, await result);


    }
}