namespace SimpleDB.Tests;


    
public class CsvDatabaseTest
{
    private readonly IDatabaseRepository<Messages> _csvDatabase;
    //private readonly string _fileName = "chirp_cli_db.csv";
    
    // Instantiate the class here
    public CsvDatabaseTest()
    {
        _csvDatabase = new CsvDatabase<Messages>();
    }
    
    
    [Fact]
    public void CsvDatabaseInstantiate()
    {
        Assert.NotNull(_csvDatabase);
        Assert.Equal(typeof(CsvDatabase<Messages>), _csvDatabase.GetType());
    }

    [Theory]
    [InlineData(42)] // int
    [InlineData("hello")]   // string         
    [InlineData(true)] // boolean
    [InlineData(3.14)] // double
    public void CsvDatabaseInstantiateGenerics<T>(T type)
    {
        var csvDatabase = new CsvDatabase<T>();
        Assert.NotNull(csvDatabase);
        Assert.Equal(typeof(CsvDatabase<T>), csvDatabase.GetType());
    }

    [Fact]
    public void CsvDatabaseInstance()
    {
        
        Assert.IsType<CsvDatabase<Messages>>(_csvDatabase);
    }
    
    [Fact]
    public void Read()
    {
        var read = _csvDatabase.Read();
        
        Assert.NotNull(read);
        Assert.Equal(typeof(List<Messages>), read.GetType());
    }

    [Fact]
    public void ReadWithLimit()
    {
        var read = _csvDatabase.Read(1);
        
        Assert.NotNull(read);
        Assert.Equal(typeof(List<Messages>), read.GetType());
        Assert.Equal(1, read.Count());
    }

    [Fact]
    public void Store()
    {
        var record = new Messages()
        {
            Author = "Karam", 
            Message = "Hello", 
            Timestamp = "112390123"
        };
        
        _csvDatabase.Store(record);
        var read = _csvDatabase.Read();
        
        Assert.Equal(record, read.Last());
    }
}