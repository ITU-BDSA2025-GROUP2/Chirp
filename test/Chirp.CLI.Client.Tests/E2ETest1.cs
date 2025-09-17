

using Xunit;
using Chirp.CLI;

namespace test;
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


