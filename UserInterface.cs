

public static class UserInterface
{
    public static void PrintCheeps(List<Messages> records)
    {
        foreach (var rs in records)
        {
            DateTimeOffset dataTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(rs.Timestamp));
            DateTime time = dataTimeOffSet.DateTime;
            Console.WriteLine(rs.Author + " @ " + time + " " + rs.Message);
        }
        
    }
}