namespace Server.Tests;

public class CsvMessageMappingTests
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
            Timestamp = "1231239123"
        };
        
        Assert.NotNull(messages);
        Assert.NotNull(messages.Author);
        Assert.NotNull(messages.Message);
        Assert.NotNull(messages.Timestamp);
        Assert.Equal("Karam",messages.Author);
        Assert.Equal("Hello",messages.Message);
        Assert.Equal("1231239123",messages.Timestamp);
    }
}
