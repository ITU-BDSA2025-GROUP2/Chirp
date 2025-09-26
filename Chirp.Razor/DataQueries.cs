using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

class DataQueries {


    public void CreateDb()
    {
        string strCmdText;
        strCmdText= "/C sqlite3 /tmp/chirp.db < data/schema.sql";
        System.Diagnostics.Process.Start("CMD.exe",strCmdText);
    }
   

    public void testQuery() {
        CreateDb();
        
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