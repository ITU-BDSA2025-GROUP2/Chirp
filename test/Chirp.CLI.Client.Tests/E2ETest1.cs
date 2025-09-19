namespace Chirp.CLI.Client.Tests;

using Xunit;

public class E2ETest1
{



    [Fact]
  public void TestReadCheeps()
  {
      // Arrange
      var args = new string[] { "cheeps" };
      // Act
      var result = Program.Main(args);
      // Assert
      Assert.Equal(0, result);
  }
}


