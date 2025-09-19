namespace Chirp.CLI.Client.Tests;

using Server;
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
      var args = new string[] { "cheeps" };
      // Act
      var result = Program.Main(args);
      // Assert
      Assert.Equal(0, result);
  }
}


