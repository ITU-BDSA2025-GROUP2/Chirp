namespace SimpleDB.Tests;

public class CsvMessageMappingTest
{
    [Fact]
    public void CsvMessageMappingInstantiate()
    {
        var mapping = new CsvMessageMapping();
        
        Assert.NotNull(mapping);
    }

    [Fact]
    public void MessagesInstantiate()
    {
        var messages = new Messages()
        {
            Author = "Karam", 
            Message = "Hello", 
            Timestamp = "Tomorrow"
        };
        
        Assert.NotNull(messages);
        Assert.NotNull(messages.Author);
        Assert.NotNull(messages.Message);
        Assert.NotNull(messages.Timestamp);
        Assert.Equal("Karam",messages.Author);
        Assert.Equal("Hello",messages.Message);
        Assert.Equal("Tomorrow",messages.Timestamp);
    }
}
