using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

class DataQueries {
   

    public void testQuery() {
        //SQLitePCL.raw.SetProvider();
        
        // TODO: Make a function that creates the temp db using the data folder files
        
        using var connection = new SqliteConnection("Data source=/tmp/chirp.db");

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