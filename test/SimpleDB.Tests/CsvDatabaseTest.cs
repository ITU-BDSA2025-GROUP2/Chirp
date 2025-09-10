using System.Diagnostics;

namespace SimpleDB.Tests;

public class CsvDatabaseTest
{
    [Fact]
    public void CsvDatabaseInstantiate()
    {
        var csvDatabase = new CsvDatabase<string>();
        
        Assert.NotNull(csvDatabase);
        Assert.Equal(typeof(CsvDatabase<string>), csvDatabase.GetType());
    }

    [Theory]
    [InlineData(42)] // int
    [InlineData("hello")]   // string         
    [InlineData(true)] // boolean
    [InlineData(3.14)] // double
    public void CsvDatabaseInstantiateGenerics<T>(T sample)
    {
        var csvDatabase = new CsvDatabase<T>();
        Assert.NotNull(csvDatabase);
        Assert.Equal(typeof(CsvDatabase<T>), csvDatabase.GetType());
    }
    
    [Fact]
    public void Read()
    {
        var csvDatabase = new CsvDatabase<string>();
        
        csvDatabase.Read();
    }
}