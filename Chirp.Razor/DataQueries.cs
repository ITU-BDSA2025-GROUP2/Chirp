using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;

namespace Chirp.Razor;

public class DataQueries
{

    public void QuerySetup()
    {
        var pathByUser = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        var dbPath = Path.GetTempPath() + "Chirp.db";

        if (pathByUser != null)
        {
            dbPath = pathByUser;
        }

        if (!File.Exists(dbPath))
        {
            CreateDb(dbPath);
        }    
    }


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

    public List<CheepViewModel> GetAllQuery(int page, int limit = 32)
    {

        //TODO if emviorment variable -> use that for db else temp
        var pathByUser = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        var dbPath = Path.GetTempPath() + "Chirp.db";
        
        if (pathByUser != null)
        {
            dbPath = pathByUser;
        }
        
       

        using var connection = new SqliteConnection($"Data Source={dbPath}");

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM message LIMIT {limit} OFFSET {page * 32}";
        //command.CommandText = $"SELECT m.text, m.pub_date FROM message m LIMIT {limit}";

        using var reader = command.ExecuteReader();
        var returnList = new List<CheepViewModel>();
        while (reader.Read())
        {
            //0 = messageid ; 1 = author id ; 2 = message ; 3 = publishing date (in unixTime) 
            returnList.Add(new CheepViewModel(reader.GetString(1), reader.GetString(2), reader.GetString(3)));

        }

        connection.Close();

        return returnList;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page)
    {   
        var pathByUser = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        var dbPath = Path.GetTempPath() + "Chirp.db";
        
        if (pathByUser != null)
        {
            dbPath = pathByUser;
        }

        using var connection = new SqliteConnection($"Data Source={dbPath}");

        connection.Open();

        var limit = 32;

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM message m join user u on m.author_id = u.user_id where u.username = '{author}' LIMIT {limit} OFFSET {page * 32}";

        using var reader = command.ExecuteReader();
        var returnList = new List<CheepViewModel>();
        while (reader.Read())
        {
            //0 = messageid ; 1 = author id ; 2 = message ; 3 = publishing date (in unixTime) 
            returnList.Add(new CheepViewModel(reader.GetString(1), reader.GetString(2), reader.GetString(3)));

        }

        connection.Close();

        return returnList;
        
    }
    
}