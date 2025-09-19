namespace Chirp.CLI.Client.Tests;

using chirp.CLI;
using Xunit;

public class E2ETest1
{



    [Fact]
  public void TestReadCheeps()
  {
      // Arrange
      var args = new string[] { "read" };
      // Act
      var result = Program.Main(args);
      // Assert
      Assert.Equal(0, result);
  }
}


