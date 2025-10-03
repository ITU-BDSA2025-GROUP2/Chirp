using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
public class integration_test : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;



    public integration_test()
    {

    }

    [Fact]
    public void ServerExists()
    {
        var emptyArg = new string[] {" "};

        Assert.Equal(0, _program.Main(emptyArg));
    }
}