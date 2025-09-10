namespace SimpleDB;

using CsvHelper;
using Microsoft.VisualBasic;
using System.Globalization;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader("../SimpleDB/chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<csvMessageMapping>();
        var record = csv.GetRecords<Messages>().ToList();

        return (IEnumerable<T>)record;
    }

    public void Store(T record)
    {

        using var writer = new StreamWriter("../SimpleDB/chirp_cli_db.csv", true);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csvWriter.WriteRecord(record);
        csvWriter.NextRecord();
    }
}