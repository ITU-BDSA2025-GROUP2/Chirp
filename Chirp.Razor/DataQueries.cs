using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

internal class DataQueries
{

    //create the database files "dump.sql" and "schema.sql"
    //this will only run once, as if the files already exist, they should not have duplicates
    private static void CreateDb(string dbPath)
    {
        var projectDir = AppDomain.CurrentDomain.BaseDirectory;
        var schemaPath = Path.Combine(projectDir, "data", "schema.sql");
        var dumpPath = Path.Combine(projectDir, "data", "dump.sql");
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            var schemaSql = File.ReadAllText(schemaPath);
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = schemaSql;
                cmd.ExecuteNonQuery();
            }
            var dumpSql = File.ReadAllText(dumpPath);
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = dumpSql;
                cmd.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void GetAllQuery(int limit = 32)
    {

        //TODO if emviorment variable -> use that for db else temp
        var dbPath = Path.GetTempPath() + "Chirp.db";
        if (!File.Exists(dbPath))
        {
            CreateDb(dbPath);
        }
 
        using var connection = new SqliteConnection($"Data Source={dbPath}");

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM message LIMIT {limit}";
        //command.CommandText = $"SELECT m.text, m.pub_date FROM message m LIMIT {limit}";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            //0 = messageid ; 1 = author id ; 2 = message ; 3 = publishing date (in unixTime) 
            var message = reader.GetString(2);

            Console.WriteLine(message);
        }

        connection.Close();
    }


}