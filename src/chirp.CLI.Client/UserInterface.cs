using SimpleDB;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Messages> records)
    {
        foreach (var rs in records)
        {
            DateTimeOffset dataTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(rs.Timestamp));
            DateTime time = dataTimeOffSet.DateTime;
            Console.WriteLine(rs.Author + " @ " + time + " " + rs.Message);
        }
        
    }
}