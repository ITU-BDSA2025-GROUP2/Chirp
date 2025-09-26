namespace Chirp.CLI;

using Server;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Messages> records)
    {
        foreach (var rs in records)
        {
            var dataTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(rs.Timestamp);
            var time = dataTimeOffSet.DateTime;
            Console.WriteLine(rs.Author + " @ " + time + " " + rs.Message);
        }
    }
}