namespace Chirp.CLI.Client.Tests;

using Server;
using chirp.CLI;
using Xunit;

public class E2ETest1 
{

  public E2ETest1()
    {
      var car = new string[] {""};
      ServerProgram.Main(car);
    }

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


