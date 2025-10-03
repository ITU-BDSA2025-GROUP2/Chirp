using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

internal class DataQueries
{
    
    private static void CreateDb()
    {
        var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dbPath = Path.Combine(userDir, "tmp", "chirp.db");

        if (File.Exists(dbPath)) return; // Quit early if the database exists
        var dbDir = Path.GetDirectoryName(dbPath)!;
        Directory.CreateDirectory(dbDir);

        var projectDir = AppDomain.CurrentDomain.BaseDirectory;
        var schemaPath = Path.Combine(projectDir, "data", "schema.sql");
        var dumpPath = Path.Combine(projectDir, "data", "dump.sql");
        
        var connectionString = $"Data Source={dbPath}";

        using (var connection = new SqliteConnection(connectionString))
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
    
    public void GetAllQuery(int limit = 32) {
        var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dbPath = Path.Combine(userDir, "tmp", "chirp.db");
        if (!File.Exists(dbPath))
        {
            CreateDb();
        }
        
        using var connection = new SqliteConnection($"Data Source={dbPath}");

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM message LIMIT {limit}";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var message = reader.GetString(0);

            Console.WriteLine(message);
        }

        connection.Close();
    }

}