

using Xunit;

namespace test;
public class E2ETest1
{



    [Fact]
    public void TestReadCheep()
    {
        // Arrange
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "/usr/bin/dotnet";
            process.StartInfo.Arguments = "./src/Chirp.CLI.Client/bin/Debug/net8.0/chirp.dll read";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        string fstCheep = output.Split("\n")[0];
        // Assert
        Assert.StartsWith("ropf", fstCheep);
        Assert.EndsWith("Hello, BDSA students!", fstCheep);
    }
}

