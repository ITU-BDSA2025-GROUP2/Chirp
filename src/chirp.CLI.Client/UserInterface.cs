namespace chirp.CLI;

using Server;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Messages> records)
    {
        foreach (var rs in records)
        {
            var dataTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(rs.Timestamp));
            var time = dataTimeOffSet.DateTime;
            Console.WriteLine(rs.Author + " @ " + time + " " + rs.Message);
        }
    }
}