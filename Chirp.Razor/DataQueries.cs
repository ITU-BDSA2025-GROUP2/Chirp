using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

class DataQueries {
   

    public void testQuery() {
        //SQLitePCL.raw.SetProvider();
        
        using var connection = new SqliteConnection("Data source=chirp.db");

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "Select * from message limit 32";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var message = reader.GetString(0);

            Console.WriteLine(message);
        }
    }

}