namespace SimpleDB;

using CsvHelper;
using Microsoft.VisualBasic;
using System.Globalization;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    //code for singleton implementation taken from: https://csharpindepth.com/Articles/Singleton
    private static CSVDatabase<T> instance = null;
    private static readonly object padlock = new object();

    public static CSVDatabase<T> Instance
    {
        get
        {
            //ensures that another instance cannot be created while the process of making sure only on instance is present is still running
            //can be removed as it can affect performance (will just be less secure if we work with threads)
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new CSVDatabase<T>();
                }
                return instance;
            }
        }
    }


    public IEnumerable<T> Read(int? limit = null)
    {

        string filepath = Path.Combine(AppContext.BaseDirectory, "data", "chirp_cli_db.csv");
        using var reader = new StreamReader(filepath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<csvMessageMapping>();
        var record = csv.GetRecords<Messages>().ToList();

        return (IEnumerable<T>)record;
    }

    public void Store(T record)
    {
        string filepath = Path.Combine(AppContext.BaseDirectory, "data", "chirp_cli_db.csv");
        using var writer = new StreamWriter(filepath, true);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
    }
}