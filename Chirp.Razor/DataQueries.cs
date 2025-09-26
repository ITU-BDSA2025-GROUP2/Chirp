using Microsoft.Data.Sqlite;




class DataQueries {
   

    public void testQuery() {
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